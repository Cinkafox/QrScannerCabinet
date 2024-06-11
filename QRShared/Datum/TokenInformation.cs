using QRShared.DataBase.Attributes;
using QRShared.Datum.DataBase.Attributes;

namespace QRShared.Datum;

public class TokenInformation
{
    [PrimaryKey]
    public string Token { get; set; } = String.Empty;
    public string Key { get; set; } = String.Empty;
}