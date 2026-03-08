using FitHub.Application.Common;
using FitHub.Application.Files;
using FitHub.Application.Messaging.Commands;
using FitHub.Application.Messaging.Commands.Attachments;
using FitHub.Application.Messaging.Queries;
using FitHub.Application.Users;
using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Common.Utilities.System;
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
    private readonly IChatReadingModelRepository chatReadingModelRepository;
    private readonly IMessageViewRepository messageViewRepository;

    public MessageService(IMessageRepository messageRepository,
        IChatService chatService,
        ICurrentIdentityUserIdAccessor userIdAccessor,
        IMessageAttachmentRepository messageAttachmentRepository,
        IUnitOfWork unitOfWork,
        IUserService userService,
        IFileService fileService,
        IChatReadingModelRepository chatReadingModelRepository,
        IMessageViewRepository messageViewRepository)
    {
        this.messageRepository = messageRepository;
        this.chatService = chatService;
        this.userIdAccessor = userIdAccessor;
        this.messageAttachmentRepository = messageAttachmentRepository;
        this.unitOfWork = unitOfWork;
        this.userService = userService;
        this.fileService = fileService;
        this.chatReadingModelRepository = chatReadingModelRepository;
        this.messageViewRepository = messageViewRepository;
    }

    public async Task<Message> GetMessageAsync(MessageId messageId, CancellationToken ct)
    {
        var message = await messageRepository.GetMessageAsync(messageId, ct);
        var chat = await chatService.GetChatAsync(message.ChatId, ct);
        chat.CheckAccess(userIdAccessor.GetCurrentUserId());
        return message;
    }

    public async Task<IReadOnlyList<Message>> GetMessagesAsync(GetMessagesQuery messageQuery, PagedQuery paged, CancellationToken ct)
    {
        var chat = await chatService.GetChatAsync(messageQuery.ChatId, ct);
        chat.CheckAccess(userIdAccessor.GetCurrentUserId());

        if (!messageQuery.FromUnread)
        {
            return await messageRepository.GetMessagesAsync(messageQuery, paged, ct);
        }

        var firstUnreadMessage = await messageViewRepository.GetFirstUnreadMessageAsync(messageQuery.ChatId, userIdAccessor.GetCurrentUserId(), ct);

        if (firstUnreadMessage == null)
        {
            return await messageRepository.GetMessagesAsync(messageQuery, paged, ct);
        }

        var newGetMessageQuery = new GetMessagesQuery(messageQuery.ChatId,
            isDescending: false,
            loadLastMessages: false,
            fromUnread: false,
            firstUnreadMessage.CreatedAt);
        return await messageRepository.GetMessagesAsync(newGetMessageQuery, paged, ct);

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
        await AttachStickersAsync(message, command.Stickers, ct);
        await AttachDocumentsAsync(message, command.Documents, ct);

        await CreateMessageViews(chat, message, ct);
        await UpdateReadModelsAfterCreationMessageAsync(chat, message, ct);

        await messageRepository.PendingAddAsync(message, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return message;
    }

    public async Task<Message> UpdateMessageAsync(MessageId id, UpdateMessageCommand command, CancellationToken ct)
    {
        var message = await GetMessageAsync(id, ct);

        var replyMessage = await GetReplyMessageIfNeededAsync(command.ReplyMessageId, ct);
        message.SetReplyMessage(replyMessage);
        message.SetMessageText(command.MessageText);

        messageAttachmentRepository.PendingRemoveRange(message.Attachments);

        await AttachLinksAsync(message, command.Links, ct);
        await AttachTagsAsync(message, command.Tags, ct);
        await AttachPhotosAsync(message, command.Photos, ct);
        await AttachStickersAsync(message, command.Stickers, ct);

        await unitOfWork.SaveChangesAsync(ct);
        return message;
    }

    public async Task<Message> DeleteAsync(MessageId messageId, CancellationToken ct)
    {
        var message = await GetMessageAsync(messageId, ct);
        message.CheckAccess(userIdAccessor.GetCurrentUserId());
        messageRepository.PendingRemove(message);

        await UpdateReadModelsAfterDeletingMessageAsync(message.Chat, message, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return message;
    }

    public Task<PagedResult<ChatReadingModel>> GetChatReadingsAsync(PagedQuery paged, CancellationToken ct)
    {
        return chatReadingModelRepository.GetChatReadingModelAsync(paged, userIdAccessor.GetCurrentUserId(), ct);
    }

    public async Task ReadMessagesAsync(MessageId messageId, CancellationToken ct)
    {
        var currentUserId = userIdAccessor.GetCurrentUserId();

        var message = await GetMessageAsync(messageId, ct);

        var unreadMessagesOlder = await messageRepository.GetUnreadMessagesOlderThan(message, currentUserId, ct);

        var readModel = await chatReadingModelRepository.GetFirstOrDefaultAsync(x => x.ChatId == message.ChatId && x.UserId == currentUserId, ct);
        UnexpectedException.ThrowIfNull(readModel, "Не смогли найти readModel");

        var firstUnreadMessage = unreadMessagesOlder.OrderBy(x => x.CreatedAt).First();

        readModel.UpdateLastMessageAndUnreadCount(message, firstUnreadMessage, readModel.UnreadCount - unreadMessagesOlder.Count);

        var messageIds = unreadMessagesOlder.Select(x => x.Id).ToList();
        var messageViews = await messageViewRepository.GetAllAsync(x => messageIds.Contains(x.MessageId) && x.UserId == currentUserId, ct);

        foreach (var messageView in messageViews)
        {
            messageView.SetViewedAt(DateTimeOffset.UtcNow);
        }

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

    private async Task AttachStickersAsync(Message message, IEnumerable<CreateStickerAttachmentCommand> stickers, CancellationToken ct)
    {
        foreach (var sticker in stickers)
        {
            var stickerAttachment = new StickerAttachment(sticker.StickerId, sticker.FileId, sticker.Name);
            var messageAttachment = MessageAttachment.CreateStickerAttachment(message, stickerAttachment);
            await messageAttachmentRepository.PendingAddAsync(messageAttachment, ct);
        }
    }

    private async Task AttachDocumentsAsync(Message message, IEnumerable<CreateDocumentAttachmentCommand> documents, CancellationToken ct)
    {
        foreach (var doc in documents)
        {
            await fileService.GetFile(doc.FileId, ct);
            var documentAttachment = new DocumentAttachment(doc.FileId, doc.FileName, doc.FileSize, doc.MimeType);
            var messageAttachment = MessageAttachment.CreateDocumentAttachment(message, documentAttachment);
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

    private async Task CreateMessageViews(Chat chat, Message message, CancellationToken ct)
    {
        foreach (var participant in chat.Participants)
        {
            var messageView = MessageView.Create(message, participant.User);
            await messageViewRepository.PendingAddAsync(messageView, ct);
        }
    }

    private async Task UpdateReadModelsAfterCreationMessageAsync(Chat chat, Message message, CancellationToken ct)
    {
        var currentUserId = userIdAccessor.GetCurrentUserId();

        var userIds = chat.Participants
            .Select(p => p.UserId)
            .Where(x => x != currentUserId)
            .ToList();
        var chatReadModels = await chatReadingModelRepository.GetAllAsync(x => x.ChatId == chat.Id && userIds.Contains(x.UserId), ct);

        foreach (var model in chatReadModels)
        {
            model.UpdateLastMessageAndIncrement(message);
        }

        if (chatReadModels.Count != userIds.Count)
        {
            foreach (var user in chat.Participants.Select(x => x.User).Where(x => x.Id != currentUserId).ToList())
            {
                var chatReadModel = ChatReadingModel.Create(chat, user, message, message, 1);
                await chatReadingModelRepository.PendingAddAsync(chatReadModel, ct);
            }
        }


        var currentUserChatReadModel = await chatReadingModelRepository.GetFirstOrDefaultAsync(x => x.ChatId == chat.Id && x.UserId == currentUserId, ct);
        if (currentUserChatReadModel == null)
        {
            var creator = chat.Participants.Select(x => x.User).Single(x => x.Id == currentUserId);
            var chatReadModel = ChatReadingModel.Create(chat, creator, message, message, 0);
            await chatReadingModelRepository.PendingAddAsync(chatReadModel, ct);
        }
        else
        {
            currentUserChatReadModel.UpdateLastMessageAndUnreadCount(message, message, 0);
        }
    }

    private async Task UpdateReadModelsAfterDeletingMessageAsync(Chat chat, Message message, CancellationToken ct)
    {
        var userIds = chat.Participants.Select(p => p.UserId).ToList();
        var readModels = await chatReadingModelRepository.GetAllAsync(x => x.ChatId == chat.Id && userIds.Contains(x.UserId), ct);

        foreach (var readModel in readModels)
        {
            if (readModel.FirstMessageTime >= message.CreatedAt
                && readModel.LastMessageTime <= message.CreatedAt)
            {
                readModel.UpdateUnreadCount(readModel.UnreadCount - 1);
            }
        }
    }
}
