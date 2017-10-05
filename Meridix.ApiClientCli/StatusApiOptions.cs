using CommandLine;

namespace Meridix.ApiClientCli
{
    [Verb("status", HelpText = "Status API")]
    public class StatusApiOptions : ApiOptions
    {
        [Option('e', "execute", Required = true, HelpText = "Execute a status request against Meridix")]
        public bool Execute { get; set; }
    }
}