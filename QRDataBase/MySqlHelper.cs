namespace QRDataBase;

public static class MySqlHelper
{
    public static string Stringify(object o)
    {
        if (o is int i)
        {
            return i.ToString();
        }

        return $"'{o}'";
    }
}