using System.Net;
using QRScanner.BottomSheets;
using QRShared.Datum;

namespace QRScanner.Services;

public class CabinetInfoService
{
    private readonly RestService _restService;
    private readonly DebugService _debugService;
    private readonly AuthService _authService;
    private readonly UriHolderService _uriHolderService;

    public CabinetInfoService(RestService restService,DebugService debugService,AuthService authService,UriHolderService uriHolderService)
    {
        _restService = restService;
        _debugService = debugService;
        _authService = authService;
        _uriHolderService = uriHolderService;
    }

    public async Task<ResultCabinet?> Get(Uri uri,CancellationToken cancellationToken)
    {
        var information = await _restService.GetAsync<RoomInformation>(uri,cancellationToken);
        if (information.Value == null) return null;
        
        var imageInformation = await _restService.GetAsyncDefault<List<RoomImageInformation>>(
            _uriHolderService.GetRoomImageUri(uri,information.Value.Id),[],cancellationToken);
        return new ResultCabinet(information.Value, imageInformation);
    }

    public async Task<ResultCabinet?> Get(long id,CancellationToken cancellationToken)
    {
        if (!_uriHolderService.EnsureUri())
            return null;
        
        return await Get(_uriHolderService.GetRoomUri(id), cancellationToken);
    }
}