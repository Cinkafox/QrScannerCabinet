using System.Diagnostics.CodeAnalysis;

namespace QRScanner.Utils;

public static class Dumper
{
    public static string DumpPath = Path.Combine(FileSystem.CacheDirectory, "crash.txt");

    public static void Dump(string? message)
    {
        using var stream =
            new FileStream(DumpPath, FileMode.Create);
        using var sw = new StreamWriter(stream);
        sw.WriteLine("FUCK!!! " + message);
        sw.Flush();
    }

    public static bool TryReadDump([NotNullWhen(true)] out DumpReader? streamReader)
    {
        if (!File.Exists(DumpPath))
        {
            streamReader = null;
            return false;
        }

        streamReader = new DumpReader();
        return true;
    }
}

public class DumpReader : IDisposable
{
    public readonly StreamReader StreamReader = new(new FileStream(Dumper.DumpPath, FileMode.OpenOrCreate));

    public void Dispose()
    {
        StreamReader.Dispose();
        File.Delete(Dumper.DumpPath);
    }

    public string? ReadLine()
    {
        return StreamReader.ReadLine();
    }
}