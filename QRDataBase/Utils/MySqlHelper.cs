namespace QRDataBase.Utils;

public static class MySqlHelper
{
    public static string Stringify(object o)
    {
        if (o is int i) return i.ToString();

        return $"'{o}'";
    }

    public static string GetSqlType(Type type)
    {
        if (type == typeof(int) || type == typeof(long)) return "INT";
        if (type == typeof(string)) return "VARCHAR(255)";
        if (type == typeof(bool)) return "BOOLEAN";
        if (type == typeof(DateTime)) return "DATETIME";
        if (type == typeof(byte[])) return "MEDIUMBLOB";

        throw new Exception($"{type} NOT FOUND");
    }
}