using Microsoft.AspNetCore.Mvc;
using QRServer.Auth;

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

    [HttpGet("Login")]
    public string GetToken(string login, string password)
    {
        if (_authManager.TryAuth(login, password, out var guid))
        {
            return guid.ToString();
        }
        
        Response.StatusCode = 401;
        
        return string.Empty;
    }
    
    [HttpGet("Register")]
    public string GenToken(string login, string password)
    {
        if (_authManager.Register(login, password, out var guid))
        {
            return guid.ToString();
        }

        Response.StatusCode = 403;
        
        return string.Empty;
    }

    [HttpGet]
    public string GetUser(string token)
    {
        if (_authManager.TryGetUserByUid(token, out var login))
        {
            return login;
        }

        Response.StatusCode = 404;
        
        return string.Empty;
    }
}