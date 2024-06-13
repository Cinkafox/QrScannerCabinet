namespace QRServer.Services.AuthProvider;

public class LocalAuthProvider : IAuthDataProvider
{
    public Dictionary<string, string> LocalSession = new();

    public bool TryGetPassword(string login, out string password)
    {
        return LocalSession.TryGetValue(login, out password!);
    }

    public bool TryRegister(string login, string password)
    {
        return LocalSession.TryAdd(login, password);
    }
}