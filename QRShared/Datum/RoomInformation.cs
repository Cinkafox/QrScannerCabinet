using QRShared.DataBase.Attributes;
using QRShared.Datum.DataBase.Attributes;

namespace QRShared.Datum;

public class RoomInformation
{
    [PrimaryKey] [ValueNotNull] public long Id { get; set; }

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}