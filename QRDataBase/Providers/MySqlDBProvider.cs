using System.Data;
using System.Reflection;
using MySql.Data.MySqlClient;
using QRDataBase.Filter;
using QRShared.DataBase.Attributes;
using QRShared.Datum.DataBase.Attributes;
using MySqlHelper = QRDataBase.Utils.MySqlHelper;

namespace QRDataBase.Providers;

public class MySqlDBProvider : IDataBaseProvider, IAsyncDisposable
{
    private MySqlConnection _mySqlConnection = default!;

    public async ValueTask DisposeAsync()
    {
        await _mySqlConnection.DisposeAsync();
    }

    public void Connect(DataBaseOption option)
    {
        _mySqlConnection = new MySqlConnection(new MySqlConnectionStringBuilder
        {
            Server = option.Ip, Port = (uint)option.Port, Database = option.DataBase, UserID = option.Login,
            Password = option.Password, CharacterSet = "utf8"
        }.ConnectionString);
        _mySqlConnection.Open();
    }

    public void Push<T>(T information, bool force = false)
    {
        CreateTableIfNotExist<T>();

        if (Has<T>(GetKeyProperty(information)))
            if (!force)
                return;

        using var command = _mySqlConnection.CreateCommand();
        CreateInsertCommand(information, command, force);
        command.ExecuteNonQuery();
    }

    public List<T> Get<T>(ISearchItem? search = null, int limit = -1)
    {
        CreateTableIfNotExist<T>();
        var objList = new List<T>();

        using var command = _mySqlConnection.CreateCommand();
        command.CommandText = "SELECT * FROM " + typeof(T).Name;
        ExtendSearch(command, search);

        if (limit > 0)
            command.CommandText += $" limit {limit}";

        using var reader = command.ExecuteReader();
        while (reader.Read()) objList.Add(Parse<T>(reader));

        return objList;
    }

    public void Remove<T>(ISearchItem? search = null)
    {
        CreateTableIfNotExist<T>();
        using var command = _mySqlConnection.CreateCommand();
        command.CommandText = "DELETE FROM " + typeof(T).Name;
        ExtendSearch(command, search);

        command.ExecuteNonQuery();
    }

    public bool Has<T>(ISearchItem? search = null)
    {
        return Get<T>(search, 1).Count > 0;
    }

    public void Dispose()
    {
        _mySqlConnection.Dispose();
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
        if (search is not null) command.CommandText += $" WHERE {search}";
    }

    private T Parse<T>(MySqlDataReader reader)
    {
        var instance = Activator.CreateInstance<T>();
        var type = typeof(T);

        foreach (var property in type.GetProperties())
        {
            var value = reader[property.Name];
            property.SetValue(instance, value);
        }

        return instance;
    }

    private void CreateInsertCommand<T>(T value, MySqlCommand command, bool force = false)
    {
        var propertyList = new List<string>();
        var insertCommand = "INSERT INTO";
        if (force) insertCommand = "REPLACE INTO";

        foreach (var property in typeof(T).GetProperties())
        {
            if (property.GetCustomAttribute<AutoIncrementAttribute>() is not null && !force)
                continue;

            var propValue = property.GetValue(value);
            if (propValue is null)
            {
                if (property.GetCustomAttribute<ValueNotNullAttribute>() is not null)
                    throw new ArgumentNullException(property.Name);
                continue;
            }

            AddProperty(command, property.Name, propValue);
            propertyList.Add(property.Name);
        }

        command.CommandText =
            $"{insertCommand} {typeof(T).Name} ({string.Join(',', propertyList)}) VALUES ({string.Join(',', propertyList.Select(a => "@" + a))})";
    }

    public void AddProperty(MySqlCommand mySqlCommand, string name, object value)
    {
        var dbName = "@" + name;
        switch (value)
        {
            case byte[] b:
                mySqlCommand.Parameters.Add(dbName, MySqlDbType.MediumBlob, b.Length);
                break;
            case int:
                mySqlCommand.Parameters.Add(dbName, MySqlDbType.Int32);
                break;
            case long:
                mySqlCommand.Parameters.Add(dbName, MySqlDbType.Int32);
                break;
            case string:
                mySqlCommand.Parameters.Add(dbName, MySqlDbType.VarChar, 256);
                break;
            default:
                throw new Exception($"{value.GetType()} no type");
        }

        mySqlCommand.Parameters[dbName].Value = value;
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

                command.CommandText += $"`{property.Name}` {MySqlHelper.GetSqlType(property.PropertyType)}{args},";
            }

            command.CommandText += $"PRIMARY KEY (`{primary}`))";
            command.ExecuteNonQuery();
        }
    }
}