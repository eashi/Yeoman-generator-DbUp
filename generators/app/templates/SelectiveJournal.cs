using System;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Support.SqlServer;

namespace <%= projectname %>
{
    //
    // Apply conditions to skip journal, for certain scripts
    //  thus making their execution repeatable.
    //
    internal class SelectiveJournal : IJournal
    {
        private readonly IUpgradeLog _logger;
        private readonly IJournal _journal;
        private readonly Func<string, bool> _shouldJournalPredicate;

        public SelectiveJournal(IJournal journal, Func<string, bool> shouldJournalPredicate, Func<IUpgradeLog> logger)
        {
            _journal = journal ?? throw new ArgumentNullException(nameof(journal));
            _shouldJournalPredicate = shouldJournalPredicate ?? (s => true);
            _logger = logger();
        }

        public string[] GetExecutedScripts()
        {
            return _journal.GetExecutedScripts();
        }

        public void StoreExecutedScript(SqlScript script)
        {
            if (!_shouldJournalPredicate(script.Name)) return;
            _journal.StoreExecutedScript(script);
        }
    }

    public static class SelectiveJournalExtensions
    {
        public static UpgradeEngineBuilder JournalToSqlTableJournalSelective(this UpgradeEngineBuilder builder, string schema, string table, Func<string, bool> shouldJournalPredicate)
        {
            builder.Configure(c => c.Journal = new SelectiveJournal(new SqlTableJournal(() => c.ConnectionManager, () => c.Log, schema, table), shouldJournalPredicate, () => c.Log));
            return builder;
        }
    }
}