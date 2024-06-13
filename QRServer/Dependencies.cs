using QRDataBase;
using QRDataBase.Providers;
using QRServer.Services;
using QRServer.Services.AuthProvider;
using QRServer.Services.FileApi;
using QRServer.Services.TokenProvider;

namespace QRServer;

public static class Dependencies
{
    public static void Register(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ITokenProvider, LocalTokenProvider>();
        builder.Services.AddSingleton<IDataBaseProvider, MySqlDBProvider>();
        builder.Services.AddSingleton<IFileApi, DBFileApi>();
        builder.Services.AddSingleton<IAuthDataProvider, DataBaseAuthProvider>();
        builder.Services.AddSingleton<AuthService>();
    }

    public static void ConnectDataBase(WebApplication app)
    {
        var configuration = app.Services.GetService<IConfiguration>()!;
        var option = configuration.GetSection("DataBase").Get<DataBaseOption>()!;
        app.Services.GetService<IDataBaseProvider>()?.Connect(option);
    }
}