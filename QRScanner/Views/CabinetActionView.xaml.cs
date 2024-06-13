using QRScanner.Services;
using QRShared.Datum;

namespace QRScanner.Views;

public partial class CabinetActionView : ContentView
{
    private readonly AuthService _authService;
    private readonly CabinetInfoService _cabinetInfoService;
    private readonly DebugService _debugService;
    private readonly RestService _restService;
    private readonly IServiceProvider _serviceProvider;
    private readonly UriHolderService _uriHolderService;

    public CancellationToken CancellationToken = CancellationToken.None;

    public CabinetActionView(RestService restService, AuthService authService,
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
            cab.ActionClicked += EditClicked;
            cab.ActionName = "Редактировать";
            cab.RemoveRequired = CabinetRemoveRequired;
            CabinetStack.Children.Add(cab);
        }
    }

    private async void CabinetRemoveRequired(SelectiveCabinetView obj)
    {
        await _restService.DeleteAsync<RawResult>(_uriHolderService.GetRoomUri(obj.CabinetId), CancellationToken,
            _authService.Token);
        CabinetStack.Children.Remove(obj);
    }

    private async void EditClicked(long id, RoomInformation? roomInformation)
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

        if (id != null)
        {
            var cabinetInfo = await _cabinetInfoService.Get(id.Value, CancellationToken);
            if (cabinetInfo != null) cabinetEdit.LoadFromResult(cabinetInfo.Value);
        }

        cabinetEdit.OnResult += OnResult;
        cabinetEdit.CancellationToken = CancellationToken;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            AdditionView.Content = cabinetEdit;
            AdditionView.IsVisible = true;
            MainLayout.IsVisible = false;
        });
    }

    private async void OnResult(RoomInformation roomInformation, List<ImageInfoCompound> list, List<long> removedList)
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
        return _authService.IsSuccessful;
    }
}