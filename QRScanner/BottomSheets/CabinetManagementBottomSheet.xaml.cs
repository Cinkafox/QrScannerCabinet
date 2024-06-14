using QRScanner.Services;
using QRScanner.Utils;
using QRScanner.Views;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class CabinetManagementBottomSheet : BottomSheet, ICancellationBehaviour
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AuthService _authService;

    public CabinetManagementBottomSheet(IServiceProvider serviceProvider,AuthService authService)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _authService = authService;

       UrlInputRequire();
    }

    public CancellationToken CancellationToken { get; set; }

    private void UrlInputRequire()
    {
        var urlInput = _serviceProvider.GetService<ServerUrlInputView>()!;
        urlInput.CancellationToken = CancellationToken;
        urlInput.OnProceed += AuthRequire;
        ViewContainer.Content = urlInput;
    }

    private void AuthRequire()
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
        cabinetAct.OnAuthError += OnAuthError;
        ViewContainer.Content = cabinetAct;
    }

    private void OnAuthError()
    {
        if (_authService.IsAuthRequired)
            AuthRequire();
        else
            UrlInputRequire();
    }
}