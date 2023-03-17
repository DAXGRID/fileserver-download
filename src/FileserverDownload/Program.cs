using System.Globalization;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace FileServerDownload;

public record Setting
{
    [Option('r', "resource-path", Required = true, HelpText = "Fileserver resource path.")]
    public string ResourcePath { get; init; } = "";

    [Option('u', "username", Required = true, HelpText = "Fileserver username.")]
    public string Username { get; init; } = "";

    [Option('p', "password", Required = true, HelpText = "Fileserver password.")]
    public string Password { get; init; } = "";

    [Option('f', "filename-prefix", Required = true, HelpText = "Fileserver filename prefix.")]
    public string FileNamePrefix { get; init; } = "";
}

internal static class Program
{
    public static void Main(string[] args)
    {
        Parser.Default
            .ParseArguments<Setting>(args)
            .WithParsed<Setting>(o =>
            {
                var provider = new ServiceCollection()
                    .AddLogging(x => x.AddSerilog(GetLogger()))
                    .AddSingleton<Setting>(o)
                    .AddSingleton<FileDownload>()
                    .BuildServiceProvider();

                var logger = provider.GetService<ILoggerFactory>()
                     !.CreateLogger(nameof(Program));

                try
                {
                    provider.GetService<FileDownload>()!.Download();
                }
                catch (Exception ex)
                {
                    logger.LogError("{Exception}", ex);
                    throw;
                }
            });
    }

    public static Logger GetLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
    }
}
