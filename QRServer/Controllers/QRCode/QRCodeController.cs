using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using QRCoder;

namespace QRServer.Controllers.QRCode;

[ApiController]
[Route("[controller]")]
public class QRCodeController : ControllerBase
{
    public readonly Dictionary<long, byte[]> ImageCache = new();
    
    [HttpGet("{roomId}")]
    public IActionResult Get(long roomId)
    {
        if (!ImageCache.TryGetValue(roomId, out var image))
        {
            var location = new Uri($"{Request.Scheme}://{Request.Host}/RoomInformation/{roomId}");
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(location.ToString(), QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            image = qrCode.GetGraphic(20);
            ImageCache.Add(roomId,image);
        }
        
        return new FileContentResult(image, new MediaTypeHeaderValue("image/png"));
    }
}