using QRServer.Services.AuthProvider;
using QRServer.Services.TokenProvider;

namespace QRServer.Services;

public class AuthService
{
    private IAuthDataProvider _authDataProvider;
    private ITokenProvider _tokenProvider;

    public AuthService(IAuthDataProvider authDataProvider,ITokenProvider tokenProvider)
    {
        _authDataProvider = authDataProvider;
        _tokenProvider = tokenProvider;
    }

    public bool TryAuth(string login, string password, out Guid guid)
    {
        if (!_authDataProvider.TryGetPassword(login, out var pswd) || password != pswd)
        {
            guid = Guid.Empty;
            return false;
        }

        if(!_tokenProvider.TryGet(login, out guid))
            guid = _tokenProvider.Add(login);
        
        return true;
    }

    public bool HasAuthed(Guid guid)
    {
        return _tokenProvider.TryGet(guid, out _);
    }

    public bool HasAuthed(string guid)
    {
        return HasAuthed(Guid.Parse(guid));
    }

    public bool Register(string login, string password, out Guid guid)
    {
        if (!_authDataProvider.TryRegister(login, password))
        {
            guid = Guid.Empty;
            return false;
        }

        return TryAuth(login, password, out guid);
    }

    public bool TryGetUserByUid(Guid guid, out string login)
    {
        if (_tokenProvider.TryGet(guid, out login))
        {
            return true;
        }

        login = string.Empty;
        return false;
    }

    public bool TryGetUserByUid(string guid, out string login)
    {
        return TryGetUserByUid(Guid.Parse(guid),out login);
    }
}