using QRScanner.Utils;
using QRScanner.Views;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class MenuBottomSheet : BottomSheet, ICancellationBehaviour
{
    private readonly MainPage _mainPage;
    public CancellationToken CancellationToken { get; set; }

    public MenuBottomSheet(MainPage mainPage)
    {
        _mainPage = mainPage;
        InitializeComponent();

        var hCount = 0;
        foreach (var resultCabinet in mainPage.History)
        {
            var cabView = new CabinetMiniView(resultCabinet);
            
            History.Add(cabView);
            hCount++;
        }

        DevSwitch.IsToggled = mainPage.DevEnabled;
        DevSwitch.Toggled += DevSwitchOnToggled;

        if (_mainPage.Auth.IsAuthRequired)
            MainThread.InvokeOnMainThreadAsync(CheckAuth);
        else
            PasteAuthDatum();
        
        Dismissed += OnDismissed;
    }

    private void PasteAuthDatum()
    { 
        AuthButton.IsVisible = _mainPage.Auth.IsAuthRequired;
        UserName.Text = _mainPage.Auth.Login ?? "Гость";
    }

    private async Task CheckAuth()
    {
        await _mainPage.Auth.CheckAuth(CancellationToken);
        await MainThread.InvokeOnMainThreadAsync(PasteAuthDatum);
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
    
    private async void AuthButtonClicked(object? sender, EventArgs e)
    {
        await DismissAsync();
        await _mainPage.BottomSheetCollection.ShowBottomSheet(new AuthBottomSheet(_mainPage.Auth));
        await _mainPage.BottomSheetCollection.ShowBottomSheet(new MenuBottomSheet(_mainPage));
    }
}