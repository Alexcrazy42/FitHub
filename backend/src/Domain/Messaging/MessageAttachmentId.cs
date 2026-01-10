using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class MessageAttachmentId : GuidIdentifier<MessageAttachmentId>, IIdentifierDescription
{
    public MessageAttachmentId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Приложение к сообщению";
    public static string Prefix => FormatPrefix("fithub", "message-attachment");
}
