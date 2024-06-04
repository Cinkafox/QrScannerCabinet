namespace QRShared;

public class BaseInformation
{
   public long Id { get; set; }
}

public class RoomInformation : BaseInformation
{
   public string Name { get; init; } = string.Empty;
   public string Description { get; init; } = string.Empty;
}

public class RoomImageInformation : BaseInformation
{
   public long RoomId { get; set; }
   public string URL { get; init; } = string.Empty;
   public string Description { get; init; } = string.Empty;
}