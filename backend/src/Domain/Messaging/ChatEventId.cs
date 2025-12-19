using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class ChatEventId : GuidIdentifier<ChatEventId>, IIdentifierDescription
{
    public ChatEventId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Событие в чате";
    public static string Prefix => FormatPrefix("fithub", "chat-event");
}
