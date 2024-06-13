using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRDataBase.Providers;
using QRServer.Controllers.Auth;
using QRServer.Controllers.Images;
using QRServer.Services;
using QRServer.Services.AuthProvider;
using QRServer.Services.FileApi;
using QRServer.Services.TokenProvider;
using QRShared.Datum;

namespace QRTest.Server.Controllers;

[TestFixture]
public class Image
{
    [SetUp]
    public void Setup()
    {
        _dataBaseProvider = new LocalDBProvider();
        _fileApi = new TempFileApi();
    }

    private IDataBaseProvider _dataBaseProvider = default!;
    private IAuthDataProvider _authDataProvider = default!;
    private AuthService _authService = default!;
    private ITokenProvider _tokenProvider = default!;
    private IFileApi _fileApi = default!;

    [Test]
    public void TestLocalDatabase()
    {
        _authDataProvider = new LocalAuthProvider();
        _tokenProvider = new DataBaseTokenProvider(_dataBaseProvider);

        _authService = new AuthService(_authDataProvider, _tokenProvider);
        Test();
    }

    [Test]
    public void TestDatabaseLocal()
    {
        _authDataProvider = new DataBaseAuthProvider(_dataBaseProvider);
        _tokenProvider = new LocalTokenProvider();

        _authService = new AuthService(_authDataProvider, _tokenProvider);
        Test();
    }

    [Test]
    public void TestDatabaseDatabase()
    {
        _authDataProvider = new DataBaseAuthProvider(_dataBaseProvider);
        _tokenProvider = new DataBaseTokenProvider(_dataBaseProvider);

        _authService = new AuthService(_authDataProvider, _tokenProvider);
        Test();
    }

    [Test]
    public void TestLocalLocal()
    {
        _authDataProvider = new LocalAuthProvider();
        _tokenProvider = new LocalTokenProvider();

        _authService = new AuthService(_authDataProvider, _tokenProvider);
        Test();
    }

    public Guid Auth()
    {
        var userInfo = new UserInformation
        {
            Login = "Test", Password = "Test"
        };

        var authController = new AuthController(_authService);
        var result = authController.GenToken(userInfo);
        Assert.IsAssignableFrom<OkObjectResult>(result);
        var ok = (OkObjectResult)result;
        Assert.IsAssignableFrom<Guid>(ok.Value);
        return (Guid)ok.Value;
    }

    public void Test()
    {
        var token = Auth();
        var imageController = new ImageController(_fileApi, _authService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        imageController.Request.Scheme = "https";
        imageController.Request.Host = new HostString("localhost");
        imageController.Request.Path = "";

        var data = "whatever and whatnever"u8.ToArray();
        var stream = new MemoryStream(data);
        var form = new FormFile(stream, 0, data.Length, "image.jpeg", "image.jpeg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        var result = imageController.Post(form, token.ToString());
        Assert.IsAssignableFrom<OkObjectResult>(result);
        var ok = (OkObjectResult)result;
        Assert.IsAssignableFrom<Uri>(ok.Value);
        var imageUrl = (Uri)ok.Value;
        var imageUid = imageUrl.LocalPath[1..];

        var getResult = imageController.Get(imageUid);
        Assert.IsAssignableFrom<FileStreamResult>(getResult);
        var fileResult = (FileStreamResult)getResult;

        var emptyData = new byte[data.Length];
        var fileStream = fileResult.FileStream;
        fileStream.Seek(0, SeekOrigin.Begin);
        fileStream.Read(emptyData, 0, data.Length);
        Assert.That(emptyData, Is.EqualTo(data));

        stream.Dispose();
        fileStream.Dispose();
    }
}