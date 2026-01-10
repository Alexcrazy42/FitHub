using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;

namespace FitHub.Data.Messaging;

internal sealed class ChatParticipantRepository : DefaultPendingRepository<ChatParticipant, ChatParticipantId, DataContext>, IChatParticipantRepository
{
    public ChatParticipantRepository(DataContext context) : base(context)
    {
    }
}
