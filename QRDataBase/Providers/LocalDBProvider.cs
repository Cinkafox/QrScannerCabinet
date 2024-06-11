using System.Reflection;
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
        Console.WriteLine("PUSHED " + data);
        strObj(data);
        if (data != null) 
            objList.Add(data);
    }

    private void strObj(object? data)
    {
        if(data == null) return;
        foreach (var prop in data.GetType().GetProperties())
        {
            Console.WriteLine(prop.Name + " = " + prop.GetValue(data));
        }
    }

    public List<T> Get<T>(ISearchItem? search = null, int limit = -1)
    {
        Console.WriteLine("REQURED " + search);
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
        if (obj is null)
            return false;

        var oriProp = obj.GetType().GetProperty(dbKeyValue.Key);
        var oriValue = oriProp?.GetValue(obj);
        Console.WriteLine(dbKeyValue.Key + ": EQU " + dbKeyValue.Value + " " + oriValue);
        return oriValue?.Equals(dbKeyValue.Value) ?? false;
    }
}