using QRDataBase.Providers;

namespace QRDataBase;

public class DataBaseManager : IDataBaseManager
{
    private readonly IDataBaseProvider _provider;
    
    public DataBaseManager()
    {
        _provider = Config.Provider;
        _provider.Connect(Config.ConnectionOption);
    }

    public IDataBaseProvider GetProvider()
    {
        return _provider;
    }
}