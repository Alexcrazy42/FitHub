using AutoFixture;
using AutoFixture.AutoMoq;
using FitHub.Application.Messaging;
using FitHub.Application.Users;
using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.TestsCommon;
using Moq;

namespace FitHub.UnitTests.Application;

public abstract class ApplicationTestsBase
{
    protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());

    protected IdentityUserId FirstUserId = IdentityUserId.Parse(Guid.NewGuid().ToString());
    protected IdentityUserId SecondUserId = IdentityUserId.Parse(Guid.NewGuid().ToString());
    protected IdentityUserId ThirdUserId = IdentityUserId.Parse(Guid.NewGuid().ToString());


    // Mocks
    protected readonly Mock<IChatRepository> ChatRepositoryMock;
    protected readonly Mock<IChatParticipantRepository> ChatParticipantRepositoryMock;
    protected readonly Mock<IUserService> UserServiceMock;
    protected readonly Mock<IUnitOfWork> UnitOfWorkMock;
    protected readonly Mock<ICurrentIdentityUserIdAccessor> CurrentIdentityUserIdAccessorMock;


    protected ApplicationTestsBase()
    {
        CustomizeEntities();
        ChatRepositoryMock = new Mock<IChatRepository>();
        ChatParticipantRepositoryMock = new Mock<IChatParticipantRepository>();
        UserServiceMock = new Mock<IUserService>();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        CurrentIdentityUserIdAccessorMock = new Mock<ICurrentIdentityUserIdAccessor>();
    }

    private void CustomizeEntities()
    {
        // Регаем билдера для создания сущностей без публичного конструктора (подразумевает наличие оного в принципе)
        AutoFixture.ResidueCollectors.Add(new NonPublicConstructorBuilder());

    }
}
