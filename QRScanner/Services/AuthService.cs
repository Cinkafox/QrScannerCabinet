using System.Collections.Specialized;
using System.Net;
using System.Web;
using QRShared.Datum;

namespace QRScanner.Services;

public class AuthService
{
    private readonly RestService _rest;
    private readonly DebugService _debug;

    public Guid Token { get; private set; } = Guid.Empty;
    public string? Login { get; private set; }
    public string? Reason { get; private set; }
    public Uri? CurrentUri { get; set; }
    public Uri AuthUri => new Uri(CurrentUri, "/Auth");
    public bool IsAuthRequired { get; private set; } = true;
    public bool IsSuccessful { get; private set; }

    public AuthService(RestService rest,DebugService debug)
    {
        _rest = rest;
        _debug = debug;
    }

    public async Task CheckAuth(CancellationToken cancellationToken)
    {
        if (CurrentUri is null)
        {
            _debug.Error("CurrentUri is not set!");
            Reason = "CurrentUri is not set!";
            return;
        }
        
        var login = await _rest.GetAsync<string>(AuthUri,cancellationToken,Token);
        
        Login = login.Value;
        Reason = login.Message;
        IsAuthRequired = login.IsAuthRequire;
        IsSuccessful = !string.IsNullOrEmpty(login);
    }

    public async Task Auth(string login, string password, CancellationToken cancellationToken)
    {
        if (CurrentUri is null)
        {
            _debug.Error("CurrentUri is not set!");
            Reason = "CurrentUri is not set!";
            return;
        }
        
        var guid = await _rest.PostAsync<string,UserInformation>(new UserInformation()
        {
            Login = login, Password = password
        },AuthUri, cancellationToken);

        Login = login;
        Reason = guid.Message;
        IsAuthRequired = guid.IsAuthRequire;
        IsSuccessful = Guid.TryParse(guid, out var parsed);

        if (IsSuccessful)
            Token = parsed;
    } 
}