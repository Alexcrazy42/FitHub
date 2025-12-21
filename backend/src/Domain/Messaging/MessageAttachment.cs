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

    #region CommonFields

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }
    public IdentityUserId CreatedById { get; } = null!;
    public User? CreatedBy { get; }
    public IdentityUserId UpdatedById { get; } = null!;
    public User? UpdatedBy { get; }

    #endregion
}
