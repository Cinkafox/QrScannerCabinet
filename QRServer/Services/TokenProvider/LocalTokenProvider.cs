namespace QRServer.Services.TokenProvider;

public class LocalTokenProvider : ITokenProvider
{
    private readonly Dictionary<Guid, DateTime> _lifeList = new();
    private readonly Dictionary<Guid, string> _keyList = new();
    private readonly Dictionary<string, Guid> _guidList = new();
    private static readonly TimeSpan TokenLifeTime = new TimeSpan(0,0,30,0);


    public Guid Add(string key)
    {
        if (_guidList.TryGetValue(key, out var guid)) 
            return guid;
        
        guid = Guid.NewGuid();
        _lifeList.Add(guid,DateTime.Now + TokenLifeTime);
        _keyList.Add(guid,key);
        _guidList.Add(key,guid);
        return guid;
    }

    public bool TryGet(string key, out Guid guid)
    {
        return _guidList.TryGetValue(key, out guid) && EnsureData(guid);
    }

    public Guid? Get(string key)
    {
        if (TryGet(key, out var guid)) return guid;
        return null;
    }

    public bool TryGet(Guid guid, out string key)
    {
        if (EnsureData(guid) && _keyList.TryGetValue(guid, out key!)) return true;
        key = string.Empty;
        return false;
    }

    public string? Get(Guid guid)
    {
        if (TryGet(guid, out var key)) return key;
        return null;
    }

    private bool EnsureData(Guid guid)
    {
        if (_lifeList.TryGetValue(guid, out var time) && time > DateTime.Now) return true;

        _lifeList.Remove(guid);
        if (_keyList.TryGetValue(guid, out var key))
        {
            _guidList.Remove(key);
            _keyList.Remove(guid);
        }

        return false;
    }
}