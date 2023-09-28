using QRDataBase.Providers;

namespace QRDataBase;

public interface IDataBaseManager
{
    public IDataBaseProvider GetProvider();
}