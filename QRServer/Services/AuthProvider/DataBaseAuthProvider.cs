using QRDataBase.Filter;
using QRDataBase.Providers;
using QRShared.Datum;

namespace QRServer.Services.AuthProvider;

public class DataBaseAuthProvider : IAuthDataProvider
{
    private readonly IDataBaseProvider _provider;

    public DataBaseAuthProvider(IDataBaseProvider provider)
    {
        _provider = provider;
    }

    public bool TryGetPassword(string login, out string password)
    {
        var ba = _provider.Get<UserInformation>(new DbKeyValue(nameof(UserInformation.Login), login));
        if (ba.Count > 0)
        {
            password = ba[0].Password;
            return true;
        }

        password = string.Empty;
        return false;
    }

    public bool TryRegister(string login, string password)
    {
        var ba = _provider.Get<UserInformation>(new DbKeyValue(nameof(UserInformation.Login), login));
        if (ba.Count > 0) return false;
        
        _provider.Push(new UserInformation()
        {
            Login = login,Password = password
        });

        return true;
    }
}
