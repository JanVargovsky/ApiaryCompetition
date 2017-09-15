using CommandLine;

namespace ApiaryCompetition
{
    class CLIOptions
    {
        [Option('m', nameof(RunType),
            Default = RunType.OneTime,
            HelpText = "Run mode (OneTime | Mine)."
        )]
        public RunType RunType { get; set; }

        [Option('l', nameof(LogProgress),
            Default = false,
            HelpText = "Log progress."
        )]
        public bool LogProgress { get; set; }

        [Option('a', nameof(SaveApiCalls),
            Default = false,
            HelpText = "Save rest api calls (requests and responses)."
        )]
        public bool SaveApiCalls { get; set; }

        [Option('s', nameof(SaveSolutions),
            Default = false,
            HelpText = "Save solved solutions."
        )]
        public bool SaveSolutions { get; set; }
    }

    enum RunType
    {
        OneTime,
        Mine,
    }
}
