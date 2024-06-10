using System.Data;
using System.Reflection;
using MySql.Data.MySqlClient;
using QRDataBase.Filter;
using QRShared.DataBase.Attributes;
using QRShared.Datum;
using QRShared.Datum.DataBase.Attributes;

namespace QRDataBase.Providers;

public class MySqlDBProvider : IDataBaseProvider, IAsyncDisposable
{
    private MySqlConnection _mySqlConnection = default!;
    
    public void Connect(DataBaseOption option)
    {
        _mySqlConnection = new MySqlConnection(new MySqlConnectionStringBuilder()
        {
            Server = option.Ip, Port = (uint)option.Port, Database = option.DataBase, UserID = option.Login, Password = option.Password
        }.ConnectionString);
        _mySqlConnection.Open();
    }
    
    public void Push<T>(T information,bool force = false)
    {
        CreateTableIfNotExist<T>();

        using var command = _mySqlConnection.CreateCommand();
        var insertCommand = "INSERT INTO";
        if (Has<T>(GetKeyProperty(information)))
        {
            if(!force) return;
            insertCommand = "REPLACE INTO";
        }
        
        CreateInsertCommand(information, command, insertCommand);
        command.ExecuteNonQuery();
    }

    public List<T> Get<T>(ISearchItem? search = null, int limit = -1)
    {
        CreateTableIfNotExist<T>();
        var objList = new List<T>();
        
        using var command = _mySqlConnection.CreateCommand();
        command.CommandText = "SELECT * FROM " + typeof(T).Name;
        ExtendSearch(command,search);
        
        if (limit > 0)
            command.CommandText += $" limit {limit}";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            objList.Add(Parse<T>(reader));
        }

        return objList;
    }

    public void Remove<T>(ISearchItem? search = null)
    {
        CreateTableIfNotExist<T>();
        using var command = _mySqlConnection.CreateCommand();
        command.CommandText = "DELETE FROM " + typeof(T).Name;
        ExtendSearch(command,search);

        command.ExecuteNonQuery();
    }

    public bool Has<T>(ISearchItem? search = null)
    {
        return Get<T>(search,1).Count > 0;
    }

    private ISearchItem? GetKeyProperty<T>(T obj)
    {
        var type = typeof(T);
        foreach (var property in type.GetProperties())
        {
            if (property.GetCustomAttribute<PrimaryKeyAttribute>() is null) continue;
            var value = property.GetValue(obj);
            return new DbKeyValue(property.Name, value!);
        }

        return null;
    }

    private void ExtendSearch(IDbCommand command, ISearchItem? search)
    {
        if (search is not null)
        {
            command.CommandText += $" WHERE {search}";
        }
    }

    private string GetSqlType(Type type)
    {
        if (type == typeof(string))
        {
            return "varchar(255)";
        }

        if (type == typeof(int) || type == typeof(long))
        {
            return "INT";
        }

        throw new Exception();
    }
    
    private T Parse<T>(MySqlDataReader reader)
    {
        var instance = Activator.CreateInstance<T>();
        var type = typeof(T);
        
        foreach (var property in type.GetProperties())
        {
            var value = reader[property.Name];
            property.SetValue(instance,value);
        }

        return instance;
    }

    private void CreateInsertCommand<T>(T value, IDbCommand command,string insertCommand = "INSERT INTO")
    {
        var propertyList = new List<string>();
        var valueList = new List<string>();
        
        foreach (var property in typeof(T).GetProperties())
        {
            if (property.GetCustomAttribute<AutoIncrementAttribute>() is not null)
            {
                continue;   
            }

            var propValue = property.GetValue(value);
            if (propValue is null)
            {
                if (property.GetCustomAttribute<ValueNotNullAttribute>() is not null)
                    throw new ArgumentNullException(property.Name);
                continue;
            }
            
            propertyList.Add(property.Name);
            valueList.Add(MySqlHelper.Stringify(propValue));
        }
        
        command.CommandText = $"{insertCommand} {typeof(T).Name} ({string.Join(',',propertyList)}) VALUES ({string.Join(',',valueList)})" ;
    }

    private void CreateTableIfNotExist<T>()
    {
        using (var command = _mySqlConnection.CreateCommand())
        {
            command.CommandText = "CREATE TABLE IF NOT EXISTS " + typeof(T).Name + " (";
            
            var primary = "Id";
            
            foreach (var property in typeof(T).GetProperties())
            {
                var args = "";
                if (property.GetCustomAttribute<PrimaryKeyAttribute>() is not null)
                    primary = property.Name;

                if (property.GetCustomAttribute<AutoIncrementAttribute>() is not null)
                    args += " AUTO_INCREMENT";

                if (property.GetCustomAttribute<ValueNotNullAttribute>() is not null)
                    args += " NOT NULL";
                
                command.CommandText += $"`{property.Name}` {GetSqlType(property.PropertyType)}{args},";
            }

            command.CommandText += $"PRIMARY KEY (`{primary}`))";
            command.ExecuteNonQuery();
        }
    }

    public void Dispose()
    {
        _mySqlConnection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _mySqlConnection.DisposeAsync();
    }
}