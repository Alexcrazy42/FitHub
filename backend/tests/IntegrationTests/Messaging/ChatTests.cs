using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Messaging.Chats;
using FitHub.IntegrationTests.Infrastructure;
using FitHub.Shared.Messaging;
using Shouldly;
using Xunit;

namespace FitHub.IntegrationTests.Messaging;

public class ChatTests : ControllerTestsBase
{
    public ChatTests(ServerFixture serverFixture) : base(serverFixture)
    {
    }

    [Fact(DisplayName = "Create one to one chat")]
    public async Task CreateChat_ShouldReturnChat()
    {
        // arrange
        List<string> participantIds = [DataSeed.FirstCmsAdminId.ToString(), DataSeed.FirstGymAdminId.ToString()];

        var createChatRequest = new CreateChatRequest
        {
            Type = ChatType.OneToOne,
            ParticipantUserIds = participantIds
        };

        // act
        var chat = await ChatClient.CreateChatAsync(createChatRequest, CancellationToken.None);

        // assert
        chat.ShouldNotBeNull();
        chat.Participants.ShouldNotBeEmpty();
        chat.Participants.Count.ShouldBe(2);
        chat.Participants.Select(x => x.User.Required().Id.Required()).ShouldBe(participantIds, ignoreOrder: true);
    }

    [Fact(DisplayName = "Get chat after creation")]
    public async Task GetChatAfterCreation_ShouldReturnChat()
    {
        // arrange
        List<string> participantIds = [DataSeed.FirstCmsAdminId.ToString(), DataSeed.FirstGymAdminId.ToString()];

        var createChatRequest = new CreateChatRequest
        {
            Type = ChatType.OneToOne,
            ParticipantUserIds = participantIds
        };

        var chatResponse = await ChatClient.CreateChatAsync(createChatRequest, CancellationToken.None);

        // act
        var newChatResponse = await ChatClient.CreateChatAsync(createChatRequest, CancellationToken.None);


        // assert
        newChatResponse.ShouldNotBeNull();
        newChatResponse.Id.ShouldBe(chatResponse.Required().Id);
        newChatResponse.Participants.Count.ShouldBe(chatResponse.Participants.Count);
        chatResponse.Participants.Select(x => x.User.Required().Id.Required()).ShouldBe(participantIds, ignoreOrder: true);
        newChatResponse.Participants.Select(x => x.User.Required().Id.Required()).ShouldBe(participantIds, ignoreOrder: true);
    }
}
