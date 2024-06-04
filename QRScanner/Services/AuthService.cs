namespace QRScanner.Services;

public class AuthService
{
    private readonly RestService _rest;
    private readonly DebugService _debug;

    public Guid Token = Guid.Empty;
    public string? Login;

    public AuthService(RestService rest,DebugService debug)
    {
        _rest = rest;
        _debug = debug;
    }

    public async Task<bool> CheckAuth(Uri uri)
    {
        var login = await _rest.GetAsync<string>(new UriBuilder(uri.Scheme, uri.Host, uri.Port, $"/Auth?token={Token.ToString()}").Uri);
        Login = login;
        return !string.IsNullOrEmpty(login);
    }
}