using QRShared.DataBase.Attributes;
using QRShared.Datum.DataBase.Attributes;

namespace QRShared.Datum;

public class TokenInformation
{
    [PrimaryKey]
    public Guid Token { get; set; }
    public string Key { get; set; } = String.Empty;
}