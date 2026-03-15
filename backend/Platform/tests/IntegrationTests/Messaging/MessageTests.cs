using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Messaging.Chats;
using FitHub.Contracts.V1.Messaging.Messages;
using FitHub.IntegrationTests.Infrastructure;
using FitHub.Shared.Messaging;
using Shouldly;
using Xunit;

namespace FitHub.IntegrationTests.Messaging;

public class MessageTests : ControllerTestsBase
{
    public MessageTests(ServerFixture serverFixture) : base(serverFixture)
    {
    }

    [Fact(DisplayName = "Send message to new chat")]
    public async Task SendMessage_ShouldWorkCorrectly()
    {
        // arrange
        var chat = await CreateChatAsync();

        var createMessageRequest = new CreateMessageRequest
        {
            ChatId = chat.Required().Id,
            MessageText = "Привет"
        };

        // act
        var messageResponse = await MessageClient.CreateMessageAsync(createMessageRequest, CancellationToken.None);

        // assert
        messageResponse.ShouldNotBeNull();
        messageResponse.ChatId.ShouldBe(chat.Required().Id);
        messageResponse.CreatedBy.Required().Id.ShouldBe(DataSeed.FirstCmsAdminId.ToString());
    }

    private async Task<ChatResponse?> CreateChatAsync()
    {
        List<string> participantIds = [DataSeed.FirstCmsAdminId.ToString(), DataSeed.FirstGymAdminId.ToString()];

        var createChatRequest = new CreateChatRequest
        {
            Type = ChatType.OneToOne,
            ParticipantUserIds = participantIds
        };
        return await ChatClient.CreateChatAsync(createChatRequest, CancellationToken.None);
    }
}
