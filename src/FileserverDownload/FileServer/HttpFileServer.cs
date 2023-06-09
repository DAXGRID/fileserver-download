using HtmlAgilityPack;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace FileServerDownload.FileServer;

internal sealed class HttpFileServer
{
    private readonly HttpClient _httpClient;

    public HttpFileServer(
        HttpClient httpClient,
        string username,
        string password,
        Uri baseAddress)
    {
        httpClient.BaseAddress = baseAddress;
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                BasicAuthToken(username, password));

        _httpClient = httpClient;
    }

    public async IAsyncEnumerable<byte[]> DownloadFile(string filePath)
    {
        using var response = await _httpClient.GetStreamAsync(filePath)
            .ConfigureAwait(false);

        var read = 0;
        var bufferCount = 4096;
        using var binaryReader = new BinaryReader(response);

        do
        {
            var buffer = binaryReader.ReadBytes(bufferCount);
            read = buffer.Length;
            yield return buffer;
        } while (read == bufferCount);
    }

    public async Task<IEnumerable<FileServerFileInfo>> ListFiles(string dirPath)
    {
        var response = await _httpClient
            .GetAsync(dirPath)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var htmlBody = await response.Content
            .ReadAsStringAsync()
            .ConfigureAwait(false);

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlBody);

        return htmlDocument.DocumentNode
            .Descendants("ul")
            .Where(x => x.Attributes["class"].Value == "item-list")
            .First()
            .Descendants()
            .Where(x => x.Attributes["class"]?.Value == "detail")
            .Select(x => x.InnerText
                    .Trim()
                    .Split("\n")
                    .Select(x => x.Trim())
                    .ToArray())
            // We skip first two, since they do not contain output we want.
            .Skip(2)
            .Where(x => !x[0].EndsWith('/')) // We do not want directories
            .Select(x =>
            {
                var name = x[0];
                var size = SizeShortHandToByteCount(x[1]);
                var created = DateTime.ParseExact(
                    x[2],
                    "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture);

                return new FileServerFileInfo(name, dirPath, size, created);
            });
    }

    /// <summary>
    /// When size is displayed in the HTML, it might be displayed as 'Ki', 'Mi' or 'Gi'.
    /// This function converts that representation to a byte count representation.
    /// </summary>
    private static long SizeShortHandToByteCount(string text)
    {
        var defaultParseLong = long (string x) =>
        {
            return long.Parse(
                x,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture);
        };

        const string kibiBytes = "K";
        const string mibiBytes = "M";
        const string gibiBytes = "G";

        var textUpperCase = text.ToUpperInvariant();
        if (textUpperCase.Contains(kibiBytes, StringComparison.OrdinalIgnoreCase))
        {
            return defaultParseLong(textUpperCase.Split(kibiBytes)[0]) * 1024;
        }
        else if (textUpperCase.Contains(mibiBytes, StringComparison.OrdinalIgnoreCase))
        {
            return defaultParseLong(textUpperCase.Split(mibiBytes)[0]) * 1024 * 1024;
        }
        else if (textUpperCase.Contains(gibiBytes, StringComparison.OrdinalIgnoreCase))
        {
            return defaultParseLong(textUpperCase.Split(gibiBytes)[0]) * 1024 * 1024 * 1024;
        }
        else // Bytes
        {
            return defaultParseLong(text);
        }
    }

    private static string BasicAuthToken(string username, string password)
    {
        return Convert
            .ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{username}:{password}"));
    }
}
