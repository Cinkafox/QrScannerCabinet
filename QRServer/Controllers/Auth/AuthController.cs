using Microsoft.AspNetCore.Mvc;
using QRServer.Services;
using QRShared.Datum;

namespace QRServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public IActionResult GetToken(UserInformation userInformation)
    {
        if (_authService.TryAuth(userInformation.Login, userInformation.Password, out var guid))
            return Ok(guid);
        
        return Unauthorized();
    }
    
    [HttpPost("Register")]
    public IActionResult GenToken(UserInformation userInformation)
    {
        if (_authService.Register(userInformation.Login, userInformation.Password, out var guid))
            return Ok(guid);
        
        return Unauthorized();
    }

    [HttpGet]
    public IActionResult GetUser(string token)
    {
        if (_authService.TryGetUserByUid(token, out var login))
            return Ok($"\"{login}\""); 
        
        return Unauthorized();
    }
}