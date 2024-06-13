using QRDataBase.Utils;

namespace QRDataBase.Filter;

public class DbKeyValue : ISearchItem
{
    public string Key;
    public object Value;

    public DbKeyValue(string key, object value)
    {
        Key = key;
        Value = value;
    }

    public override string ToString()
    {
        return Key + " = " + MySqlHelper.Stringify(Value);
    }
}