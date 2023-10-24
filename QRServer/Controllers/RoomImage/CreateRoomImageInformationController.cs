using Microsoft.AspNetCore.Mvc;
using QRDataBase;
using QRDataBase.Providers;
using QRShared;

namespace QRServer.Controllers.RoomImage;

[ApiController]
[Route("[controller]")]
public class CreateRoomImageInformationController : ControllerBase
{
    private readonly IDataBaseProvider _provider;
    public CreateRoomImageInformationController(IDataBaseManager dbManager)
    {
        _provider = dbManager.GetProvider();
    }
    
    [HttpPost(Name = "AddRoomImageInformation")]
    public bool Post(RoomImageInformation room)
    {
        _provider.CreateInformation<RoomImageInformation>(room);
        return true;
    }
}