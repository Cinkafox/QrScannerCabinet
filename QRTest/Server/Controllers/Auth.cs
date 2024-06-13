using Microsoft.AspNetCore.Mvc;
using QRDataBase.Providers;
using QRServer.Controllers.Auth;
using QRServer.Services;
using QRServer.Services.AuthProvider;
using QRServer.Services.TokenProvider;
using QRShared.Datum;

namespace QRTest.Server.Controllers;

[TestFixture]
public class Auth
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
        TestAuth();
    }

    [Test]
    public void TestDatabaseLocal()
    {
        _authDataProvider = new DataBaseAuthProvider(_dataBaseProvider);
        _tokenProvider = new LocalTokenProvider();

        _authService = new AuthService(_authDataProvider, _tokenProvider);
        TestAuth();
    }

    [Test]
    public void TestDatabaseDatabase()
    {
        _authDataProvider = new DataBaseAuthProvider(_dataBaseProvider);
        _tokenProvider = new DataBaseTokenProvider(_dataBaseProvider);

        _authService = new AuthService(_authDataProvider, _tokenProvider);
        TestAuth();
    }

    [Test]
    public void TestLocalLocal()
    {
        _authDataProvider = new LocalAuthProvider();
        _tokenProvider = new LocalTokenProvider();

        _authService = new AuthService(_authDataProvider, _tokenProvider);
        TestAuth();
    }


    public void TestAuth()
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
        var guid = (Guid)ok.Value;

        var userNameResult = authController.GetUser(guid.ToString());
        Assert.IsAssignableFrom<OkObjectResult>(userNameResult);
        var userOk = (OkObjectResult)userNameResult;
        Assert.IsAssignableFrom<string>(userOk.Value);
        var username = (string)userOk.Value;
        Assert.That(username, Is.EqualTo($"\"{userInfo.Login}\""));

        result = authController.GetToken(userInfo);
        Assert.IsAssignableFrom<OkObjectResult>(result);
        ok = (OkObjectResult)result;
        Assert.IsAssignableFrom<Guid>(ok.Value);
        Assert.That((Guid)ok.Value, Is.Not.EqualTo(guid));
    }
}