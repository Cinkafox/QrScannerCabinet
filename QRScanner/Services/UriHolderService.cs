namespace QRScanner.Services;

public class UriHolderService
{
    private readonly DebugService _debugService;

    public UriHolderService(DebugService debugService)
    {
        _debugService = debugService;
    }

    public Uri? CurrentUri { get; set; }
    public Uri ImagePostUri => new(CurrentUri, "/Image");
    public Uri RoomListUri => new(CurrentUri, "/RoomInformation/List");
    public Uri AuthUri => new(CurrentUri, "/Auth");
    public Uri RoomPostUri => new(CurrentUri!, "/RoomInformation?overrideValue=true");

    public Uri GetRoomImageUri(Uri uri, long id)
    {
        return new Uri(uri, $"/RoomInformation/{id}/Images");
    }

    public Uri GetRoomUri(long id)
    {
        return new Uri(CurrentUri, $"/RoomInformation/{id}");
    }

    public Uri GetQrCodeUri(long id)
    {
        return new Uri(CurrentUri, $"/QRCode/{id}");
    }

    public bool EnsureUri()
    {
        if (CurrentUri is not null) return true;
        _debugService.Error("Uri is not set!");
        return false;
    }
}