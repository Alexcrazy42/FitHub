using FitHub.Application.Messaging.Commands.Attachments;
using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Messaging.Chats;
using FitHub.Contracts.V1.Messaging.Messages;
using FitHub.Contracts.V1.Messaging.Messages.Attachments;
using FitHub.Domain.Files;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging.Commands;

public static class CommandExtensions
{
    public static CreateMessageCommand ToCommand(this CreateMessageRequest request)
    {
        var chatId = ChatId.Parse(request.ChatId);
        var messageText = ValidationException.ThrowIfNull(request.MessageText);
        return new CreateMessageCommand(chatId, messageText)
        {
            ReplyMessageId = request.ReplyMessageId != null ? MessageId.Parse(request.ReplyMessageId) : null,
            Links = request.Links.Select(CommandExtensions.FromRequest).ToList(),
            Tags = request.Tags.Select(CommandExtensions.FromRequest).ToList(),
            Photos = request.Photos.Select(x => FileId.Parse(x.FileId)).ToList(),
        };
    }

    public static UpdateMessageCommand ToCommand(this UpdateMessageRequest request)
    {
        var messageText = ValidationException.ThrowIfNull(request.MessageText);
        return new UpdateMessageCommand(messageText)
        {
            ReplyMessageId = request.ReplyMessageId != null ? MessageId.Parse(request.ReplyMessageId) : null,
            Links = request.Links.Select(CommandExtensions.FromRequest).ToList(),
            Tags = request.Tags.Select(CommandExtensions.FromRequest).ToList(),
            Photos = request.Photos.Select(x => FileId.Parse(x.FileId)).ToList(),
        };
    }


    public static CreateLinkAttachmentCommand FromRequest(this CreateLinkAttachmentRequest request)
    {
        return new CreateLinkAttachmentCommand(ValidationException.ThrowIfNull(request.Url))
        {
            Title = request.Title,
            Caption = request.Caption,
            PhotoUrl = request.PhotoUrl
        };
    }

    public static CreateChatCommand FromRequest(this CreateChatRequest request)
    {
        var participantUserIds = request.ParticipantUserIds.Select(IdentityUserId.Parse).ToList();
        return new CreateChatCommand(request.Type.Required(), participantUserIds);
    }

    public static InitiatorAndTargetUserCommand FromRequest(this InitiatorAndTargetUserRequest request, IdentityUserId currentUserId)
    {
        return new InitiatorAndTargetUserCommand(ChatId.Parse(request.ChatId), currentUserId, IdentityUserId.Parse(request.TargetUserId));
    }

    public static CreateTagUserAttachmentCommand FromRequest(this CreateTagUserAttachmentRequest request)
    {
        var name = ValidationException.ThrowIfNull(request.Name);
        var type = ValidationException.ThrowIfNull(request.Type);
        var identityUserId = request.TaggedUserId != null ? IdentityUserId.Parse(request.TaggedUserId) : null;
        var command = new CreateTagUserAttachmentCommand(name, type) { TaggetUserId = identityUserId };
        return command;
    }
}
