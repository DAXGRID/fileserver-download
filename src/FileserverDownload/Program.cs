﻿using CommandLine;

namespace FileServerDownload;

public record CommandLineOptions
{
    [Option('r', "resource", Required = true, HelpText = "Fileserver resource path.")]
    public string ResourcePath { get; init; } = "";

    [Option('u', "username", Required = true, HelpText = "Fileserver username.")]
    public string Username { get; init; } = "";

    [Option('p', "password", Required = true, HelpText = "Fileserver password.")]
    public string Password { get; init; } = "";

    [Option('f', "filename", Required = true, HelpText = "Fileserver filename prefix.")]
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
                Console.WriteLine($"Url is: '{o.ResourcePath}'");
                Console.WriteLine($"Username is: '{o.Username}'");
                Console.WriteLine($"Password is: '{o.Password}'");
                Console.WriteLine($"File name is: '{o.FileNamePrefix}'");
            });
    }
}
