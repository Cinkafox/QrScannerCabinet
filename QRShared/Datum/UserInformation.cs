using QRShared.DataBase.Attributes;
using QRShared.Datum.DataBase.Attributes;

namespace QRShared.Datum;

public class UserInformation
{
    [PrimaryKey,ValueNotNull]
    public string Login { get; init; } = string.Empty;
    [ValueNotNull]
    public string Password { get; init; } = string.Empty;
}