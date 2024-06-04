using QRScanner.Views;
using The49.Maui.BottomSheet;
using AndroidUri = Android.Net.Uri;

namespace QRScanner.BottomSheets;

public partial class MenuBottomSheet : BottomSheet
{
    private readonly MainPage _mainPage;

    public MenuBottomSheet(MainPage mainPage)
    {
        _mainPage = mainPage;
        InitializeComponent();

        foreach (var resultCabinet in mainPage.History)
        {
            History.Add(new CabinetMiniView(resultCabinet));
        }

        DevSwitch.IsToggled = mainPage.DevEnabled;
        DevSwitch.Toggled += DevSwitchOnToggled;
        
        Dismissed += OnDismissed;
    }

    private void DevSwitchOnToggled(object? sender, ToggledEventArgs e)
    {
        _mainPage.DevEnabled = e.Value;
    }

    private void OnDismissed(object? sender, DismissOrigin e)
    {
        DevSwitch.Toggled -= DevSwitchOnToggled;
        Dismissed -= OnDismissed;
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            Dumper.DumpShit("TEST!");
            _mainPage._debug.Debug("SHIT IS DUMPED! " + Dumper.DumpPath);
        }
        catch (Exception exception)
        {
            _mainPage._debug.Error(exception.Message);
        }
    }
}