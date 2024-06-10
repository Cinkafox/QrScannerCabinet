using QRShared.DataBase.Attributes;

namespace QRShared;

public class RoomImageInformation 
{
    [PrimaryKey,ValueNotNull, AutoIncrement]
    public long Id { get; set; }
    [ValueNotNull]
    public long RoomId { get; set; }
    public string URL { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}