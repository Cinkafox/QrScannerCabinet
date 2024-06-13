using Microsoft.AspNetCore.Mvc;
using QRDataBase.Filter;
using QRDataBase.Providers;
using QRServer.Services;
using QRShared.Datum;

namespace QRServer.Controllers.Room;

[ApiController]
[Route("[controller]")]
public class RoomInformationController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IDataBaseProvider _provider;

    public RoomInformationController(IDataBaseProvider provider, AuthService authService)
    {
        _provider = provider;
        _authService = authService;
    }

    [HttpGet("List", Name = "RoomList")]
    public List<RoomInformation> Get()
    {
        return _provider.Get<RoomInformation>();
    }

    [HttpGet("{id:long}", Name = "GetRoomInformation")]
    public RoomInformation Get(long id)
    {
        var room = _provider.Get<RoomInformation>(new DbKeyValue("Id", id), 1);
        if (room.Count > 0) return room[0];
        Response.StatusCode = 404;

        return new RoomInformation
        {
            Id = -1,
            Name = "Error",
            Description = "Not found"
        };
    }

    [HttpPost(Name = "AddRoomInformation")]
    public bool Post(RoomInformation room, string token, bool overrideValue = false)
    {
        if (!_authService.HasAuthed(token))
        {
            Response.StatusCode = 401;
            return false;
        }

        _provider.Push(room, overrideValue);
        return true;
    }

    [HttpDelete("{id:long}", Name = "DeleteRoomInformation")]
    public bool Delete(long id, string token)
    {
        if (!_authService.HasAuthed(token))
        {
            Response.StatusCode = 401;
            return false;
        }

        _provider.Remove<RoomInformation>(new DbKeyValue("Id", id));
        return true;
    }

    [HttpGet("{id:long}/Images", Name = "GetRoomImageInformation")]
    public List<RoomImageInformation> GetImage(long id)
    {
        return _provider.Get<RoomImageInformation>(
            new DbKeyValue(nameof(RoomImageInformation.RoomId), id)
        );
    }

    [HttpPost("Images", Name = "AddRoomImageInformation")]
    public bool PostImage(RoomImageInformation room, string token, bool overrideValue = false)
    {
        if (!_authService.HasAuthed(token))
        {
            Response.StatusCode = 401;
            return false;
        }

        _provider.Push(room, overrideValue);
        return true;
    }

    [HttpDelete("Images/{id:long}", Name = "DeleteRoomImageInformation")]
    public bool DeleteImage(long id, string token)
    {
        if (!_authService.HasAuthed(token))
        {
            Response.StatusCode = 401;
            return false;
        }

        _provider.Remove<RoomImageInformation>(
            new DbKeyValue(nameof(RoomImageInformation.Id), id)
        );
        return true;
    }
}