using Microsoft.AspNetCore.Mvc;
using QRDataBase.Filter;
using QRDataBase.Providers;
using QRServer.Auth;
using QRShared;

namespace QRServer.Controllers.Room;

[ApiController]
[Route("[controller]")]
public class RoomInformationController : ControllerBase
{
    private readonly IDataBaseProvider _provider;
    private readonly AuthManager _authManager;

    public RoomInformationController(IDataBaseProvider provider, AuthManager authManager)
    {
        _provider = provider;
        _authManager = authManager;
    }
    
    [HttpGet("{id:long}",Name = "GetRoomInformation")]
    public RoomInformation Get(long id)
    {
        return _provider.Get<RoomInformation>(
            new DbKeyValue("Id",id.ToString())
            )[0];
    }
    
    [HttpPost(Name = "AddRoomInformation")]
    public bool Post(RoomInformation room,string token)
    {
        if (!_authManager.HasAuthed(token))
        {
            Response.StatusCode = 401;
            return false;
        }
        
        _provider.Push(room);
        return true;
    }
    
    [HttpGet("{id:long}/Images",Name = "GetRoomImageInformation")]
    public List<RoomImageInformation> GetImage(long id)
    {
        return _provider.Get<RoomImageInformation>(
            new DbKeyValue(nameof(RoomImageInformation.RoomId),id.ToString())
        );
    }
    
    [HttpPost("Images",Name = "AddRoomImageInformation")]
    public bool PostImage(RoomImageInformation room,string token)
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