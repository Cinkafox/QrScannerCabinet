using QRScanner.Services;
using QRScanner.Utils;
using QRScanner.Views;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class AddCabinetBottomSheet : BottomSheet, ICancellationBehaviour
{
    private readonly AuthService _authService;
    private readonly RestService _restService;
    private readonly IServiceProvider _serviceProvider;

    public AddCabinetBottomSheet(RestService restService, AuthService authService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _restService = restService;
        _authService = authService;
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
        var cabinetAct = _serviceProvider.GetService<CabinetActionView>()!;
        cabinetAct.CancellationToken = CancellationToken;
        ViewContainer.Content = cabinetAct;
    }
}