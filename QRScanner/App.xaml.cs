using QRScanner.Utils;

namespace QRScanner;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        MainPage = serviceProvider.GetService<MainPage>();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            Dumper.Dump(args.ExceptionObject.ToString());
        };
    }
}