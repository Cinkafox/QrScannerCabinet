using QRDataBase.Filter;

namespace QRDataBase.Providers;

public class LocalDBProvider : IDataBaseProvider
{
    public List<object> objList = new();
    public void Dispose()
    {
        
    }

    public void Connect(DataBaseOption option)
    {
        
    }

    public void Push<T>(T data, bool force = false)
    {
        if (data != null) 
            objList.Add(data);
    }

    public List<T> Get<T>(ISearchItem? search = null, int limit = -1)
    {
        if (search is not DbKeyValue dbKeyValue) return new();
        return objList.OfType<T>().Where((a) => IsKey(a,dbKeyValue)).ToList();
    }

    public void Remove<T>(ISearchItem? search = null)
    {
        if (search is not DbKeyValue dbKeyValue) return;
        foreach (var obj in objList.OfType<T>().ToList())
        {
            if (IsKey(obj,dbKeyValue)) objList.Remove(obj!);
        }
    }

    public bool Has<T>(ISearchItem? search = null)
    {
        if (search is not DbKeyValue dbKeyValue) return false;
        return objList.OfType<T>().Any(obj => IsKey(obj, dbKeyValue));
    }

    private bool IsKey(object? obj, DbKeyValue dbKeyValue)
    {
        return obj?.GetType().GetProperty(dbKeyValue.Key)?.GetValue(obj) == dbKeyValue.Value;
    }
}