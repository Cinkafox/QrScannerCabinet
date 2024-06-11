using System.Diagnostics.CodeAnalysis;

namespace QRServer.Services.FileApi;

public class TempFileApi : IFileApi
{
    public Dictionary<string, Stream> Streams = new();
    
    public Stream Open(string path)
    {
        return TryOpen(path, out var stream) ? stream : Stream.Null;
    }

    public bool TryOpen(string path,[NotNullWhen(true)] out Stream? stream)
    {
        if (Streams.TryGetValue(path, out var oristream))
        {
            oristream.Seek(0, SeekOrigin.Begin);
            stream = new MemoryStream();
            oristream.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return true;
        }

        stream = null;
        return false;
    }

    public void Save(Stream stream, string path)
    {
        var st = new MemoryStream();
        stream.CopyTo(st);
        Streams[path] = st;
    }
}