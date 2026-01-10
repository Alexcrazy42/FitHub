using System.ComponentModel;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Domain.Messaging;
using FitHub.Shared.Messaging;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Domain.Messaging;

public sealed class ChatTests : DomainTestsBase
{

    [Fact, DisplayName("Валидные параметры, приватный чат должен создаться")]
    public void CreatePrivateChat_ValidArguments_ShouldCreated()
    {
        // act
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, participants.First().UserId);

        // assert
        chat.Participants.Count.ShouldBe(2);
    }

    [Fact, DisplayName("Попытка создать приватный чат без себя")]
    public void CreatePrivateChat_CurrentUserNotInParticipants_ShouldThrowValidationException()
    {
        // arrange
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, participants.First().UserId);

        // act
        var act = () => chat.SetParticipants(participants, ThirdUserId);

        // assert
        act.ShouldThrow<UnexpectedException>();
    }

    [Theory, DisplayName("Попытка создать приватный чат не на двух человек")]
    [InlineData(1)]
    [InlineData(3)]
    public void CreatePrivateChat_CountParticipantsNotEqualTwo_ShouldThrowValidationException(int participantsCount)
    {
        // arrange
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, participantsCount);

        // act
        var act = () => chat.SetParticipants(participants, ThirdUserId);

        // assert
        act.ShouldThrow<ValidationException>();
    }

    [Fact, DisplayName("Попытка создания группы без участников")]
    public void CreateGroupChat_ZeroParticipants_ShouldThrowValidationException()
    {
        // arrange
        var chat = Chat.Create(ChatType.Group);
        var participants = CreateChatParticipants(chat, 0);

        // act
        var act = () => chat.SetParticipants(participants, ThirdUserId);

        // assert
        act.ShouldThrow<ValidationException>();
    }


    [Fact, DisplayName("Добавление участника, валидные параметры")]
    public void AddParticipant_ValidArguments_ShouldAdded()
    {
        // arrange
        var chat = Chat.Create(ChatType.Group);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, FirstUserId);
        var newParticipant = CreateChatParticipant(chat, ThirdUserId);

        // act
        chat.AddParticipant(newParticipant);

        // assert
        chat.Participants.Count.ShouldBe(3);
    }

    [Fact, DisplayName("Попытка добавления участника в приватный чат")]
    public void AddParticipant_TryAddToOneToOneChat_ShouldThrowValidationException()
    {
        // arrange
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, FirstUserId);
        var newParticipant = CreateChatParticipant(chat, ThirdUserId);

        // act
        var act = () => chat.AddParticipant(newParticipant);

        // should
        act.ShouldThrow<ValidationException>();
    }

    [Fact, DisplayName("Попытка добавить участника в чат с пустыми участниками (ситуация происходит когда не сделали Include или другую подгрузку)")]
    public void AddParticipant_ZeroParticipant_ShouldThrowValidationException()
    {
        // arrange
        var chat = Chat.Create(ChatType.OneToOne);
        var newParticipant = CreateChatParticipant(chat, ThirdUserId);

        // act
        var act = () => chat.AddParticipant(newParticipant);

        // should
        act.ShouldThrow<ValidationException>();
    }

    [Fact, DisplayName("Пользователь имеет доступ к чату")]
    public void ChatHasAccess_ValidArguments_ShouldReturnTrue()
    {
        // arrange
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, FirstUserId);

        // act
        var hasAccess = chat.HasAccess(FirstUserId);

        // arrange
        hasAccess.ShouldBeTrue();
    }

    [Fact, DisplayName("Пользователь имеет доступ к чату")]
    public void ChatHasAccess_NoParticipant_ShouldReturnFalse()
    {
        // arrange
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, FirstUserId);

        // act
        var hasAccess = chat.HasAccess(ThirdUserId);

        // arrange
        hasAccess.ShouldBeFalse();
    }

    [Fact, DisplayName("ПОльзователь не имеет доступа к чату")]
    public void CheckChatAccess_Valid_ShouldOk()
    {
        // arrange
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, FirstUserId);

        // act
        var act = () => chat.CheckAccess(FirstUserId);

        // arrange
        chat.ShouldNotBeNull();
        chat.Participants.Count.ShouldBe(2);
        act.ShouldNotThrow();
    }

    [Fact, DisplayName("ПОльзователь не имеет доступа к чату")]
    public void CheckChatAccess_NoParticipant_ShouldThrowValidationException()
    {
        // arrange
        var chat = Chat.Create(ChatType.OneToOne);
        var participants = CreateChatParticipants(chat, 2);
        chat.SetParticipants(participants, FirstUserId);

        // act
        var act = () => chat.CheckAccess(ThirdUserId);

        // arrange
        act.ShouldThrow<ValidationException>();
    }

    private List<User> CreateUsers(int? count = null)
    {
        List<User> users =
        [
            User.Create(
                FirstUserId,
                "",
                "",
                "",
                "",
                IdentityUserType.CmsAdmin,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow
            ),
            User.Create(
                SecondUserId,
                "",
                "",
                "",
                "",
                IdentityUserType.GymAdmin,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow
            ),
            User.Create(
                ThirdUserId,
                "",
                "",
                "",
                "",
                IdentityUserType.GymAdmin,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow
            )
        ];

        if (count == null)
        {
            return users.ToList();
        }
        return users.Take(count.Value).ToList();
    }

    private List<ChatParticipant> CreateChatParticipants(Chat chat, int count)
    {
        var users = CreateUsers(count);
        return users.Select(user => ChatParticipant.Create(user, chat)).ToList();
    }

    private ChatParticipant CreateChatParticipant(Chat chat, IdentityUserId userId)
    {
        var users = CreateUsers();
        var user = users.First(user => user.Id == userId);
        return ChatParticipant.Create(user, chat);
    }
}
