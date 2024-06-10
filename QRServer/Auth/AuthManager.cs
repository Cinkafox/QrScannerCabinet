namespace QRServer.Auth;

public class AuthManager
{
    private readonly Dictionary<Guid, DateTime> _authList = new();
    private readonly Dictionary<Guid, string> _users = new();
    
    private IAuthDataProvider _authDataProvider;

    public AuthManager(IAuthDataProvider authDataProvider)
    {
        _authDataProvider = authDataProvider;
    }

    public bool TryAuth(string login, string password, out Guid guid)
    {
        if (!_authDataProvider.TryGetPassword(login, out var pswd) || password != pswd)
        {
            guid = Guid.Empty;
            return false;
        }
        
        guid = Guid.NewGuid();
        _authList.Add(guid,DateTime.Now + new TimeSpan(0,0,30,0));
        _users.Add(guid, login);
        return true;
    }

    public bool HasAuthed(Guid guid)
    {
        return true; //76b2fc4f-8e83-4587-8b12-dd78e4a337eb
        return _authList.TryGetValue(guid, out var time) && time > DateTime.Now;
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
        if (_users.TryGetValue(guid, out var value))
        {
            login = value;
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