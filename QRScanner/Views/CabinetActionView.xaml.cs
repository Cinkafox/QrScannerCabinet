using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly DebugService _debugService;
    private readonly Uri _uri;
    
    public CancellationToken CancellationToken = CancellationToken.None;

    public CabinetActionView(RestService restService,AuthService authService, 
        CabinetInfoService cabinetInfoService,IServiceProvider serviceProvider,
        DebugService debugService)
    {
        InitializeComponent();
        _restService = restService;
        _authService = authService;
        _cabinetInfoService = cabinetInfoService;
        _serviceProvider = serviceProvider;
        _debugService = debugService;
        _uri = _authService.CurrentUri!;
        _cabinetInfoService.CurrentUri = _uri;

        MainThread.InvokeOnMainThreadAsync(LoadCabinetInfo);
    }

    public async Task LoadCabinetInfo()
    {
        CabinetStack.Children.Add(new Label()
        {
            Text = "Загрузка"
        });
        
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
        
        var cabinetEdit = _serviceProvider.GetService<CabinetEditView>()!;

        if (id != null)
        {
            var cabinetInfo = await _cabinetInfoService.Get(id.Value, CancellationToken);
            if (cabinetInfo != null) cabinetEdit.LoadFromResult(cabinetInfo.Value);
        }
        cabinetEdit.OnResult += OnResult;
        
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            AdditionView.Content = cabinetEdit;
            MainLayout.IsVisible = false;
        });
    }

    private async void OnResult(RoomInformation roomInformation, List<ImageInfoCompound> list)
    {
        if(!await EnsureAuth())
            return;
        
        _debugService.Debug("START SOME PUSH");

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            AdditionView.Content = null;
            MainLayout.IsVisible = true;
        });
        
        var result = await _restService.PostAsync<RawResult, RoomInformation>(roomInformation, 
            new Uri(_authService.CurrentUri!, $"/RoomInformation?overrideValue=true"),
            CancellationToken,_authService.Token);
        
        if (result.StatusCode != HttpStatusCode.OK)
        {
            _debugService.Error(result.StatusCode+" ROOM POST ERROR:"+result.Value?.Result);
            return;
        }

        foreach (var imageInfoCompound in list)
        {
            if(imageInfoCompound.IsEqual) continue;

            var overrideRequired = "?overrideValue=true";
            if (!imageInfoCompound.ForcePush) overrideRequired = "";

            var image = imageInfoCompound.Result;
            
            var resultImage = await _restService.PostAsync<RawResult, RoomImageInformation>(image,
                new Uri(_authService.CurrentUri!, "/RoomInformation/Images" + overrideRequired), CancellationToken, _authService.Token);
            
            if (resultImage.StatusCode != HttpStatusCode.OK)
            {
                _debugService.Error(resultImage.StatusCode+" IMAGE POST ERROR:"+result.Value?.Result);
                continue;
            }
        }
        
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