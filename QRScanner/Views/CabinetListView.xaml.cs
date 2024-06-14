using Android.Widget;
using QRScanner.Services;
using QRShared.Datum;
using Button = Microsoft.Maui.Controls.Button;

namespace QRScanner.Views;

public partial class CabinetListView : ContentView
{
    private readonly AuthService _authService;
    private readonly CabinetInfoService _cabinetInfoService;
    private readonly DebugService _debugService;
    private readonly RestService _restService;
    private readonly IServiceProvider _serviceProvider;
    private readonly UriHolderService _uriHolderService;

    public CancellationToken CancellationToken = CancellationToken.None;

    public Action? OnAuthError;

    public CabinetListView(RestService restService, AuthService authService,
        CabinetInfoService cabinetInfoService, IServiceProvider serviceProvider,
        DebugService debugService, UriHolderService uriHolderService)
    {
        InitializeComponent();
        _restService = restService;
        _authService = authService;
        _cabinetInfoService = cabinetInfoService;
        _serviceProvider = serviceProvider;
        _debugService = debugService;
        _uriHolderService = uriHolderService;

        MainThread.InvokeOnMainThreadAsync(LoadCabinetInfo);
    }

    public async Task LoadCabinetInfo()
    {
        CabinetStack.Children.Add(new Label
        {
            Text = "Загрузка"
        });

        if (!await EnsureAuth())
            return;

        var roomList =
            await _restService.GetAsync<List<RoomInformation>>(_uriHolderService.RoomListUri, CancellationToken);

        CabinetStack.Children.Clear();

        if (roomList.Value is null)
            return;

        foreach (var room in roomList.Value)
        {
            var cab = new SelectiveCabinetView();
            cab.LoadFromCabinetInfo(room);
            
            var actionButton = new Button();
            actionButton.Text = "Редактировать";
            actionButton.Clicked += (_, _) => EditClicked(room.Id);

            var removeButton = new Button();
            removeButton.Text = "Удалить";
            removeButton.Clicked += (_, _) => CabinetRemoveRequired(cab);

            var getQRButton = new Button();
            getQRButton.Text = "QR code";
            getQRButton.Clicked += (_, _) => QrCodeRequired(room.Id);
            
            cab.AddButton(actionButton);
            cab.AddButton(removeButton);
            cab.AddButton(getQRButton);
            
            CabinetStack.Children.Add(cab);
        }
    }

    private async void QrCodeRequired(long id)
    {
        await Clipboard.Default.SetTextAsync(_uriHolderService.GetQrCodeUri(id).ToString());
        _debugService.Toast("Ссылка скопирована в буфер обмена");
    }

    private async void CabinetRemoveRequired(SelectiveCabinetView obj)
    {
        if (!await EnsureAuth())
            return;
        
        await _restService.DeleteAsync<RawResult>(_uriHolderService.GetRoomUri(obj.CabinetId), CancellationToken,
            _authService.Token);
        CabinetStack.Children.Remove(obj);
    }

    private async void EditClicked(long id)
    {
        await EditCabinet(id);
    }

    private async void AddButtonClicked(object? sender, EventArgs e)
    {
        await EditCabinet(null);
    }

    private async Task EditCabinet(long? id)
    {
        if (!await EnsureAuth())
            return;

        var cabinetEdit = _serviceProvider.GetService<CabinetEditView>()!;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            AdditionView.Content = cabinetEdit;
            AdditionView.IsVisible = true;
            MainLayout.IsVisible = false;
        });
        
        if (id != null)
        {
            var cabinetInfo = await _cabinetInfoService.Get(id.Value, CancellationToken);
            if (cabinetInfo != null) cabinetEdit.LoadFromResult(cabinetInfo.Value);
        }
        else
        {
            cabinetEdit.LoadEmpty();
        }

        cabinetEdit.OnResult += OnResult;
        cabinetEdit.CancellationToken = CancellationToken;
        
    }

    private async void OnResult()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            AdditionView.Content = null;
            AdditionView.IsVisible = false;
            MainLayout.IsVisible = true;
        });

        await LoadCabinetInfo();
    }

    private async Task<bool> EnsureAuth()
    {
        await _authService.CheckAuth(CancellationToken);
        if (_authService.Reason is not null)
            await MainThread.InvokeOnMainThreadAsync(() => Message.Text = _authService.Reason);
        
        if(!_authService.IsSuccessful)
            OnAuthError?.Invoke();
        
        return _authService.IsSuccessful;
    }
}