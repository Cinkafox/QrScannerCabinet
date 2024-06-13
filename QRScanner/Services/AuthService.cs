using System.Collections.Specialized;
using System.Net;
using System.Web;
using QRShared.Datum;

namespace QRScanner.Services;

public class AuthService
{
    private readonly RestService _rest;
    private readonly DebugService _debug;
    private readonly UriHolderService _uriHolderService;

    public Guid Token { get; private set; } = Guid.Empty;
    public string? Login { get; private set; }
    public string? Reason { get; private set; }
    public bool IsAuthRequired { get; private set; } = true;
    public bool IsSuccessful { get; private set; }

    public AuthService(RestService rest,DebugService debug, UriHolderService uriHolderService)
    {
        _rest = rest;
        _debug = debug;
        _uriHolderService = uriHolderService;
    }

    public async Task CheckAuth(CancellationToken cancellationToken)
    {
        if (!_uriHolderService.EnsureUri())
        {
            Reason = "CurrentUri is not set!";
            return;
        }
        
        var login = await _rest.GetAsync<string>(_uriHolderService.AuthUri,cancellationToken,Token);
        
        Login = login.Value;
        Reason = login.Message;
        IsAuthRequired = login.IsAuthRequire;
        IsSuccessful = !string.IsNullOrEmpty(login);
    }

    public async Task Auth(string login, string password, CancellationToken cancellationToken)
    {
        if (!_uriHolderService.EnsureUri())
        {
            Reason = "CurrentUri is not set!";
            return;
        }
        
        var guid = await _rest.PostAsync<string,UserInformation>(new UserInformation()
        {
            Login = login, Password = password
        },_uriHolderService.AuthUri, cancellationToken);

        Login = login;
        Reason = guid.Message;
        IsAuthRequired = guid.IsAuthRequire;
        IsSuccessful = Guid.TryParse(guid, out var parsed);

        if (IsSuccessful)
            Token = parsed;
    } 
}