using AutoFixture;
using AutoFixture.AutoMoq;
using FitHub.Authentication;
using FitHub.Clients.Chats;
using FitHub.Clients.Messages;
using FitHub.TestsCommon;
using Moq;
using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

[Trait("Category", IntegrationTestCollection.Category)]
[Collection(IntegrationTestCollection.Name)]
public abstract class ControllerTestsBase
{
    protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());

    protected readonly DataSeed DataSeed;
    protected readonly CurrentUserProvider CurrentUserProvider;

    // Клиенты апи
    protected readonly IChatClient ChatClient;
    protected readonly IMessageClient MessageClient;

    // Моки зависимостей
    protected readonly Mock<IIdentityUserService> IdentityUserServiceMock;

    protected ControllerTestsBase(ServerFixture serverFixture)
    {
        AutoFixture.Behaviors.Add(new OmitOnRecursionBehavior(recursionDepth: 1));

        DataSeed = serverFixture.DataSeed;
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
}
