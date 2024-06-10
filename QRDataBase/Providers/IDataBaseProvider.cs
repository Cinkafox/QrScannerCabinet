using QRDataBase.Filter;
using QRShared;

namespace QRDataBase.Providers;

public interface IDataBaseProvider : IDisposable
{
   public void Push<T>(T data,bool force = false);
   public List<T> Get<T>(ISearchItem? search = null, int limit = -1);
   public void Remove<T>(ISearchItem? search = null);
   public bool Has<T>(ISearchItem? search = null);
}

