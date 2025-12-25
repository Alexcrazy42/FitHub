using FitHub.Contracts.V1.Messaging.Chats;
using FitHub.IntegrationTests.Infrastructure;
using Xunit;

namespace FitHub.IntegrationTests.Messaging;

public class ChatTests : ControllerTestsBase
{
    public ChatTests(ServerFixture serverFixture) : base(serverFixture)
    {
    }

    [Fact]
    public Task CreateChat_ShouldReturnChat()
    {
        throw new NotImplementedException();
    }
}
