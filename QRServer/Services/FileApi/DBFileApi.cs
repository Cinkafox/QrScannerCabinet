using System.Diagnostics.CodeAnalysis;
using QRDataBase.Filter;
using QRDataBase.Providers;
using QRServer.Utils;
using QRShared.DataBase.Attributes;

namespace QRServer.Services.FileApi;

public class DBFileApi : IFileApi
{
    private readonly IDataBaseProvider _dataBaseProvider;

    public DBFileApi(IDataBaseProvider dataBaseProvider)
    {
        _dataBaseProvider = dataBaseProvider;
    }
    
    public Stream Open(string path)
    {
        path = path.Replace("\\", "-");
        if (TryOpen(path, out var stream)) return stream;
        return Stream.Null;
    }

    public bool TryOpen(string path,[NotNullWhen(true)] out Stream? stream)
    {
        path = path.Replace("\\", "-");
        var list = _dataBaseProvider.Get<FileDB>(new DbKeyValue(nameof(FileDB.Path),path));
        if (list.Count > 0)
        {
           stream = new MemoryStream(list[0].Data);
           return true;
        }

        stream = null;
        return false;
    }

    public void Save(Stream stream, string path)
    {
        path = path.Replace("\\", "-");
        _dataBaseProvider.Push(new FileDB
        {
            Path = path,
            Length = stream.Length,
            Data = StreamUtil.ReadFully(stream)
        });
    }
    
}

public class FileDB
{
    [PrimaryKey]
    public string Path { get; set; } = string.Empty;
    public long Length { get; set; }
    public byte[] Data { get; set; } = [];
}