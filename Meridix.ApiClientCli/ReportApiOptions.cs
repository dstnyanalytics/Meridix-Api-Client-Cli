using CommandLine;

namespace Meridix.ApiClientCli
{
    [Verb("report", HelpText = "Report API")]
    public class ReportApiOptions : ApiOptions
    {
        [Option('p', "parameter-file", Required = false, Default = "report-parameters.json", HelpText = "Relative or absolute file path to the parameter json file")]
        public string ParameterFilePath { get; set; }

        [Option('f', "format", Required = false, Default = "json", HelpText = "The result format [json|xlsx|csv]")]
        public string Format { get; set; }

        [Option('o', "output-file", Required = true, HelpText = "Relative or absolute file path to the result file")]
        public string OutputFile { get; set; }

        [Option('l', "language", Required = false, HelpText = "en-GB|sv-SE etc", Default = "en-GB")]
        public string Language { get; set; }

        [Option("from-date", Required = false, HelpText = "Override the from date in the parameter file [yyyyMMdd]")]
        public string FromDate { get; set; }

        [Option("to-date", Required = false, HelpText = "Override the to date in the parameter file [yyyyMMdd]")]
        public string ToDate { get; set; }
    }
}