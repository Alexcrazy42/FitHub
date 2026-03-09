using FitHub.Application.Messaging.Commands.Attachments;
using FitHub.Domain.Files;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging.Commands;

public class CreateMessageCommand
{
    public ChatId ChatId { get; init; }

    public string MessageText { get; init; }

    public MessageId? ReplyMessageId { get; init; }

    public List<CreateLinkAttachmentCommand> Links { get; init; } = [];

    public List<CreateTagUserAttachmentCommand> Tags { get; init; } = [];

    public List<FileId> Photos { get; init; } = [];

    public List<CreateStickerAttachmentCommand> Stickers { get; init; } = [];

    public List<CreateDocumentAttachmentCommand> Documents { get; init; } = [];

    public List<CreateVoiceAttachmentCommand> Voices { get; init; } = [];

    public CreateMessageCommand(ChatId chatId, string messageText)
    {
        ChatId = chatId;
        MessageText = messageText;
    }
}
