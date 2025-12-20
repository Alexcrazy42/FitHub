using FitHub.Application.Common;
using FitHub.Application.Files;
using FitHub.Application.Messaging.Commands;
using FitHub.Application.Messaging.Commands.Attachments;
using FitHub.Application.Users;
using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Files;
using FitHub.Domain.Messaging;
using FitHub.Domain.Messaging.Attachments;

namespace FitHub.Application.Messaging;

internal sealed class MessageService : IMessageService
{
    private readonly IMessageRepository messageRepository;
    private readonly IMessageAttachmentRepository messageAttachmentRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IChatService chatService;
    private readonly ICurrentIdentityUserIdAccessor userIdAccessor;
    private readonly IUserService userService;
    private readonly IFileService fileService;

    public MessageService(IMessageRepository messageRepository, IChatService chatService, ICurrentIdentityUserIdAccessor userIdAccessor, IMessageAttachmentRepository messageAttachmentRepository, IUnitOfWork unitOfWork, IUserService userService, IFileService fileService)
    {
        this.messageRepository = messageRepository;
        this.chatService = chatService;
        this.userIdAccessor = userIdAccessor;
        this.messageAttachmentRepository = messageAttachmentRepository;
        this.unitOfWork = unitOfWork;
        this.userService = userService;
        this.fileService = fileService;
    }

    public async Task<Message> GetMessageAsync(MessageId messageId, CancellationToken ct)
    {
        var message = await messageRepository.GetMessageAsync(messageId, ct);
        message.Chat.CheckAccess(userIdAccessor.GetCurrentUserId());
        return message;
    }

    public async Task<IReadOnlyList<Message>> GetMessagesAsync(ChatId chatId, PagedQuery paged, CancellationToken ct)
    {
        var chat = await chatService.GetChatAsync(chatId, ct);
        chat.CheckAccess(userIdAccessor.GetCurrentUserId());

        return await messageRepository.GetMessagesAsync(chat.Id, paged, ct);
    }

    public async Task<Message> CreateMessageAsync(CreateMessageCommand command, CancellationToken ct)
    {
        var chat = await chatService.GetChatAsync(command.ChatId, ct);
        chat.CheckAccess(userIdAccessor.GetCurrentUserId());

        var replyMessage = await GetReplyMessageIfNeededAsync(command.ReplyMessageId, ct);
        var message = Message.Create(chat, command.MessageText, replyMessage);

        await AttachLinksAsync(message, command.Links, ct);
        await AttachTagsAsync(message, command.Tags, ct);
        await AttachPhotosAsync(message, command.Photos, ct);

        await messageRepository.PendingAddAsync(message, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return message;
    }

    public async Task<Message> UpdateMessageAsync(MessageId id, UpdateMessageCommand command, CancellationToken ct)
    {
        var message = await GetMessageAsync(id, ct);
        message.Chat.CheckAccess(userIdAccessor.GetCurrentUserId());

        var replyMessage = await GetReplyMessageIfNeededAsync(command.ReplyMessageId, ct);
        message.SetReplyMessage(replyMessage);
        message.SetMessageText(command.MessageText);

        messageAttachmentRepository.PendingRemoveRange(message.Attachments);

        await AttachLinksAsync(message, command.Links, ct);
        await AttachTagsAsync(message, command.Tags, ct);
        await AttachPhotosAsync(message, command.Photos, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return message;
    }

    public async Task DeleteAsync(MessageId messageId, CancellationToken ct)
    {
        var message = await GetMessageAsync(messageId, ct);
        messageRepository.PendingRemove(message);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Message?> GetReplyMessageIfNeededAsync(MessageId? replyMessageId, CancellationToken ct)
    {
        if (replyMessageId == null)
        {
            return null;
        }

        return await GetMessageAsync(replyMessageId, ct);
    }

    private async Task AttachLinksAsync(Message message, IEnumerable<CreateLinkAttachmentCommand> links, CancellationToken ct)
    {
        foreach (var link in links)
        {
            var linkAttachment = new LinkAttachment(link.Url, link.Title, link.Caption, link.PhotoUrl);
            var messageAttachment = MessageAttachment.CreateLinkAttachment(message, linkAttachment);
            await messageAttachmentRepository.PendingAddAsync(messageAttachment, ct);
        }
    }

    private async Task AttachTagsAsync(Message message, IEnumerable<CreateTagUserAttachmentCommand> tags, CancellationToken ct)
    {
        foreach (var tag in tags)
        {
            var taggedUser = await GetTaggedUserIfNeededAsync(tag.TaggetUserId, ct);
            var tagAttachment = new TagUserAttachment(tag.Name, tag.Type, taggedUser?.Id);
            var messageAttachment = MessageAttachment.CreateTagUserAttachment(message, tagAttachment);
            await messageAttachmentRepository.PendingAddAsync(messageAttachment, ct);
        }
    }

    private async Task AttachPhotosAsync(Message message, IEnumerable<FileId> photoIds, CancellationToken ct)
    {
        foreach (var photoId in photoIds)
        {
            var file = await fileService.GetFile(photoId, ct);
            var photoAttachment = new PhotoAttachment(file.Id);
            var messageAttachment = MessageAttachment.CreatePhotoAttachment(message, photoAttachment);
            await messageAttachmentRepository.PendingAddAsync(messageAttachment, ct);
        }
    }

    private async Task<User?> GetTaggedUserIfNeededAsync(IdentityUserId? taggedUserId, CancellationToken ct)
    {
        if (taggedUserId == null)
        {
            return null;
        }

        return await userService.GetUserAsync(taggedUserId, ct);
    }

}
