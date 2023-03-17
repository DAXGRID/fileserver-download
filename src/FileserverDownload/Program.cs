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
    [Option('h', "host-url", Required = true, HelpText = "Fileserver host-url, example 'https://fileserver.mydomain.com'.")]
    public string HostUrl { get; init; } = "";

    [Option('r', "resource-path", Required = true, HelpText = "Fileserver resource path, example '/my-folder-name'")]
    public string ResourcePath { get; init; } = "";

    [Option('u', "username", Required = true, HelpText = "Fileserver username.")]
    public string Username { get; init; } = "";

    [Option('p', "password", Required = true, HelpText = "Fileserver password.")]
    public string Password { get; init; } = "";

    [Option('f', "filename-prefix", Required = true, HelpText = "Fileserver filename prefix, example 'my-file-name'.")]
    public string FileNamePrefix { get; init; } = "";

    [Option('o', "output-directory", Required = true, HelpText = "The path to the directory where the file should be saved on the local file-system.")]
    public string OutputDirectory { get; init; } = "";
}

internal static class Program
{
    public static async Task Main(string[] args)
    {
        await Parser.Default
            .ParseArguments<Setting>(args)
            .WithParsedAsync<Setting>(async o =>
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
                    await provider.GetService<FileDownload>()
                        !.DownloadAsync()
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError("{Exception}", ex);
                    throw;
                }
            }).ConfigureAwait(false);
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
