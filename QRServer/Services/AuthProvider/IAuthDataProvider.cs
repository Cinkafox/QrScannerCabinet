namespace QRServer.Services.AuthProvider;

public interface IAuthDataProvider
{
    bool TryGetPassword(string login, out string password);
    bool TryRegister(string login, string password);
}