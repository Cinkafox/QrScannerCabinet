using QRDataBase;

namespace QRServer.DataBaseConnection;

public class DataBaseConnection
{
    private readonly IConfiguration _configuration;

    public DataBaseConnection(IConfiguration configuration)
    {
        _configuration = configuration;
        Config.ConnectionOption = _configuration.GetSection("DataBase").Get<DataBaseOption>()!;
    }
}