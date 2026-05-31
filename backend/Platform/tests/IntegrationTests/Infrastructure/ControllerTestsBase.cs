using AutoFixture;
using AutoFixture.AutoMoq;
using FitHub.Authentication;
using FitHub.Clients.Chats;
using FitHub.Clients.Messages;
using FitHub.Common.TestsCommon;
using Moq;
using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

[Trait("Category", IntegrationTestCollection.Category)]
[Collection(IntegrationTestCollection.Name)]
public abstract class ControllerTestsBase : IDisposable
{
    protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());

    protected readonly CurrentUserProvider CurrentUserProvider;

    // Клиенты апи
    protected readonly IChatClient ChatClient;
    protected readonly IMessageClient MessageClient;

    // Моки зависимостей
    protected readonly Mock<IIdentityUserService> IdentityUserServiceMock;

    protected ControllerTestsBase(ServerFixture serverFixture)
    {
        AutoFixture.Behaviors.Add(new OmitOnRecursionBehavior(recursionDepth: 1));

        CurrentUserProvider = serverFixture.CurrentUserProvider;

        ChatClient = serverFixture.ChatClient;
        MessageClient = serverFixture.MessageClient;

        IdentityUserServiceMock = serverFixture.IdentityUserServiceMock;

        CustomizeEntities();
    }

    public void CustomizeEntities()
    {
        AutoFixture.ResidueCollectors.Add(new NonPublicConstructorBuilder());
    }

    public void Dispose()
    {
        // здесь добавляем Reset моков, чтобы разные тесты друг-друга не аффектили
        IdentityUserServiceMock.Reset();
    }
}
