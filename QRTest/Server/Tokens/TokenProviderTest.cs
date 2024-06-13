using QRDataBase.Providers;
using QRServer.Services.TokenProvider;

namespace QRTest.Server.Tokens;

[TestFixture]
public class TokenProviderTest
{
    [SetUp]
    public void Setup()
    {
        _dataBaseProvider = new LocalDBProvider();
    }

    private IDataBaseProvider _dataBaseProvider = default!;

    public void ProviderTestBase(ITokenProvider provider)
    {
        for (var i = 0; i < 20; i++) TestSingle(provider, "test" + i);
    }

    public void TestSingle(ITokenProvider provider, string key)
    {
        var origGuid = provider.Add(key);
        Assert.That(provider.TryGet(key, out var guid), Is.True);
        Assert.That(guid, Is.EqualTo(origGuid));
    }


    [Test]
    public void LocalProviderTest()
    {
        var provider = new LocalTokenProvider();
        ProviderTestBase(provider);
    }

    [Test]
    public void DbProviderTest()
    {
        var provider = new DataBaseTokenProvider(_dataBaseProvider);
        ProviderTestBase(provider);
    }
}