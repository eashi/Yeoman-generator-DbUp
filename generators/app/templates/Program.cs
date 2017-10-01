using DbUp;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace <%= projectname %>
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = new Options();
            var isOptionsValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

            if( !isOptionsValid )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid parameters");

            }

            int exitCode;
            try
            {
                var result = RunDatabaseScripts(options.ConnectionStringName, options.LogToConsole, options.WhatIf);
                if (result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success!");
                    Console.ResetColor();
                    exitCode = 0;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
                    exitCode = -1;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                exitCode = -1;
            }
            finally
            {
                if (Debugger.IsAttached)
                {
                    Console.Read();
                }
            }

            return exitCode;
        }

        private static DatabaseUpgradeResult RunDatabaseScripts(string targetDatabase, bool logToConsole = true, bool whatif = false)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[targetDatabase] != null ? ConfigurationManager.ConnectionStrings[targetDatabase].ConnectionString : targetDatabase;

            return Upgrade(connectionString, logToConsole, whatif);
        }

        public static DatabaseUpgradeResult Upgrade(string connectionString, bool logToConsole, bool whatif)
        {
            var schemaUpgradeResult = RunMigrationScripts(connectionString, logToConsole, whatif);

            if (!schemaUpgradeResult.Successful)
                return schemaUpgradeResult;

            return RunNonMigratingScripts(connectionString, logToConsole, whatif);
        }

        public static DatabaseUpgradeResult RunMigrationScripts(string connectionString, bool logToConsole, bool whatif)
        {
            Console.WriteLine("=== Executing migrating scripts ===");
           return BuildAndRunUpgrader( 
               connectionString, 
               logToConsole, 
               whatif, 
               builder =>
               {
                   return builder
                   .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), name => name.StartsWith("<%= projectname %>.Scripts.MigrationScripts"));
               }
               );

        }

        public static DatabaseUpgradeResult RunNonMigratingScripts(string connectionString, bool logToConsole, bool whatif)
        {
            Console.WriteLine("=== Executing Non-migrating scripts ===");
            return BuildAndRunUpgrader(
               connectionString,
               logToConsole,
               whatif,
               builder =>
               {
                   return builder.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), name => name.StartsWith("<%= projectname %>.Scripts.NonMigrationScripts"))
                   .JournalTo(new NullJournal());
               });
        }

        public static DatabaseUpgradeResult BuildAndRunUpgrader(string connectionString, bool logToConsole, bool whatif, Func<UpgradeEngineBuilder, UpgradeEngineBuilder> buildWithScriptsFilter)
        {
            var deployer =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithTransaction();

            deployer = buildWithScriptsFilter(deployer);

            deployer = logToConsole ? deployer.LogToConsole() : deployer.LogToTrace();

            var upgrader = deployer.Build();
            if (whatif)
            {
                try
                {
                    var result = new DatabaseUpgradeResult(upgrader.GetScriptsToExecute(), true, null);
                    Console.WriteLine("WHATIF Mode!");
                    Console.WriteLine("The following scripts would have been executed:");
                    foreach (var script in result.Scripts) { Console.WriteLine(script.Name); }
                    return result;
                }
                catch (ArgumentException ex)
                {
                    return new DatabaseUpgradeResult(new List<SqlScript>(), false, ex);
                }
            }
            else
                return upgrader.PerformUpgrade();
        }
    }

}
