using System;
using System.Collections.Generic;

namespace <%= projectname %>
{
    internal class SelectiveScriptHelper : Comparer<string>
    {
        public static bool IsMigration(string s) => s?.StartsWith("<%= projectname %>.Scripts.MigrationScripts") ?? false;
        public static bool IsApplicable(string s) => IsMigration(s) || (s?.StartsWith("<%= projectname %>.Scripts.NonMigrationScripts") ?? false);

        public static IComparer<string> Comparer { get; } = new SelectiveScriptHelper();

        public override int Compare(string x, string y)
        {
            if (!IsApplicable(x)) throw new ArgumentException($"Invalid script ${x}", nameof(x));
            if (!IsApplicable(y)) throw new ArgumentException($"Invalid script ${y}", nameof(y));
            var isXMigration = IsMigration(x);
            var isYMigration = IsMigration(y);

            if (isXMigration == isYMigration)
                return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);

            return isXMigration ? -1 : 1;   // Migration scripts before non migration scripts
        }
    }
}