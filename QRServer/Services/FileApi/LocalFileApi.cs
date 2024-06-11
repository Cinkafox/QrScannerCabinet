using System.Diagnostics.CodeAnalysis;

namespace QRServer.Services.FileApi;

public class LocalFileApi : IFileApi
{
    public string RootPath = "bin/";
    public Stream Open(string path)
    {
        return File.OpenRead(Path.Combine(RootPath,path));
    }

    public bool TryOpen(string path,[NotNullWhen(true)] out Stream? stream)
    {
        if (File.Exists(Path.Combine(RootPath,path)))
        {
            stream = Open(path);
            return true;
        }
        
        stream = null;
        return false;
    }

    public void Save(Stream stream, string path)
    {
        using var fileStream = File.Open(Path.Combine(RootPath,path),FileMode.OpenOrCreate,FileAccess.Write);
        stream.CopyTo(fileStream);
        fileStream.Flush();
    }
}