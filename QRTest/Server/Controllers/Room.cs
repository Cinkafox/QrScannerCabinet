using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRDataBase.Providers;
using QRServer.Controllers.Auth;
using QRServer.Controllers.Room;
using QRServer.Services;
using QRServer.Services.AuthProvider;
using QRServer.Services.TokenProvider;
using QRShared.Datum;

namespace QRTest.Server.Controllers;

[TestFixture]
public class Room
{
    [SetUp]
    public void Setup()
    {
        _dataBaseProvider = new LocalDBProvider();
    }

    private IDataBaseProvider _dataBaseProvider = default!;
    private IAuthDataProvider _authDataProvider = default!;
    private AuthService _authService = default!;
    private ITokenProvider _tokenProvider = default!;

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
        var roomInfo = new RoomInformation
        {
            Id = 15, Name = "Test", Description = "Testing"
        };

        var roomInfoController = new RoomInformationController(_dataBaseProvider, _authService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        Assert.That(roomInfoController.Post(roomInfo, token.ToString()), Is.True);
        var room = roomInfoController.Get(roomInfo.Id);
        Console.WriteLine(room.Id);
        Assert.That(room, Is.EqualTo(roomInfo));

        var imageInfo = new RoomImageInformation
        {
            Id = 1, RoomId = roomInfo.Id, Description = "Some test", URL = "https://bla.bla/bla/bla.bla"
        };

        Assert.That(roomInfoController.PostImage(imageInfo, token.ToString()), Is.True);
        Assert.That(roomInfoController.GetImage(roomInfo.Id), Is.Not.Empty);
        Assert.That(roomInfoController.DeleteImage(imageInfo.Id, token.ToString()));
        Assert.That(roomInfoController.GetImage(roomInfo.Id), Is.Empty);

        Assert.That(roomInfoController.Delete(roomInfo.Id, token.ToString()), Is.True);
        Assert.That(roomInfoController.Get(roomInfo.Id), Is.Not.EqualTo(roomInfo));
    }
}