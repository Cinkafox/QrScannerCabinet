using QRScanner.Utils;
using QRScanner.Views;
using QRShared.Datum;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class MenuBottomSheet : BottomSheet, ICancellationBehaviour
{
    private readonly MainPage _mainPage;
    private readonly IServiceProvider _serviceProvider;
    public CancellationToken CancellationToken { get; set; }

    public MenuBottomSheet(MainPage mainPage, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        
        _mainPage = mainPage;
        _serviceProvider = serviceProvider;
        
        foreach (var resultCabinet in mainPage.History)
        {
            var cabView = new SelectiveCabinetView();
            cabView.ActionName = "I";
            cabView.LoadFromCabinetInfo(resultCabinet.Information);
            cabView.ActionClicked += ActionClicked;
            History.Add(cabView);
        }

        DevSwitch.IsToggled = mainPage.DevEnabled;
        DevSwitch.Toggled += DevSwitchOnToggled;
        
        Dismissed += OnDismissed;
    }

    private async void ActionClicked(long obj, RoomInformation? roomInformation)
    {
        await SwitchBottomSheet(new ResultBottomSheet(_mainPage.History.First(a => a.Information.Id==obj)));
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

    private async void AddCabinetButtonClicked(object? sender, EventArgs e)
    {
        await SwitchBottomSheet(_serviceProvider.GetService<AddCabinetBottomSheet>()!);
    }

    private async Task SwitchBottomSheet(BottomSheet bottomSheet)
    {
        await _mainPage.BottomSheetCollection.ShowBottomSheet(bottomSheet);
        await DismissAsync();
    }
}