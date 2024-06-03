using QRDataBase.Providers;
using QRServer.Auth;

namespace QRServer;

public static class Dependencies
{
    public static void Register(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDataBaseProvider,MySqlDBProvider>();
        builder.Services.AddSingleton<IAuthDataProvider,DataBaseAuthProvider>();
        builder.Services.AddSingleton<AuthManager>();
    }
}