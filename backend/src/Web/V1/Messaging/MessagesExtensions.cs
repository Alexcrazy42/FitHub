using FitHub.Application.Messaging.Queries;
using FitHub.Common.Entities;
using FitHub.Contracts.V1.Messaging;
using FitHub.Contracts.V1.Messaging.Chats;
using FitHub.Contracts.V1.Messaging.Messages;
using FitHub.Domain.Messaging;
using FitHub.Web.V1.Users;

namespace FitHub.Web.V1.Messaging;

public static class MessagesExtensions
{
    public static List<ChatParticipantResponse> ToResponses(this IReadOnlyList<ChatParticipant> participants)
        => participants.Select(ToResponse).ToList();

    public static ChatResponse ToResponse(this Chat chat)
    {
        return new ChatResponse()
        {
            Id = chat.Id.ToString(),
            Type = chat.Type,
            Participants = chat.Participants.ToResponses(),
            Name = chat.Name
        };
    }

    public static ChatParticipantResponse ToResponse(this ChatParticipant chatParticipant)
    {
        return new ChatParticipantResponse
        {
            Id = chatParticipant.Id.ToString(),
            ChatId = chatParticipant.ChatId.ToString(),
            User = chatParticipant.User.ToResponse(),
            JoinedAt = chatParticipant.JoinedAt
        };
    }

    public static MessageResponse ToResponse(this Message message)
    {
        return new MessageResponse()
        {
            Id = message.Id.ToString(),
            ChatId = message.ChatId.ToString(),
            MessageText = message.MessageText,
            ReplyMessage = message.ReplyMessage?.ToResponse(),
            ForwardedMessage = message.ReplyMessage?.ToResponse(),
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            CreatedBy = message.CreatedBy?.ToResponse(),
            UpdatedBy = message.UpdatedBy?.ToResponse(),
            Attachments = message.Attachments.Select(ToResponse).ToList()
        };
    }

    public static MessageAttachmentResponse ToResponse(this MessageAttachment attachment)
    {
        return new MessageAttachmentResponse
        {
            Id = attachment.Id.ToString(),
            MessageId = attachment.MessageId.ToString(),
            Type = attachment.Type,
            Data = attachment.Data,
            CreatedAt = attachment.CreatedAt,
            UpdatedAt = attachment.UpdatedAt,
            CreatedBy = attachment.CreatedBy?.ToResponse(),
            UpdatedBy = attachment.UpdatedBy?.ToResponse()
        };
    }

    public static ChatMessageResponse ToResponse(this ChatReadingModel model)
    {
        return new ChatMessageResponse
        {
            Id = model.Id.ToString(),
            Chat = model.Chat.ToResponse(),
            LastMessage = model.LastMessage.ToResponse(),
            LastMessageTime = model.LastMessageTime,
            UnreadCount = model.UnreadCount
        };
    }

    public static GetMessagesQuery ToQuery(this GetMessagesRequest? request)
    {
        ValidationException.ThrowIfNull(request, "request != null");

        return new GetMessagesQuery(
            chatId: ChatId.Parse(ValidationException.ThrowIfNull(request.ChatId)),
            isDescending: ValidationException.ThrowIfNull(request.IsDescending),
            loadLastMessages: ValidationException.ThrowIfNull(request.LoadLastMessages),
            fromUnread: ValidationException.ThrowIfNull(request.FromUnread),
            from: request.From
        );
    }
}
