using QRDataBase.Providers;
using QRServer.Services.FileApi;

namespace QRTest.Server.FileApi;

[TestFixture]
public class FileApiTest
{
    [Test]
    public void TestDB()
    {
        var fileApi = new DBFileApi(new LocalDBProvider());
        Test(fileApi);
    }

    [Test]
    public void TestTemp()
    {
        var fileApi = new TempFileApi();
        Test(fileApi);
    }


    public void Test(IFileApi fileApi)
    {
        var data = "bla-bla"u8.ToArray();
        using var stream = new MemoryStream(data);
        fileApi.Save(stream, "test.txt");
        //using var fileStream = fileApi.Open("test.txt");
        Assert.IsTrue(fileApi.TryOpen("test.txt", out var fileStream));
        Assert.IsNotNull(fileStream);
        var fileBuff = new byte[data.Length];
        fileStream.Read(fileBuff, 0, data.Length);
        Assert.That(fileBuff, Is.EqualTo(data));
    }
}