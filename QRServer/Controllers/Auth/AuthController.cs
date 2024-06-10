using Microsoft.AspNetCore.Mvc;
using QRServer.Auth;
using QRShared;

namespace QRServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthManager _authManager;
    
    public AuthController(AuthManager authManager)
    {
        _authManager = authManager;
    }

    [HttpPost]
    public IActionResult GetToken(UserInformation userInformation)
    {
        if (_authManager.TryAuth(userInformation.Login, userInformation.Password, out var guid))
            return Ok(guid);
        
        return Unauthorized();
    }
    
    [HttpPost("Register")]
    public IActionResult GenToken(UserInformation userInformation)
    {
        if (_authManager.Register(userInformation.Login, userInformation.Password, out var guid))
            return Ok(guid);
        
        return Unauthorized();
    }

    [HttpGet]
    public IActionResult GetUser(string token)
    {
        if (_authManager.TryGetUserByUid(token, out var login))
            return Ok(login); 
        
        return Unauthorized();
    }
}