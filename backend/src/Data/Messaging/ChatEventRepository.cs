using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;

namespace FitHub.Data.Messaging;

internal sealed class ChatEventRepository : DefaultPendingRepository<ChatEvent, ChatEventId, DataContext>, IChatEventRepository
{
    public ChatEventRepository(DataContext context) : base(context)
    {
    }
}
