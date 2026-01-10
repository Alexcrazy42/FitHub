using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class MessageViewId : GuidIdentifier<MessageViewId>, IIdentifierDescription
{
    public MessageViewId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Просмотр сообщения";
    public static string Prefix => FormatPrefix("fithub", "message-view");
}
