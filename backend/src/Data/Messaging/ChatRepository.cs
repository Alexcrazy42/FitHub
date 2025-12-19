using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Messaging;

internal sealed class ChatRepository : DefaultPendingRepository<Chat, ChatId, DataContext>, IChatRepository
{
    public ChatRepository(DataContext context) : base(context)
    {
    }

    protected override IQueryable<Chat> ReadRaw()
    {
        return base.ReadRaw()
            .Include(x => x.Participants);
    }
}
