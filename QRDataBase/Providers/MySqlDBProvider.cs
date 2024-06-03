using System.Reflection;
using MySql.Data.MySqlClient;
using QRDataBase.Filter;
using QRShared;

namespace QRDataBase.Providers;

public class MySqlDBProvider : IDataBaseProvider, IAsyncDisposable
{
    private MySqlConnection _mySqlConnection = default!;
    
    public MySqlDBProvider()
    {
        Connect(Config.ConnectionOption);
    }
    
    public void Connect(DataBaseOption option)
    {
        _mySqlConnection = new MySqlConnection(new MySqlConnectionStringBuilder()
        {
            Server = option.Ip, Port = (uint)option.Port, Database = option.DataBase, UserID = option.Login, Password = option.Password
        }.ConnectionString);
        _mySqlConnection.Open();
    }
    
    public void Push<T>(T information)
    {
        CreateTableIfNotExist<T>();

        using var command = _mySqlConnection.CreateCommand();
        DeRar(information, command);
        command.ExecuteNonQuery();
    }

    public List<T> Get<T>(ISearchItem? search = null)
    {
        CreateTableIfNotExist<T>();
        var objList = new List<T>();
        
        using var command = _mySqlConnection.CreateCommand();
        command.CommandText = "SELECT * FROM " + typeof(T).Name;
        if (search is not null)
        {
            command.CommandText += $" WHERE {search}";
        }
      
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            objList.Add(Parse<T>(reader));
        }

        return objList;
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

        throw new Exception("OBOSRALIS");
    }
    
    private T Parse<T>(MySqlDataReader reader)
    {
        var instance = Activator.CreateInstance<T>();
        var type = typeof(T);
        
        foreach (var field in type.GetProperties())
        {
            var value = reader[field.Name];
            field.SetValue(instance,value);
        }

        return instance;
    }

    private void DeRar<T>(T value, MySqlCommand command)
    {
        command.CommandText = "INSERT INTO " + typeof(T).Name + " VALUES (";
        
        foreach (var field in typeof(T).GetProperties())
        {
            command.CommandText += $"{MySqlHelper.Stringify(field.GetValue(value)!)}, ";
        }
        command.CommandText = command.CommandText.Remove(command.CommandText.Length - 2, 2);
        command.CommandText += ")";
    }

    private void CreateTableIfNotExist<T>()
    {
        using (var command = _mySqlConnection.CreateCommand())
        {
            command.CommandText = "CREATE TABLE IF NOT EXISTS " + typeof(T).Name + " (";
            
            var primary = "Id";
            
            foreach (var field in typeof(T).GetProperties())
            {
                if (field.GetCustomAttribute<PrimaryKeyAttribute>() is { } primaryKeyAttribute)
                {
                    primary = field.Name;
                }
                
                command.CommandText += $"`{field.Name}` {GetSqlType(field.PropertyType)}, ";
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