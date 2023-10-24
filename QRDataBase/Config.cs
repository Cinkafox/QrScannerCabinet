using QRDataBase.Providers;

namespace QRDataBase;

public static class Config
{
    public static readonly DataBaseOption ConnectionOption = new("localhost", 27017, "QRDB", "qrlet", "12341");
    public static readonly IDataBaseProvider Provider = new MongoDBProvider();
}