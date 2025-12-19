using FitHub.Contracts.V1.Messaging;
using FitHub.Domain.Messaging;
using FitHub.Web.V1.Users;

namespace FitHub.Web.V1.Messaging;

public static class MessagingExtensions
{
    public static List<ChatParticipantResponse> ToResponses(this IReadOnlyList<ChatParticipant> participants)
        => participants.Select(ToResponse).ToList();

    public static ChatResponse ToResponse(this Chat chat)
    {
        return new ChatResponse()
        {
            Id = chat.Id.ToString(),
            Type = chat.Type,
            Participants = chat.Participants.ToResponses()
        };
    }

    public static ChatParticipantResponse ToResponse(this ChatParticipant chatParticipant)
    {
        return new ChatParticipantResponse
        {
            Id = chatParticipant.Id.ToString(),
            ChatId = chatParticipant.ChatId.ToString(),
            User = chatParticipant.User.ToResponse(),
            JoinedAt = chatParticipant.JoinedAt
        };
    }
}
