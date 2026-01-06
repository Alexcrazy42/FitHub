using FitHub.Application.Messaging;
using FitHub.Authentication;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Messaging;

internal sealed class MessageViewRepository : DefaultPendingRepository<MessageView, MessageViewId, DataContext>, IMessageViewRepository
{
    public MessageViewRepository(DataContext context) : base(context)
    {
    }

    public async Task<Message?> GetFirstUnreadMessageAsync(ChatId chatId, IdentityUserId userId, CancellationToken ct)
    {
        var entity = await ReadRaw()
            .Include(x => x.Message)
            .Where(x => x.Message.ChatId == chatId)
            .Where(x => x.ViewedAt == null)
            .OrderBy(x => x.Message.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return entity?.Message;
    }
}
