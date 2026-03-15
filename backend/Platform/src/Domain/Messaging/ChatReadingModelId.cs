using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class ChatReadingModelId : GuidIdentifier<ChatReadingModelId>, IIdentifierDescription
{
    public ChatReadingModelId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Read-Model для чата";
    public static string Prefix => FormatPrefix("fithub", "chat-read-model");
}
