using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace QRScanner.Services;

public class AuthService
{
    private readonly RestService _rest;
    private readonly DebugService _debug;

    public Guid Token { get; private set; } = Guid.Empty;
    public string? Login { get; private set; }
    public string? Reason { get; private set; }
    public Uri? CurrentUri { get; set; }
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
            return;
        }
        
        var authCheckUri = new UriBuilder(CurrentUri.Scheme, CurrentUri.Host, CurrentUri.Port, "/Auth");
        var login = await _rest.GetAsync<string>(authCheckUri.Uri,cancellationToken,Token);
        
        Login = login;
        Reason = login.Message;
        IsAuthRequired = login.IsAuthRequire;
        IsSuccessful = !string.IsNullOrEmpty(login);
    }

    public async Task Auth(string login, string password, CancellationToken cancellationToken)
    {
        if (CurrentUri is null)
        {
            _debug.Error("CurrentUri is not set!");
            return;
        }
        
        var authUri = new UriBuilder(CurrentUri.Scheme, CurrentUri.Host, CurrentUri.Port, "/Auth/Login");
        var query = HttpUtility.ParseQueryString("");
        query.Add("login",login);
        query.Add("password", password);
        authUri.Query = query.ToString();
        
        var guid = await _rest.GetAsync<string>(authUri.Uri, cancellationToken);

        Login = login;
        Reason = guid.Message;
        IsAuthRequired = guid.IsAuthRequire;
        IsSuccessful = Guid.TryParse(guid, out var parsed);

        if (IsSuccessful)
            Token = parsed;
    } 
}