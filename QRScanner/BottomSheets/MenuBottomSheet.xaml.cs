using QRScanner.Utils;
using QRScanner.Views;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class MenuBottomSheet : BottomSheet, ICancellationBehaviour
{
    private readonly MainPage _mainPage;
    private readonly IServiceProvider _serviceProvider;

    public MenuBottomSheet(MainPage mainPage, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _mainPage = mainPage;
        _serviceProvider = serviceProvider;

        foreach (var resultCabinet in mainPage.History)
        {
            var cabView = new SelectiveCabinetView();
            cabView.LoadFromCabinetInfo(resultCabinet.Value.Information);
            
            var actionButton = new Button();
            actionButton.Text = "Информация";
            actionButton.Clicked += (_,_)=>
                ActionClicked(resultCabinet.Value.Information.Id);
            
            cabView.AddButton(actionButton);
            History.Add(cabView);
        }

        DevSwitch.IsToggled = mainPage.DevEnabled;
        DevSwitch.Toggled += DevSwitchOnToggled;

        Dismissed += OnDismissed;
    }

    public CancellationToken CancellationToken { get; set; }

    private async void ActionClicked(long obj)
    {
        await SwitchBottomSheet(new ResultBottomSheet(_mainPage.History[obj]));
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
        await SwitchBottomSheet(_serviceProvider.GetService<CabinetManagementBottomSheet>()!);
    }

    private async Task SwitchBottomSheet(BottomSheet bottomSheet)
    {
        await _mainPage.BottomSheetCollection.ShowBottomSheet(bottomSheet);
        await DismissAsync();
    }
}