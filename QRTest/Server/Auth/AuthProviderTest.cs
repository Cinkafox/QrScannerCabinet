using QRDataBase.Providers;
using QRServer.Services.AuthProvider;

namespace QRTest.Server.Auth;

[TestFixture]
public class AuthProviderTest
{
    [SetUp]
    public void Setup()
    {
        _dataBaseProvider = new LocalDBProvider();
    }

    private IDataBaseProvider _dataBaseProvider = default!;

    [Test]
    public void LocalTest()
    {
        var local = new LocalAuthProvider();
        BaseTest(local);
    }

    [Test]
    public void DataBaseTest()
    {
        var db = new DataBaseAuthProvider(_dataBaseProvider);
        BaseTest(db);
    }

    public void BaseTest(IAuthDataProvider provider)
    {
        Assert.That(provider.TryRegister("test", "think"));
        Assert.That(provider.TryGetPassword("test", out var password));
        Assert.That(password, Is.EqualTo("think"));
    }
}