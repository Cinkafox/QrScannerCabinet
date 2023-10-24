using QRShared;

namespace QRDataBase.Providers;

public interface IDataBaseProvider
{
   public void Connect(DataBaseOption option);
   public void CreateInformation<T>(BaseInformation information) where T : BaseInformation;
   public List<T> GetInformationById<T>(long id, string idName = "_id") where T : BaseInformation;
}