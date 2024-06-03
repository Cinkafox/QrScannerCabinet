using Microsoft.AspNetCore.Mvc;
using QRDataBase.Filter;
using QRDataBase.Providers;
using QRServer.Auth;
using QRShared;

namespace QRServer.Controllers.RoomImage;

[ApiController]
[Route("[controller]")]
public class RoomImageInformationController : ControllerBase
{
    private readonly IDataBaseProvider _provider;
    private readonly AuthManager _authManager;
    public RoomImageInformationController(IDataBaseProvider provider, AuthManager authManager)
    {
        _authManager = authManager;
        _provider = provider;
    }
    
    [HttpGet("{id:long}",Name = "GetRoomImageInformation")]
    public List<RoomImageInformation> Get(long id)
    {
        return _provider.Get<RoomImageInformation>(
            new DbKeyValue(nameof(RoomImageInformation.RoomId),id.ToString())
            );
    }
    
    [HttpPost(Name = "AddRoomImageInformation")]
    public bool Post(RoomImageInformation room,string token)
    {
        if (!_authManager.HasAuthed(token))
        {
            Response.StatusCode = 401;
            return false;
        }
        
        _provider.Push(room);
        return true;
    }
}