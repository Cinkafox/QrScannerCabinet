using Microsoft.AspNetCore.Mvc;
using QRDataBase;
using QRDataBase.Providers;
using QRShared;

namespace QRServer.Controllers.RoomImage;

[ApiController]
[Route("[controller]")]
public class RoomImageInformationController : ControllerBase
{
    private readonly IDataBaseProvider _provider;

    public RoomImageInformationController(IDataBaseManager dbManager)
    {
        _provider = dbManager.GetProvider();
    }
    
    [HttpGet("{id:long}",Name = "GetRoomImageInformation")]
    public List<RoomImageInformation> Get(long id)
    {
        return _provider.GetInformationById<RoomImageInformation>(id,nameof(RoomImageInformation.RoomId));
    }
}