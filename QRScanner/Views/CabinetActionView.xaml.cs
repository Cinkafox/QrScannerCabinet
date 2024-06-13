using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRScanner.BottomSheets;
using QRScanner.Services;
using QRShared.Datum;

namespace QRScanner.Views;

public partial class CabinetActionView : ContentView
{
    private readonly RestService _restService;
    private readonly AuthService _authService;
    private readonly CabinetInfoService _cabinetInfoService;
    private readonly Uri _uri;
    
    public CancellationToken CancellationToken = CancellationToken.None;

    public CabinetActionView(RestService restService,AuthService authService, CabinetInfoService cabinetInfoService)
    {
        InitializeComponent();
        _restService = restService;
        _authService = authService;
        _cabinetInfoService = cabinetInfoService;
        _uri = _authService.CurrentUri!;
        _cabinetInfoService.CurrentUri = _uri;
        
        LoadCabinetInfo();
    }

    public async void LoadCabinetInfo()
    {
        if(!await EnsureAuth())
            return;
        
        var roomList = await _restService.GetAsync<List<RoomInformation>>(new Uri(_uri,"/RoomInformation/List"), CancellationToken);
        
        CabinetStack.Children.Clear();
        
        if(roomList.Value is null)
            return;

        foreach (var room in roomList.Value)
        {
            var cab = new SelectiveCabinetView();
            cab.LoadFromCabinetInfo(room);
            cab.ActionClicked += EditClicked;
            cab.ActionName = "Ред";
            CabinetStack.Children.Add(cab);
        }
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
        if(!await EnsureAuth())
            return;
        
        MainLayout.IsVisible = false;
        var cabinetEdit = new CabinetEditView();

        if (id != null)
        {
            var cabinetInfo = await _cabinetInfoService.Get(id.Value, CancellationToken);
            if (cabinetInfo != null) cabinetEdit.LoadFromResult(cabinetInfo.Value);
        }
        cabinetEdit.OnResult += OnResult;
        AdditionView.Content = cabinetEdit;
    }

    private async void OnResult(ResultCabinet obj)
    {
        AdditionView.Content = null;
        MainLayout.IsVisible = true;
        await _cabinetInfoService.Push(obj,CancellationToken);
    }

    private async Task<bool> EnsureAuth()
    {
        await _authService.CheckAuth(CancellationToken);
        if (_authService.Reason is not null)
            await MainThread.InvokeOnMainThreadAsync(() => Message.Text = _authService.Reason);
        return _authService.IsSuccessful;
    }
}