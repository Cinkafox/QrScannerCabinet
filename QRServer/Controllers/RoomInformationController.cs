using Microsoft.AspNetCore.Mvc;
using QRShared;

namespace QRServer.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomInformationController : ControllerBase
{
    
    [HttpGet(Name = "GetRoomInformation")]
    public RoomInformation Get()
    {
        return new RoomInformation
        {
            Name = "Meow",
            Id = 12
        };
    }
}