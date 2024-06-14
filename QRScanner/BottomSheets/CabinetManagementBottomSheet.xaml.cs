using QRScanner.Utils;
using QRScanner.Views;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class CabinetManagementBottomSheet : BottomSheet, ICancellationBehaviour
{
    private readonly IServiceProvider _serviceProvider;

    public CabinetManagementBottomSheet(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        var urlInput = _serviceProvider.GetService<ServerUrlInputView>()!;
        urlInput.CancellationToken = CancellationToken;
        urlInput.OnProceed += OnUrlProceed;
        ViewContainer.Content = urlInput;
    }

    public CancellationToken CancellationToken { get; set; }

    private void OnUrlProceed()
    {
        var auth = _serviceProvider.GetService<AuthView>()!;
        auth.OnProceed += OnAuthProceed;
        auth.CancellationToken = CancellationToken;
        ViewContainer.Content = auth;
    }

    private void OnAuthProceed()
    {
        var cabinetAct = _serviceProvider.GetService<CabinetListView>()!;
        cabinetAct.CancellationToken = CancellationToken;
        ViewContainer.Content = cabinetAct;
    }
}