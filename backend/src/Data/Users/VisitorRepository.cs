using FitHub.Application.Common;
using FitHub.Application.Users.Visitors;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Users;

public class VisitorRepository : DefaultPendingRepository<Visitor, VisitorId, DataContext>, IVisitorRepository
{
    public VisitorRepository(DataContext context) : base(context)
    {
    }

    public async Task<PagedResult<Visitor>> GetAll(PagedQuery query, CancellationToken ct)
    {
        var dbQuery = ReadRaw()
            .Include(x => x.User)
            .AsQueryable();

        var total = await dbQuery.CountAsync(ct);

        dbQuery = dbQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize);

        var items = await dbQuery.ToListAsync(ct);

        return PagedResult<Visitor>.Create(items: items, totalItems: total, currentPage: query.PageNumber, pageSize: query.PageSize);
    }
}
