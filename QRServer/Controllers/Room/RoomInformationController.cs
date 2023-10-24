using Microsoft.AspNetCore.Mvc;
using QRDataBase;
using QRDataBase.Providers;
using QRShared;

namespace QRServer.Controllers.Room;

[ApiController]
[Route("[controller]")]
public class RoomInformationController : ControllerBase
{
    private readonly IDataBaseProvider _provider;

    public RoomInformationController(IDataBaseManager dbManager)
    {
        _provider = dbManager.GetProvider();
    }
    
    [HttpGet("{id:long}",Name = "GetRoomInformation")]
    public List<RoomInformation> Get(long id)
    {
        return _provider.GetInformationById<RoomInformation>(id);
    }
}