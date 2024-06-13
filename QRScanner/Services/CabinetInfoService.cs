using System.Net;
using QRScanner.BottomSheets;
using QRShared.Datum;

namespace QRScanner.Services;

public class CabinetInfoService
{
    private readonly RestService _restService;
    private readonly DebugService _debugService;
    private readonly AuthService _authService;
    public Uri? CurrentUri { get; set; }

    public CabinetInfoService(RestService restService,DebugService debugService,AuthService authService)
    {
        _restService = restService;
        _debugService = debugService;
        _authService = authService;
    }

    public async Task<ResultCabinet?> Get(Uri uri,CancellationToken cancellationToken)
    {
        var information = await _restService.GetAsync<RoomInformation>(uri,cancellationToken);
        if (information.Value == null) return null;
        
        var imageInformation = await _restService.GetAsyncDefault<List<RoomImageInformation>>(
            new Uri(uri,$"/RoomInformation/{information.Value.Id}/Images"),[],cancellationToken);
        return new ResultCabinet(information.Value, imageInformation);
    }

    public async Task<ResultCabinet?> Get(long id,CancellationToken cancellationToken)
    {
        if (CurrentUri is null)
        {
            _debugService.Error("Current URI is empty!");
            return null;
        }
        
        return await Get(new Uri(CurrentUri, $"/RoomInformation/{id}"), cancellationToken);
    }

    public async Task Push(ResultCabinet resultCabinet,CancellationToken cancellationToken)
    {
        if (CurrentUri is null)
        {
            _debugService.Error("Current URI is empty!");
            return;
        }

        if (_authService.IsAuthRequired)
        {
            _debugService.Error("Auth first!");
            return;
        }

        var room = resultCabinet.Information;
        var result = await _restService.PostAsync<RawResult, RoomInformation>(room, new Uri(CurrentUri, $"/RoomInformation?overrideValue=true"),
            cancellationToken,_authService.Token);
        
        if (result.StatusCode != HttpStatusCode.OK)
        {
            _debugService.Error("ROOM POST ERROR:"+result.Value?.Result);
            return;
        }

        var images = resultCabinet.ImageInformation;

        foreach (var image in images)
        {
            await _restService.PostAsync<RawResult, RoomImageInformation>(image,
                new Uri(CurrentUri, "/RoomInformation/Images?overrideValue=true"), cancellationToken, _authService.Token);
        }
        
    }
    
}