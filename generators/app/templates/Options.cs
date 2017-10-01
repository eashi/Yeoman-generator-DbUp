using CommandLine;

namespace <%= projectname %>
{
    public class Options
    {
        [Option(shortName:'c', longName:"connectionstring", DefaultValue = "local", HelpText = "Connectionstring name in config, or connectingstring itself.")]
        public string ConnectionStringName { get; set; }

        [Option(shortName:'w', longName:"whatif", DefaultValue = false, HelpText = "Displays the scripts to be executed without executing effectively.")]
        public bool WhatIf { get; set; }

        [Option(shortName:'l', longName:"logtoconsole", DefaultValue = true, HelpText = "if set the result is output on the console")]
        public bool LogToConsole { get; set; }
    }
}
