using QRShared;

namespace QRDataBase.Providers;

public interface IDataBaseProvider
{
   public void Connect(DataBaseOption option);
   public void CreateRoom(RoomInformation information);
   public RoomInformation? GetRoomById(long id);
}