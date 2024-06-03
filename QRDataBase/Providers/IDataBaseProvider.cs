using QRDataBase.Filter;
using QRShared;

namespace QRDataBase.Providers;

public interface IDataBaseProvider : IDisposable
{
   public void Push<T>(T data);
   public List<T> Get<T>(ISearchItem? search = null);
}

