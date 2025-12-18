// This utility copies KontoApi.Api.deps.json to testhost.deps.json for integration tests
using System;
using System.IO;

class CopyDepsJson
{
    static void Main(string[] args)
    {
        var source = Path.Combine("..", "..", "src", "Api", "bin", "Debug", "net10.0", "KontoApi.Api.deps.json");
        var dest = Path.Combine("..", "KontoApi.Tests", "bin", "Debug", "net10.0", "testhost.deps.json");
        if (!File.Exists(source))
        {
            Console.Error.WriteLine($"Source not found: {source}");
            Environment.Exit(1);
        }
        File.Copy(source, dest, true);
        Console.WriteLine($"Copied {source} to {dest}");
    }
}
