using CommandLine;

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
