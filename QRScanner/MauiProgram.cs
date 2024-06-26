﻿using Microsoft.Extensions.Logging;
using The49.Maui.BottomSheet;
using ZXing.Net.Maui.Controls;

namespace QRScanner;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .UseBottomSheet()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        Dependencies.Register(builder);
        Dependencies.RegisterViews(builder);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}