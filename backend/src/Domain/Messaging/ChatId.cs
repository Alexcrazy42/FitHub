using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class ChatId : GuidIdentifier<ChatId>, IIdentifierDescription
{
    public ChatId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Чат";
    public static string Prefix => FormatPrefix("fithub", "chat");
}
