namespace QRDataBase;

public static class Config
{
    public static DataBaseOption ConnectionOption => new(
        Environment.GetEnvironmentVariable("ip") ?? "localhost",
        int.Parse(Environment.GetEnvironmentVariable("port") ?? "3306"),
        Environment.GetEnvironmentVariable("dataBase") ?? "root",
        Environment.GetEnvironmentVariable("login") ?? "root",
        Environment.GetEnvironmentVariable("password") ?? "root");
    
}