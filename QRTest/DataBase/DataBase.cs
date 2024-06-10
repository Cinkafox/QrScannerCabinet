using QRDataBase.Filter;
using QRDataBase.Providers;

namespace QRTest.DataBase;

[TestFixture]
public class DataBase
{
    private IDataBaseProvider _dataBaseProvider = default!;
    [SetUp]
    public void Setup()
    {
        _dataBaseProvider = new LocalDBProvider();
    }

    [Test]
    public void Test()
    {
        var data1 = new Data1()
        {
            Pole1 = "test1", Pole2 = "test2"
        };
        _dataBaseProvider.Push(data1);
        var result1 = _dataBaseProvider.Get<Data1>(new DbKeyValue("Pole1", "test1"));
        var result2 = _dataBaseProvider.Get<Data1>(new DbKeyValue("Pole2", "test2"));
        Assert.That(result1, Is.Not.Empty);
        Assert.That(result2, Is.Not.Empty);
        Assert.That(result1[0],Is.EqualTo(data1));
        Assert.That(result2[0],Is.EqualTo(data1));
        Assert.That(_dataBaseProvider.Has<Data1>(new DbKeyValue("Pole1", "test1")));
        Assert.That(_dataBaseProvider.Has<Data1>(new DbKeyValue("Pole2", "test2")));
        
        var data2 = new Data2()
        {
            Pole1 = "test1", Pole2 = "test2"
        };
        _dataBaseProvider.Push(data2);
        var tresult1 = _dataBaseProvider.Get<Data2>(new DbKeyValue("Pole1", "test1"));
        var tresult2 = _dataBaseProvider.Get<Data2>(new DbKeyValue("Pole2", "test2"));
        Assert.That(tresult1, Is.Not.Empty);
        Assert.That(tresult2, Is.Not.Empty);
        Assert.That(tresult1[0],Is.EqualTo(data2));
        Assert.That(tresult2[0],Is.EqualTo(data2));
        Assert.That(_dataBaseProvider.Has<Data2>(new DbKeyValue("Pole1", "test1")));
        Assert.That(_dataBaseProvider.Has<Data2>(new DbKeyValue("Pole2", "test2")));
        _dataBaseProvider.Remove<Data2>(new DbKeyValue("Pole1", "test1"));
        var tresult11 = _dataBaseProvider.Get<Data2>(new DbKeyValue("Pole1", "test1"));
        var tresult21 = _dataBaseProvider.Get<Data2>(new DbKeyValue("Pole2", "test2"));
        Assert.That(tresult11, Is.Empty);
        Assert.That(tresult21, Is.Empty);
        Assert.That(!_dataBaseProvider.Has<Data2>(new DbKeyValue("Pole1", "test1")));
        Assert.That(!_dataBaseProvider.Has<Data2>(new DbKeyValue("Pole2", "test2")));
        
        Assert.That(_dataBaseProvider.Has<Data1>(new DbKeyValue("Pole1", "test1")));
        Assert.That(_dataBaseProvider.Has<Data1>(new DbKeyValue("Pole2", "test2")));
        
        _dataBaseProvider.Remove<Data1>(new DbKeyValue("Pole1", "test1"));
        var result11 = _dataBaseProvider.Get<Data1>(new DbKeyValue("Pole1", "test1"));
        var result21 = _dataBaseProvider.Get<Data1>(new DbKeyValue("Pole2", "test2"));
        Assert.That(result11, Is.Empty);
        Assert.That(result21, Is.Empty);
        Assert.That(!_dataBaseProvider.Has<Data1>(new DbKeyValue("Pole1", "test1")));
        Assert.That(!_dataBaseProvider.Has<Data1>(new DbKeyValue("Pole2", "test2")));
    }
}

public class Data1
{
    public string Pole1 { get; set; } = string.Empty;
    public string Pole2 { get; set; } = string.Empty;
}

public class Data2
{
    public string Pole1 { get; set; } = string.Empty;
    public string Pole2 { get; set; } = string.Empty;
}