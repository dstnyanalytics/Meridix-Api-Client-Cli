using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using CommandLine;
using Meridix.Studio.Api;
using Meridix.Studio.Api.DataTransferObjects.Reports;
using Newtonsoft.Json;

namespace Meridix.ApiClientCli
{
    public class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(eventArgs.ExceptionObject);
                Console.ResetColor();
            };

            var parser = new Parser(config => config.HelpWriter = Console.Out);
            parser.ParseArguments<ReportApiOptions, StatusApiOptions, ScaffoldOptions>(args)
                .MapResult(
                (ReportApiOptions o) => RunAddAndReturnExitCode(o),
                (StatusApiOptions o) => RunAddAndReturnExitCode(o),
                (ScaffoldOptions o) => RunAddAndReturnExitCode(o),
                errors =>
                {
                    foreach (var error in errors)
                        Console.WriteLine(error.Tag + " | " + error.ToString());
                    return 1;
                });
        }

        public static int RunAddAndReturnExitCode(ReportApiOptions options)
        {
            var client = MeridixStudioClient.Create(options.BaseUrl, options.Token, options.Secret);
            client.OnOutput += (sender, args) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(args.Message);
                Console.ResetColor();
            };

            var reportApi = client.ReportApi();

            var parameterJson = File.ReadAllText(options.ParameterFilePath, Encoding.GetEncoding("ISO-8859-1"));
            var parameters = JsonConvert.DeserializeObject<ReportParametersDto>(parameterJson);

            if (!string.IsNullOrWhiteSpace(options.FromDate))
            {
                parameters.FromDate = options.FromDate;
            }

            if (!string.IsNullOrWhiteSpace(options.ToDate))
            {
                parameters.ToDate = options.ToDate;
            }

            switch (options.Format)
            {
                case "csv":
                    CreateCsv(options, reportApi, parameters);
                    break;
                case "xlsx":
                    CreateXlsx(options, reportApi, parameters);
                    break;
                case "json":
                default:
                    CreateJson(options, reportApi, parameters);
                    break;
            }

            return 0;
        }

        private static void CreateXlsx(ReportApiOptions options, ReportApi reportApi, ReportParametersDto parameters)
        {
            var culture = new CultureInfo(options.Language);
            var handle = reportApi.BeginGenerateAsBase64(parameters, "xlsx", new ExportFormatSettings { IncludeCharts = true, IncludeColumnDescriptions = true }, culture);

            Console.WriteLine($"ExecutionId: {handle.ExecutionId}");
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                reportApi.UpdateHandle(ref handle, culture);
                Console.WriteLine(handle.Status);
            } while (!handle.IsCompleted);

            var result = handle.ResultBase64;
            var resultXlsx = Convert.FromBase64String(result);
            File.WriteAllBytes(options.OutputFile, resultXlsx);

            Console.WriteLine($"Created/updated XLSX file {options.OutputFile}");
        }

        private static void CreateCsv(ReportApiOptions options, ReportApi reportApi, ReportParametersDto parameters)
        {
            var culture = new CultureInfo(options.Language);
            var handle = reportApi.BeginGenerateAsBase64(parameters, "csv", new ExportFormatSettings { IncludeCharts = false, IncludeColumnDescriptions = false }, culture);

            Console.WriteLine($"ExecutionId: {handle.ExecutionId}");
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                reportApi.UpdateHandle(ref handle, culture);
                Console.WriteLine(handle.Status);
            } while (!handle.IsCompleted);

            var result = handle.ResultBase64;
            var resultXlsx = Convert.FromBase64String(result);
            File.WriteAllBytes(options.OutputFile, resultXlsx);

            Console.WriteLine($"Created/updated CSV file {options.OutputFile}");
        }

        private static void CreateJson(ReportApiOptions options, ReportApi reportApi, ReportParametersDto parameters)
        {
            var culture = new CultureInfo(options.Language);
            var handle = reportApi.BeginGenerate(parameters, culture);

            Console.WriteLine($"ExecutionId: {handle.ExecutionId}");
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                reportApi.UpdateHandle(ref handle, culture);
                Console.WriteLine(handle.Status);
            } while (!handle.IsCompleted);

            var result = handle.Result;
            var resultJson = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(options.OutputFile, resultJson);

            Console.WriteLine($"Created/updated JSON file {options.OutputFile}");
        }

        public static int RunAddAndReturnExitCode(StatusApiOptions options)
        {
            var client = MeridixStudioClient.Create(options.BaseUrl, options.Token, options.Secret);
            client.OnOutput += (sender, args) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(args.Message);
                Console.ResetColor();
            };

            var statusApi = client.StatusApi();
            var getWorks = statusApi.CanUseGetVerb();
            var putWorks = statusApi.CanUsePutVerb();
            var postWorks = statusApi.CanUsePostVerb();
            var deleteWorks = statusApi.CanUseDeleteVerb();

            Console.WriteLine("PUT Request:".PadRight(20) + (getWorks ? "OK" : "Failed"));
            Console.WriteLine("PUT Request:".PadRight(20) + (putWorks ? "OK" : "Failed"));
            Console.WriteLine("POST Request:".PadRight(20) + (postWorks ? "OK" : "Failed"));
            Console.WriteLine("DELETE Request:".PadRight(20) + (deleteWorks ? "OK" : "Failed"));

            return 0;
        }

        public static int RunAddAndReturnExitCode(ScaffoldOptions options)
        {
            if (options.ParameterFile)
            {
                var parameters = new ReportParametersDto();
                var json = JsonConvert.SerializeObject(parameters, Formatting.Indented);
                if (!File.Exists(options.ParameterFilePath))
                {
                    File.WriteAllText(options.ParameterFilePath, json, Encoding.GetEncoding("ISO-8859-1"));
                    Console.WriteLine($"Created [{options.ParameterFilePath}]");
                }
                else
                {
                    Console.WriteLine($"Did not create [{options.ParameterFilePath}] since it already exists");
                }
            }
            return 0;
        }
    }
}
