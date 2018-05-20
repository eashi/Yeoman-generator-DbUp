using DbUp;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using DbUp.ScriptProviders;

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
                return -1;
            }

            int exitCode;
            try
            {
                var result = RunDatabaseScripts(options.ConnectionStringName, options.LogToConsole, options.WhatIf, TimeSpan.FromSeconds(options.TimeoutInSeconds));
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

        private static DatabaseUpgradeResult RunDatabaseScripts(string targetDatabase, bool logToConsole = true, bool whatif = false, TimeSpan? timeout = null)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[targetDatabase]?.ConnectionString ?? targetDatabase;

            return Upgrade(connectionString, logToConsole, whatif, timeout);
        }

        public static DatabaseUpgradeResult Upgrade(string connectionString, bool logToConsole, bool whatif, TimeSpan? timeout)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);
            var deployer =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithTransaction()
                    .WithExecutionTimeout(timeout)
                    .WithScripts(
                        new OrderedScriptPrvider(
                            new EmbeddedScriptProvider(Assembly.GetExecutingAssembly(), SelectiveScriptHelper.IsApplicable),
                            SelectiveScriptHelper.Comparer))
                    .JournalToSqlTableJournalSelective("dbo", "SchemaVersions", SelectiveScriptHelper.IsMigration);

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

            return upgrader.PerformUpgrade();
        }
    }

}
