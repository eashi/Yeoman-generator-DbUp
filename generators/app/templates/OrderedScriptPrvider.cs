using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace <%= projectname %>
{
    //
    // Define custom order of scripts retrieved from a script provider
    //
    internal class OrderedScriptPrvider : IScriptProvider
    {
        private IScriptProvider ScriptProvider { get; }
        private IComparer<string> Comparer { get; }

        public OrderedScriptPrvider(IScriptProvider scriptProvider, IComparer<string> comparer)
        {
            ScriptProvider = scriptProvider;
            Comparer = comparer;
        }

        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return ScriptProvider.GetScripts(connectionManager).OrderBy(script => script.Name, Comparer);
        }
    }
}
