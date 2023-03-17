using System.Globalization;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace FileServerDownload;

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
