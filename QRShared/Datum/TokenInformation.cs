using QRShared.DataBase.Attributes;

namespace QRShared.Datum;

public class TokenInformation
{
    [PrimaryKey] public string Token { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;
}