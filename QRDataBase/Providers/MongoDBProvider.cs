using MongoDB.Bson;
using MongoDB.Driver;
using QRShared;

namespace QRDataBase.Providers;

public class MongoDBProvider : IDataBaseProvider
{
    private MongoClient _client = default!;
    private IMongoDatabase _database = default!;

    public void Connect(DataBaseOption option)
    {
        _client = new MongoClient(new MongoClientSettings()
        {
            Server = new MongoServerAddress(option.Ip, option.Port),
            Credential = MongoCredential.CreateCredential(option.DataBase,option.Login,option.Password)
        });
        
        _database = _client.GetDatabase(option.DataBase);
    }

    public void CreateInformation<T>(BaseInformation information) where T : BaseInformation
    {
        _database.GetCollection<T>(typeof(T).Name).InsertOneAsync((T)information);
    }

    public List<T> GetInformationById<T>(long id, string idName = "_id") where T : BaseInformation
    {
        return _database.GetCollection<T>(typeof(T).Name).FindSync(new BsonDocument(idName,id)).ToList();
    }
}