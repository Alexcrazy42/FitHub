using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class MessageId : GuidIdentifier<MessageId>, IIdentifierDescription
{
    public MessageId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Событие в чате";
    public static string Prefix => FormatPrefix("fithub", "chat-event");
}
