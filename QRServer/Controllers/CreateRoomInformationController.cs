using Microsoft.AspNetCore.Mvc;
using QRDataBase;
using QRDataBase.Providers;
using QRShared;

namespace QRServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateRoomInformationController : ControllerBase
{
    private IDataBaseProvider _provider;
    public CreateRoomInformationController(IDataBaseManager dbManager)
    {
        _provider = dbManager.GetProvider();
    }
    
    [HttpPost(Name = "AddRoomInformation")]
    public bool Post(RoomInformation roomInformation)
    {
        _provider.CreateRoom(new ()
        {
            Id = roomInformation.Id,
            Name = roomInformation.Name
        });
        return true;
    }
}