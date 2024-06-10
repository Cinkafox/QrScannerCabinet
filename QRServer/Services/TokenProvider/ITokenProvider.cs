namespace QRServer.Services.TokenProvider;

public interface ITokenProvider
{
    public Guid Add(string key);
    public bool TryGet(string key, out Guid guid);
    public Guid? Get(string key);
    public bool TryGet(Guid guid, out string key);
    public string? Get(Guid guid);
}