using AutoFixture;
using AutoFixture.AutoMoq;
using FitHub.Application.Messaging;
using FitHub.Application.Users;
using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Common.Entities.Storage;
using FitHub.Common.TestsCommon;
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
    protected readonly Mock<IMessageRepository> MessageRepositoryMock;
    protected readonly Mock<IMessageAttachmentRepository> MessageAttachmentRepositoryMock;
    protected readonly Mock<IMessageViewRepository> MessageViewRepositoryMock;
    protected readonly Mock<IChatReadingModelRepository> ChatReadingModelRepositoryMock;

    protected ApplicationTestsBase()
    {
        CustomizeEntities();
        ChatRepositoryMock = new Mock<IChatRepository>();
        ChatParticipantRepositoryMock = new Mock<IChatParticipantRepository>();
        UserServiceMock = new Mock<IUserService>();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        CurrentIdentityUserIdAccessorMock = new Mock<ICurrentIdentityUserIdAccessor>();
        MessageRepositoryMock = new Mock<IMessageRepository>();
        MessageAttachmentRepositoryMock = new Mock<IMessageAttachmentRepository>();
        MessageViewRepositoryMock = new Mock<IMessageViewRepository>();
        ChatReadingModelRepositoryMock = new Mock<IChatReadingModelRepository>();
    }

    private void CustomizeEntities()
    {
        // Регаем билдера для создания сущностей без публичного конструктора (подразумевает наличие оного в принципе)
        AutoFixture.ResidueCollectors.Add(new NonPublicConstructorBuilder());
    }

    protected void SetupUnitOfWorkSuccess()
    {
        UnitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    protected void SetupUnitOfWorkThrowException<TException>()
        where TException : CommonException, new()
    {
        UnitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws<TException>();
    }

    protected void SetupPendingAddEntity<TEntity, TId, TRepository>(Mock<TRepository> repoMock)
        where TEntity : class, IEntity<TId>
        where TId : IIdentifier
        where TRepository : class, IPendingRepository<TEntity, TId>
    {
        repoMock
            .Setup(r => r.PendingAddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
}
