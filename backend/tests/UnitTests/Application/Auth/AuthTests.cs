using FitHub.Authentication;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.AspNetCore.Tokens;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FitHub.UnitTests.Application.Auth;

public class AuthTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public AuthTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact(DisplayName = "Сгенерировать токен доступа")]//, Skip = "Метод не предназначен для автоматических тестов")]
    public void GenerateToken()
    {
        var optionsMock = new Mock<IOptions<IAuthOptions>>();
        var authOptionsMock = new Mock<IAuthOptions>();

        authOptionsMock.Setup(service =>
                service.RequiredSecretKey)
            .Returns("some-very-very-very-very-secret-key");

        authOptionsMock.Setup(service =>
                service.RequiredIssuer)
            .Returns("FitHub");

        authOptionsMock.Setup(service =>
                service.RequiredCookieExpiration)
            .Returns(DateTimeOffset.MaxValue - TimeSpan.FromDays(1) - DateTimeOffset.UtcNow);

        optionsMock.Setup(service =>
                service.Value)
            .Returns(authOptionsMock.Object);

        var tokenService = new TokenService(optionsMock.Object);

        var claims = ITokenService.CreateCommonClaims(sub: "a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc", name: "Мамедов Александр", sessionId: "sid", userType: IdentityUserType.CmsAdmin);
        var accessToken = tokenService.Create(claims);

        testOutputHelper.WriteLine(accessToken);
    }
}
