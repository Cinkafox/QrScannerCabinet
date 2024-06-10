using MongoDB.Driver;
using QRDataBase.Filter;
using QRDataBase.Filter.Operator;

namespace QRDataBase.Providers;

public class MongoDBProvider : IDataBaseProvider
{
    private MongoClient _client = default!;
    private IMongoDatabase _database = default!;

    public MongoDBProvider()
    {
        Connect(Config.ConnectionOption);
    }

    public void Connect(DataBaseOption option)
    {
        _client = new MongoClient(new MongoClientSettings()
        {
            Server = new MongoServerAddress(option.Ip, option.Port),
            Credential = MongoCredential.CreateCredential(option.DataBase,option.Login,option.Password)
        });
        
        _database = _client.GetDatabase(option.DataBase);
    }
    
    public void Push<T>(T data,bool force = false)
    {
        _database.GetCollection<T>(typeof(T).Name).InsertOneAsync(data);
    }

    public List<T> Get<T>(ISearchItem? search = null, int limit = -1)
    {
        return _database.GetCollection<T>(typeof(T).Name).FindSync(BuildFilter<T>(search)).ToList();
    }

    public void Remove<T>(ISearchItem? search = null)
    {
        _database.GetCollection<T>(typeof(T).Name).DeleteMany(BuildFilter<T>(search));
    }

    public bool Has<T>(ISearchItem? search = null)
    {
        return Get<T>().Count > 0;
    }

    public void Dispose()
    {
        
    }

    private FilterDefinition<T> BuildFilter<T>(ISearchItem? searchItem)
    {
        if(searchItem is null) 
            return FilterDefinition<T>.Empty;
        
        return BuildFilter(new FilterDefinitionBuilder<T>(),searchItem);
    }

    private FilterDefinition<T> BuildFilter<T>(FilterDefinitionBuilder<T> filterDefinitionBuilder, ISearchItem searchItem)
    {
        switch (searchItem)
        {
            case DbKeyValue value:
                return filterDefinitionBuilder.Eq(value.Key, value.Value);
            case DbSearch search:
            {
                var currAction = DbOperator.OR;

                FilterDefinition<T>? tempDef = null;
                
                foreach (var item in search.Items)
                {
                    if (item is IDbOperator opera)
                    {
                        currAction = opera.Operator;
                        continue;
                    }
                    
                    var currFilter = BuildFilter(filterDefinitionBuilder, item);

                    if (currAction == DbOperator.NOT)
                    {
                        tempDef = filterDefinitionBuilder.Not(currFilter);
                        continue;
                    }

                    if (tempDef != null)
                    {
                        switch (currAction)
                        {
                            case DbOperator.AND:
                                tempDef = filterDefinitionBuilder.And(tempDef, currFilter);
                                break;
                            case DbOperator.OR:
                                tempDef = filterDefinitionBuilder.Or(tempDef, currFilter);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        tempDef = currFilter;
                    }
                    
                }
                return tempDef!;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
