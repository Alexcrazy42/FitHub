using FitHub.Application.Messaging;
using FitHub.Authentication;
using FitHub.Common.Entities;
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
            .Include(x => x.Participants)
                .ThenInclude(part => part.User);
    }

    public async Task<Chat> GetAsync(ChatId id, CancellationToken ct = default)
    {
        var chat = await ReadRaw()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(chat, "Чат не найден!");
        return chat;
    }

    public Task<Chat?> GetFirstOrDefaultOneToOneChatAsync(List<IdentityUserId> participantUserIds, CancellationToken ct = default)
    {
        return ReadRaw()
            .Where(x => x.Participants.All(part => participantUserIds.Contains(part.UserId)))
            .FirstOrDefaultAsync(ct);
    }
}
