using FitHub.Application.Users;
using FitHub.Authentication;
using FitHub.Common.Testing;
using Moq;

namespace FitHub.IntegrationTests.Infrastructure;

public class CurrentUserProvider : ITestDesiredUserIdProvider
{
    private readonly Mock<IIdentityUserService> identityUserServiceMock;

    public User CurrentUser { get; private set; }

    public CurrentUserProvider(User currentUser, Mock<IIdentityUserService> identityUserServiceMock)
    {
        this.identityUserServiceMock = identityUserServiceMock;
        SetupCurrentUser(currentUser);
        CurrentUser = currentUser;
    }

    public User SetupCurrentUser(User user)
    {
        CurrentUser = user;

        identityUserServiceMock.Setup(service =>
                service.GetOrDefaultAsync(CurrentUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        identityUserServiceMock.Setup(service =>
            service.IsSessionValid(CurrentUser.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return CurrentUser;
    }

    public IdentityUserId GetUserId() => CurrentUser.Id;
}
