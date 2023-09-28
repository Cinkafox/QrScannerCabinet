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

    public void CreateRoom(RoomInformation information)
    {
        _database.GetCollection<RoomInformation>("rooms").InsertOneAsync(information);
    }

    public RoomInformation? GetRoomById(long id)
    {
        var rooms = _database.GetCollection<RoomInformation>("rooms").FindSync(new BsonDocument("_id",id)).ToList();
        return rooms.Count == 0 ? null : rooms.First();
    }
}