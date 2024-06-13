using System.Diagnostics.CodeAnalysis;

namespace QRServer.Services.FileApi;

public interface IFileApi
{
    public Stream Open(string path);
    public bool TryOpen(string path, [NotNullWhen(true)] out Stream? stream);
    public void Save(Stream stream, string path);
}