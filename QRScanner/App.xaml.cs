using QRScanner.Services;

namespace QRScanner;

public partial class App : Application
{
    public App(RestService service)
    {
        InitializeComponent();

        MainPage = new MainPage(service);
    }
}