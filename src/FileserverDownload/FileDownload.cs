using FileServerDownload.FileServer;
using Microsoft.Extensions.Logging;

namespace FileServerDownload;

internal sealed class FileDownload
{
    private readonly ILogger<FileDownload> _logger;
    private readonly Setting _setting;

    public FileDownload(
        ILogger<FileDownload> logger,
        Setting setting)
    {
        _setting = setting;
        _logger = logger;
    }

    public async Task DownloadAsync()
    {
        _logger.LogInformation("Starting downloading file.");

        using var httpClientHandler = new HttpClientHandler
        {
            // The file-server might return redirects,
            // we do not want to follow the redirects.
            AllowAutoRedirect = false,
            CheckCertificateRevocationList = true,
        };

        using var httpClient = new HttpClient(httpClientHandler);

        var httpFileServer = new HttpFileServer(
            httpClient,
            _setting.Username,
            _setting.Password,
            new Uri(_setting.HostUrl));

        var newestFile = await GetNewestFileAsync(
            httpFileServer, _setting.ResourcePath,
            _setting.FileNamePrefix
        ).ConfigureAwait(false);

        if (newestFile is not null)
        {
            var downloadPath = Path.Combine(newestFile.DirPath, newestFile.Name);
            var outputPath = Path.Combine(_setting.OutputDirectory, newestFile.Name);
            _logger.LogInformation(
                "Downloading {DownloadPath} to {Directory}.",
                downloadPath,
                outputPath);

            await DownloadFileAsync(
                httpFileServer,
                downloadPath,
                outputPath
            ).ConfigureAwait(false);

            _logger.LogInformation("Finished downloading {ResourcePath}.", downloadPath);
        }
        else
        {
            _logger.LogWarning(
                "Could not find any files in the {Folder} with the {FilePrefix}.",
                _setting.ResourcePath,
                _setting.FileNamePrefix);
        }
    }

    private static async Task DownloadFileAsync(
        HttpFileServer httpFileServer,
        string externalPath,
        string localFilePath)
    {
        var fileByteAsyncEnumerable = httpFileServer
            .DownloadFile(externalPath)
            .ConfigureAwait(false);

        using var fileStream = new FileStream(
            localFilePath,
            FileMode.Create,
            FileAccess.Write);

        await foreach (var buffer in fileByteAsyncEnumerable)
        {
            await fileStream.WriteAsync(buffer).ConfigureAwait(false);
        }

        await fileStream.FlushAsync().ConfigureAwait(false);
    }

    private static async Task<FileServerFileInfo?> GetNewestFileAsync(
        HttpFileServer httpFileServer,
        string resourcePath,
        string fileStartName)
    {
        var filesFileServer = await httpFileServer
            // We use no path, since the user specifies the full resource path
            .ListFiles(resourcePath)
            .ConfigureAwait(false);

        return filesFileServer
            .Where(x => x.Name.StartsWith(fileStartName, StringComparison.Ordinal))
            .OrderByDescending(x => x.Created)
            .FirstOrDefault();
    }
}
