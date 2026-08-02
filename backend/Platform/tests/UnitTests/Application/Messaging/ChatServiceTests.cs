using System.ComponentModel;
using FitHub.Application.Messaging;
using FitHub.Application.Messaging.Commands;
using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Domain.Messaging;
using FitHub.Shared.Messaging;
using Moq;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Application.Messaging;

public sealed class ChatServiceTests : ApplicationTestsBase
{
    private readonly ChatService sut;

    public ChatServiceTests()
    {
        sut = new ChatService(
            ChatRepositoryMock.Object,
            UnitOfWorkMock.Object,
            CurrentIdentityUserIdAccessorMock.Object,
            UserServiceMock.Object,
            MessageRepositoryMock.Object,
            MessageAttachmentRepositoryMock.Object,
            MessageViewRepositoryMock.Object,
            ChatReadingModelRepositoryMock.Object);
    }

    [Fact, DisplayName("Получить чат, валидные параметры")]
    public async Task GetChatAsync_ShouldWorkCorrectly()
    {
        // arrange
        var chat = CreateOneToOneChatWithTwoParticipants();

        ChatRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<ChatId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        SetupCurrentUser(FirstUserId);

        var chatId = GetRandomChatId();

        // act
        var result = await sut.GetChatAsync(chatId, CancellationToken.None);

        // assert
        result.Id.Value.ShouldBe(chat.Id.Value);
        result.Participants.Count.ShouldBe(chat.Participants.Count);

        ChatRepositoryMock.Verify(
            r => r.GetAsync(It.IsAny<ChatId>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact, DisplayName("Получить чат, не состоит в чате")]
    public async Task GetChatAsync_NotInParticipants_ShouldThrowValidationException()
    {
        // arrange
        var chat = CreateOneToOneChatWithTwoParticipants();

        ChatRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<ChatId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        SetupCurrentUser(ThirdUserId);

        var chatId = GetRandomChatId();

        // act
        Func<Task> act = () => sut.GetChatAsync(chatId, CancellationToken.None);

        // assert
        await act.ShouldThrowAsync<ValidationException>();

        ChatRepositoryMock.Verify(
            r => r.GetAsync(It.IsAny<ChatId>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact, DisplayName("Создание приватного чата, чат уже существует")]
    public async Task CreateChat_ChatAlreadyExists_ShouldReturnExistingChat()
    {
        // arrange
        var chat = CreateOneToOneChatWithTwoParticipants();

        SetupChatExists(chat);

        var command = new CreateChatCommand(ChatType.OneToOne, [FirstUserId, SecondUserId]);

        // act
        var result = await sut.CreateChatAsync(command, CancellationToken.None);

        // assert
        result.Id.Value.ShouldBe(chat.Id.Value);
        result.Participants.Count.ShouldBe(chat.Participants.Count);

        ChatRepositoryMock.Verify(
            r => r.GetFirstOrDefaultOneToOneChatAsync(
                It.IsAny<List<IdentityUserId>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        ChatRepositoryMock.Verify(
            r => r.PendingAddAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()),
            Times.Never);

        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact, DisplayName("Создание приватного чата, валидные параметры")]
    public async Task CreatePrivateChat_Valid_ShouldCreateChat()
    {
        // arrange
        var users = CreateUsers(2).ToArray();

        SetupChatNotExists();
        SetupCurrentUser(FirstUserId);
        SetupUsers(users);
        SetupPendingAddEntity<Chat, ChatId, IChatRepository>(ChatRepositoryMock);
        SetupUnitOfWorkSuccess();

        var command = new CreateChatCommand(ChatType.OneToOne, [FirstUserId, SecondUserId]);

        // act
        var result = await sut.CreateChatAsync(command, CancellationToken.None);

        // assert
        result.Type.ShouldBe(ChatType.OneToOne);
        result.Name.ShouldBeNull();
        result.Participants.Count.ShouldBe(2);

        ChatRepositoryMock.Verify(
            r => r.PendingAddAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()),
            Times.Once);

        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact, DisplayName("Создание группового чата, валидные параметры")]
    public async Task CreateGroupChat_Valid_ShouldCreateChat()
    {
        // arrange
        var users = CreateUsers(2).ToArray();

        SetupChatNotExists();
        SetupCurrentUser(FirstUserId);
        SetupUsers(users);
        SetupPendingAddEntity<Chat, ChatId, IChatRepository>(ChatRepositoryMock);
        SetupUnitOfWorkSuccess();

        var command = new CreateChatCommand(ChatType.Group, [FirstUserId, SecondUserId]);

        // act
        var result = await sut.CreateChatAsync(command, CancellationToken.None);

        // assert
        result.Type.ShouldBe(ChatType.Group);
        result.Name.ShouldNotBeNull();
        result.Name.ShouldBe("name1, name2");
        result.Participants.Count.ShouldBe(2);

        ChatRepositoryMock.Verify(
            r => r.PendingAddAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()),
            Times.Once);

        MessageRepositoryMock.Verify(
            r => r.PendingAddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()),
            Times.Once);

        MessageAttachmentRepositoryMock.Verify(
            r => r.PendingAddAsync(It.IsAny<MessageAttachment>(), It.IsAny<CancellationToken>()),
            Times.Once);

        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact, DisplayName("Создание группового чата с тремя участниками, валидные параметры")]
    public async Task CreateGroupChat_ValidWithThreeParticipants_ShouldCreateChat()
    {
        // arrange
        var users = CreateUsers(3).ToArray();

        SetupChatNotExists();
        SetupCurrentUser(FirstUserId);
        SetupUsers(users);
        SetupPendingAddEntity<Chat, ChatId, IChatRepository>(ChatRepositoryMock);
        SetupUnitOfWorkSuccess();

        var command = new CreateChatCommand(ChatType.Group, [FirstUserId, SecondUserId, ThirdUserId]);

        // act
        var result = await sut.CreateChatAsync(command, CancellationToken.None);

        // assert
        result.Type.ShouldBe(ChatType.Group);
        result.Name.ShouldNotBeNull();
        result.Name.ShouldBe("name1, name2 + 1");
        result.Participants.Count.ShouldBe(3);

        ChatRepositoryMock.Verify(
            r => r.PendingAddAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()),
            Times.Once);

        MessageRepositoryMock.Verify(
            r => r.PendingAddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()),
            Times.Once);

        MessageAttachmentRepositoryMock.Verify(
            r => r.PendingAddAsync(It.IsAny<MessageAttachment>(), It.IsAny<CancellationToken>()),
            Times.Once);

        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // --------- helpers ---------

    private ChatId GetRandomChatId()
    {
        return ChatId.Parse(Guid.NewGuid());
    }

    private Chat CreateOneToOneChatWithTwoParticipants()
    {
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, FirstUserId);
        return chat;
    }

    private Chat CreateGroupChatWithTwoParticipants()
    {
        var chat = Chat.Create(ChatType.Group);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, participants.First().UserId);
        return chat;
    }

    private List<User> CreateUsers(int count)
    {
        List<IdentityUserId> userIds = [FirstUserId, SecondUserId, ThirdUserId];

        var users = new List<User>();
        for (var i = 1; i <= count; i++)
        {
            var userId = i < 3 ? userIds[i] : throw new Exception("Нельзя запросить больше 3 пользователей");
            users.Add(User.Create(
                userId,
                $"surname{i}",
                $"name{i}",
                "", "", IdentityUserType.GymAdmin,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow
            ));
        }
        return users;
    }

    private List<ChatParticipant> CreateChatParticipants(Chat chat, int count)
    {
        var users = CreateUsers(count);
        return users.Select(user => ChatParticipant.Create(user, chat)).ToList();
    }

    private void SetupCurrentUser(IdentityUserId userId)
    {
        CurrentIdentityUserIdAccessorMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);
    }

    private void SetupChatNotExists()
    {
        ChatRepositoryMock
            .Setup(r => r.GetFirstOrDefaultOneToOneChatAsync(
                It.IsAny<List<IdentityUserId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Chat?)null);
    }

    private void SetupChatExists(Chat chat)
    {
        ChatRepositoryMock
            .Setup(r => r.GetFirstOrDefaultOneToOneChatAsync(
                It.IsAny<List<IdentityUserId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);
    }

    private void SetupUsers(IReadOnlyList<User> users)
    {
        UserServiceMock
            .Setup(s => s.GetUsersAsync(
                It.IsAny<List<IdentityUserId>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);
    }
}
