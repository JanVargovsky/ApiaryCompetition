using CommandLine;

namespace ApiaryCompetition
{
    class CLIOptions
    {
        [Option(
            Default = RunType.OneTime,
            HelpText = "Run mode (OneTime | Mine)."
        )]
        public RunType RunType { get; set; }

        [Option(
            Default = false,
            HelpText = "Log progress."
        )]
        public bool LogProgress { get; set; }

        [Option(
            Default = false,
            HelpText = "Save rest api calls (requests and responses)."
        )]
        public bool SaveApiCalls { get; set; }

        [Option(
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
