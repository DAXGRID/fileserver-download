using CommandLine;

namespace FileServerDownload;

public record CommandLineOptions
{
    [Option('u', "username", Required = true, HelpText = "Fileserver username.")]
    public string Username { get; init; } = "";

    [Option('p', "password", Required = true, HelpText = "Fileserver password.")]
    public string Password { get; init; } = "";

    [Option('f', "fileNamePrefix", Required = true, HelpText = "Fileserver filename prefix.")]
    public string FileNamePrefix { get; init; } = "";
}

internal static class Program
{
    public static void Main(string[] args)
    {
        Parser.Default
            .ParseArguments<CommandLineOptions>(args)
            .WithParsed<CommandLineOptions>(o =>
            {
                Console.WriteLine($"Username is: '{o.Username}'");
                Console.WriteLine($"Password is: '{o.Password}'");
                Console.WriteLine($"File name is: '{o.FileNamePrefix}'");
            });
    }
}
