using QRScanner.BottomSheets;
using QRScanner.Services;
using QRScanner.Views;

namespace QRScanner;

public static class Dependencies
{
    public static void Register(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<RestService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<DebugService>();
        builder.Services.AddSingleton<CabinetInfoService>();
        builder.Services.AddSingleton<UriHolderService>();
    }

    public static void RegisterViews(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<MenuBottomSheet>();
        builder.Services.AddTransient<DebugBottomSheet>();
        builder.Services.AddTransient<CabinetManagementBottomSheet>();
        builder.Services.AddTransient<ServerUrlInputView>();
        builder.Services.AddTransient<CabinetListView>();
        builder.Services.AddTransient<AuthView>();
        builder.Services.AddTransient<CabinetEditView>();
        builder.Services.AddTransient<ImageCabinetView>();
    }
}