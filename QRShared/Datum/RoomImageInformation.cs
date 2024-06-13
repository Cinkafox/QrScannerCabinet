using QRShared.DataBase.Attributes;
using QRShared.Datum.DataBase.Attributes;

namespace QRShared.Datum;

public class RoomImageInformation 
{
    [PrimaryKey,ValueNotNull, AutoIncrement]
    public long Id { get; set; }
    [ValueNotNull]
    public long RoomId { get; set; }
    public string URL { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}