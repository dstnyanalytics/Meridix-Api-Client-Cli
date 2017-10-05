using CommandLine;

namespace Meridix.ApiClientCli
{
    [Verb("scaffold", HelpText = "Scaffold config files")]
    public class ScaffoldOptions
    {
        [Option('p', "parameter-file", Required = false, HelpText = "Scaffold the report parameters file")]
        public bool ParameterFile { get; set; }

        [Option("parameter-file-path", Required = false, Default = "report-parameters.json", HelpText = "Relative or absolute file path to the parameter json file")]
        public string ParameterFilePath { get; set; }
    }
}