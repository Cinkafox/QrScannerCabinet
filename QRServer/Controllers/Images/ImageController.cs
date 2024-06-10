using Microsoft.AspNetCore.Mvc;

namespace QRServer.Controllers.Images;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly IFileApi _fileApi = new LocalFileApi();
    private readonly string _imagePath = "Images";
    
    [HttpGet("{path}")]
    public IActionResult Get(string path)
    {
        if (!Guid.TryParse(path, out _)) 
            return Forbid();
        
        if (_fileApi.TryOpen(Path.Join(_imagePath,path), out var stream))
        {
            return File(stream, "image/jpeg");
        }

        return NotFound();
    }

    [HttpPost]
    public IActionResult Post(IFormFile formFile)
    {
        if (formFile.ContentType != "image/jpeg") 
            return Forbid();

        var fileName = Guid.NewGuid().ToString();
        
        using (var stream = formFile.OpenReadStream())
        {
            _fileApi.Save(stream,Path.Join(_imagePath,fileName));   
        }
        var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}/{fileName}");  

        return Ok(location);
    }
}

public interface IFileApi
{
    public Stream Open(string path);
    public bool TryOpen(string path, out Stream stream);
    public void Save(Stream stream, string path);
}

public class LocalFileApi : IFileApi
{
    public string RootPath = "bin/";
    public Stream Open(string path)
    {
        return File.OpenRead(Path.Combine(RootPath,path));
    }

    public bool TryOpen(string path, out Stream stream)
    {
        if (File.Exists(Path.Combine(RootPath,path)))
        {
            stream = Open(path);
            return true;
        }
        
        stream = Stream.Null;
        return false;
    }

    public void Save(Stream stream, string path)
    {
        using var fileStream = File.Open(Path.Combine(RootPath,path),FileMode.OpenOrCreate,FileAccess.Write);
        stream.CopyTo(fileStream);
        fileStream.Flush();
    }
}