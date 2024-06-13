namespace QRScanner.Services;

public class UriHolderService
{
    private readonly DebugService _debugService;
    public Uri? CurrentUri { get; set; }
    public Uri ImagePostUri => new Uri(CurrentUri, "/Image");
    public Uri RoomListUri => new Uri(CurrentUri, "/RoomInformation/List");
    public Uri AuthUri => new Uri(CurrentUri, "/Auth");
    public Uri RoomPostUri => new Uri(CurrentUri!, $"/RoomInformation?overrideValue=true");

    public UriHolderService(DebugService debugService)
    {
        _debugService = debugService;
    }

    public Uri GetRoomImageUri(Uri uri,long id)
    {
       return new Uri(uri, $"/RoomInformation/{id}/Images");
    }

    public Uri GetRoomUri(long id)
    {
        return new Uri(CurrentUri, $"/RoomInformation/{id}");
    }

    public bool EnsureUri()
    {
        if (CurrentUri is not null) return true;
        _debugService.Error("Uri is not set!");
        return false;
    }

}