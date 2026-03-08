using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Common.Json;
using FitHub.Domain.Messaging.Attachments;
using FitHub.Shared.Messaging;

namespace FitHub.Domain.Messaging;

public class MessageAttachment : IEntity<MessageAttachmentId>, IUserAuditableEntity<IdentityUserId, User>
{
    private Message? message;

    private MessageAttachment(MessageAttachmentId id, MessageId messageId, MessageAttachmentType type, string data)
    {
        Id = id;
        MessageId = messageId;
        Type = type;
        Data = data;
    }

    public MessageAttachmentId Id { get; }

    public MessageId MessageId { get; private set; }

    public Message Message
    {
        get => UnexpectedException.ThrowIfNull(message, "Сообщение неожиданно оказалось null");
        private set => message = value;
    }

    // помимо типа вложения можно еще хранить версию этого вложения, чтобы фронт мог производить версионирование
    public MessageAttachmentType Type { get; private set; }

    /// <summary>
    /// ИД сущности, на которую будем ссылаться внутри json data
    /// </summary>
    /// <remarks>
    /// Может быть не для всех. Например, некоторые сущности вообще не будут обновляться: фотографии, ссылки. тк не будут иметь своей доп таблички
    /// В отличии от опроса, который 100% будет иметь свою доп табличку, куда будут записывать реальные данные, а в Read-Model поступать потом
    /// (в нашем случае сразу, тк это будет в рамках одной транзакции, аналогично для модульного монолита, для микросервисов - передача через Outbox в соседний сервис)
    /// </remarks>
    public string? EntityId { get; private set; }

    /// <summary>
    /// JSON Read-Model для Backend-Driven-UI
    /// </summary>
    public string Data { get; private set; }

    public static MessageAttachment CreatePhotoAttachment(Message message, PhotoAttachment photoAttachment)
    {
        return new MessageAttachment(MessageAttachmentId.New(), message.Id, MessageAttachmentType.Photo, CommonJsonSerializer.Serialize(photoAttachment));
    }

    public static MessageAttachment CreateTagUserAttachment(Message message, TagUserAttachment tagUserAttachment)
    {
        return new MessageAttachment(MessageAttachmentId.New(), message.Id, MessageAttachmentType.TagUser, CommonJsonSerializer.Serialize(tagUserAttachment));
    }

    public static MessageAttachment CreateLinkAttachment(Message message, LinkAttachment linkAttachment)
    {
        return new MessageAttachment(MessageAttachmentId.New(), message.Id, MessageAttachmentType.Link, CommonJsonSerializer.Serialize(linkAttachment));
    }

    public static MessageAttachment CreateInviteOrExcludeUserAttachment(Message message,
        MessageAttachmentType type,
        InitiatorAndTargetUserActionAttachment attachment)
    {
        if (type != MessageAttachmentType.InviteUser && type != MessageAttachmentType.ExcludeUser)
        {
            throw new ValidationException("Операция должна быть на добавление или исключения пользователя!");
        }
        return new MessageAttachment(MessageAttachmentId.New(), message.Id, type, CommonJsonSerializer.Serialize(attachment));
    }

    public static MessageAttachment CreateGroupCreatedAttachment(Message message)
    {
        return new MessageAttachment(MessageAttachmentId.New(), message.Id, MessageAttachmentType.CreateGroup, CommonJsonSerializer.Serialize(new { }));
    }

    public static MessageAttachment CreateStickerAttachment(Message message, StickerAttachment stickerAttachment)
    {
        return new MessageAttachment(MessageAttachmentId.New(), message.Id, MessageAttachmentType.Sticker, CommonJsonSerializer.Serialize(stickerAttachment));
    }

    public static MessageAttachment CreateDocumentAttachment(Message message, DocumentAttachment documentAttachment)
    {
        return new MessageAttachment(MessageAttachmentId.New(), message.Id, MessageAttachmentType.Document, CommonJsonSerializer.Serialize(documentAttachment));
    }

    #region CommonFields

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }
    public IdentityUserId CreatedById { get; } = null!;
    public User? CreatedBy { get; }
    public IdentityUserId UpdatedById { get; } = null!;
    public User? UpdatedBy { get; }

    #endregion
}
