using CommandLine;

namespace Meridix.ApiClientCli
{
    public abstract class ApiOptions
    {
        [Option('u', "base-url", Required = true, HelpText = "The base url to the Meridix system e.g. https://reports.company.com")]
        public string BaseUrl { get; set; }

        [Option('t', "token", Required = true, HelpText = "The Meridix API Ticket token")]
        public string Token { get; set; }

        [Option('s', "secret", Required = true, HelpText = "The Meridix API Ticket secret")]
        public string Secret { get; set; }
    }
}