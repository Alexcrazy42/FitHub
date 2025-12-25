using AutoFixture;
using AutoFixture.AutoMoq;
using FitHub.Clients.Chats;
using FitHub.Clients.Messages;
using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

[Trait("Category", IntegrationTestCollection.Category)]
[Collection(IntegrationTestCollection.Name)]
public abstract class ControllerTestsBase
{
    protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());

    // Клиенты нашего сервиса
    protected readonly IChatClient ChatClient;
    protected readonly IMessageClient MessageClient;

    // Моки внешних зависимостей
    // protected readonly Mock<IExternalServiceClient> ExternalServiceClientMock;

    protected ControllerTestsBase(ServerFixture serverFixture)
    {
        AutoFixture.Behaviors.Add(new OmitOnRecursionBehavior(recursionDepth: 1));

        ChatClient = serverFixture.ChatClient;
        MessageClient = serverFixture.MessageClient;
    }
}
