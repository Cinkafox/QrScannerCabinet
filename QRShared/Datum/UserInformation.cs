#nullable enable
using QRShared.DataBase.Attributes;

namespace QRShared;

public class UserInformation
{
    [PrimaryKey,ValueNotNull]
    public string Login { get; init; } = string.Empty;
    [ValueNotNull]
    public string Password { get; init; } = string.Empty;
}