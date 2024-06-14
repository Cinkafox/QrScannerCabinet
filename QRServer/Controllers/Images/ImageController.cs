using Microsoft.AspNetCore.Mvc;
using QRServer.Services;
using QRServer.Services.FileApi;

namespace QRServer.Controllers.Images;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IFileApi _fileApi;
    private readonly string _imagePath = "Images";

    public ImageController(IFileApi fileApi, AuthService authService)
    {
        _fileApi = fileApi;
        _authService = authService;
    }

    [HttpGet("{path}")]
    public IActionResult Get(string path)
    {
        if (!Guid.TryParse(path, out _))
            return new StatusCodeResult(403);
        ;

        if (_fileApi.TryOpen(Path.Join(_imagePath, path), out var stream)) return File(stream, "image/jpeg");

        return NotFound();
    }

    [HttpPost]
    public IActionResult Post(IFormFile formFile, string token)
    {
        if (!_authService.HasAuthed(token))
            return Unauthorized();

        var fileName = Guid.NewGuid().ToString();

        using (var stream = formFile.OpenReadStream())
        {
            _fileApi.Save(stream, Path.Join(_imagePath, fileName));
        }

        var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}/{fileName}");

        return Ok(location);
    }
}