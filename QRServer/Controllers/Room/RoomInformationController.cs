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
    public List<RoomInformation> Get(long id)
    {
        return _provider.Get<RoomInformation>(
            new DbKeyValue("Id",id.ToString())
            );
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
}