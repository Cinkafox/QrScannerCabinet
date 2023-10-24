using Microsoft.AspNetCore.Mvc;
using QRDataBase;
using QRDataBase.Providers;
using QRShared;

namespace QRServer.Controllers.Room;

[ApiController]
[Route("[controller]")]
public class CreateRoomInformationController : ControllerBase
{
    private readonly IDataBaseProvider _provider;
    public CreateRoomInformationController(IDataBaseManager dbManager)
    {
        _provider = dbManager.GetProvider();
    }
    
    [HttpPost(Name = "AddRoomInformation")]
    public bool Post(RoomInformation room)
    {
        _provider.CreateInformation<RoomInformation>(room);
        return true;
    }
}