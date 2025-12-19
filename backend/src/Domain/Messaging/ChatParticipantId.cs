using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class ChatParticipantId : GuidIdentifier<ChatParticipantId>, IIdentifierDescription
{
    public ChatParticipantId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Участник чата";
    public static string Prefix => FormatPrefix("fithub", "chat-participant");
}
