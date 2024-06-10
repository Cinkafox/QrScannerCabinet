using QRDataBase.Filter;
using QRDataBase.Providers;
using QRShared.Datum;

namespace QRServer.Services.TokenProvider;

public class DataBaseTokenProvider : ITokenProvider
{
    
    private readonly IDataBaseProvider _provider;

    public DataBaseTokenProvider(IDataBaseProvider provider)
    {
        _provider = provider;
    }
    
    public Guid Add(string key)
    {
        var guid = Guid.NewGuid();
        _provider.Push(new TokenInformation()
        {
            Key = key,
            Token = guid
        },true);
        return guid;
    }

    public bool TryGet(string key, out Guid guid)
    {
        var t = _provider.Get<TokenInformation>(new DbKeyValue(nameof(TokenInformation.Key), key));
        if (t.Count > 0)
        {
            guid = t[0].Token;
            return true;
        }

        guid = default;
        return false;
    }

    public Guid? Get(string key)
    {
        if (TryGet(key, out var guid)) return guid;
        return null;
    }

    public bool TryGet(Guid guid, out string key)
    {
        var t = _provider.Get<TokenInformation>(new DbKeyValue(nameof(TokenInformation.Token), guid));
        if (t.Count > 0)
        {
            key = t[0].Key;
            return true;
        }

        key = string.Empty;
        return false;
    }

    public string? Get(Guid guid)
    {
        if (TryGet(guid, out var key)) return key;
        return null;
    }
}