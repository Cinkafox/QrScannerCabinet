using Microsoft.AspNetCore.Mvc;
using QRDataBase;
using QRDataBase.Providers;
using QRShared;

namespace QRServer.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomInformationController : ControllerBase
{
    private IDataBaseProvider _provider;

    public RoomInformationController(IDataBaseManager dbManager)
    {
        _provider = dbManager.GetProvider();
    }
    
    [HttpGet("{id:int}",Name = "GetRoomInformation")]
    public RoomInformation Get(int id)
    {
       var room = _provider.GetRoomById(id);
       if (room == null)
           return NullInformation.Information;
       
       return new RoomInformation()
       {
           Id = room.Id,
           Name = room.Name
       };
    }
}