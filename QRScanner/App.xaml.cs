using QRScanner.Services;
using QRScanner.Utils;

namespace QRScanner;

public partial class App : Application
{
    public App(RestService service,DebugService debug,AuthService auth)
    {
        InitializeComponent();
        MainPage = new MainPage(service, debug, auth);
        
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            Dumper.Dump(args.ExceptionObject.ToString());
        };
        
    }
}
