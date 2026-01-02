using FitHub.Application.Common;
using FitHub.Application.Messaging;
using FitHub.Authentication;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Messaging;

internal sealed class ChatReadingModelRepository : DefaultPendingRepository<ChatReadingModel, ChatReadingModelId, DataContext>, IChatReadingModelRepository
{
    public ChatReadingModelRepository(DataContext context) : base(context)
    {
    }

    public async Task<PagedResult<ChatReadingModel>> GetChatReadingModelAsync(PagedQuery paged, IdentityUserId userId, CancellationToken ct)
    {
        var dbQuery = ReadRaw()
            .Where(x => x.UserId == userId)
            .AsQueryable();

        var totalCount = await dbQuery.CountAsync(ct);

        dbQuery = dbQuery
            .Include(x => x.LastMessage)
            .Include(x => x.Chat)
            .Skip((paged.PageNumber - 1) * paged.PageSize)
            .Take(paged.PageSize)
            .AsQueryable();


        var items = await dbQuery.IgnoreAutoIncludes().ToListAsync(ct);

        return PagedResult<ChatReadingModel>.Create(items, totalCount, paged.PageNumber, paged.PageSize);
    }
}
