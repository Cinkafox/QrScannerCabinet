namespace QRShared;

public class RoomInformation
{
   public long Id { get; set; }
   public string Name { get; init; } = string.Empty;
}

public static class NullInformation
{
   public static RoomInformation Information = new RoomInformation()
   {
      Id = -1,
      Name = string.Empty
   };
}