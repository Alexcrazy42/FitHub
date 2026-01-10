using FitHub.Application.Common;
using FitHub.Application.Users.Visitors;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Users.Visitors;
using FitHub.Domain.Equipments;
using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Users;

public class VisitorRepository : DefaultPendingRepository<Visitor, VisitorId, DataContext>, IVisitorRepository
{
    public VisitorRepository(DataContext context) : base(context)
    {
    }

    protected override IQueryable<Visitor> ReadRaw()
    {
        return base.ReadRaw()
            .Include(v => v.User)
            .Include(x => x.Gyms);
    }

    public async Task<PagedResult<Visitor>> GetAll(PagedQuery query, VisitorSearchRequest? request, CancellationToken ct)
    {
        var baseQuery = DbSet.AsNoTracking();

        if (request?.GymId is not null)
        {
            var gymId = GymId.Parse(request.GymId);
            baseQuery = baseQuery.Where(v => v.Gyms.Any(g => g.GymId == gymId));
        }

        var total = await baseQuery.CountAsync(ct);

        var ids = await baseQuery
            .OrderBy(v => v.Id)
            .Select(v => v.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var items = await DbSet
            .AsNoTracking()
            .Where(v => ids.Contains(v.Id))
            .Include(v => v.User)
            .Include(v => v.Gyms)
                .ThenInclude(g => g.Gym)
            .OrderBy(v => v.Id)
            .ToListAsync(ct);

        return PagedResult<Visitor>.Create(items, total, query.PageNumber, query.PageSize);
    }

    public Task<IReadOnlyList<Visitor>> GetAsync(IReadOnlyList<VisitorId> ids, CancellationToken ct)
    {
        return ReadRaw().Where(x => ids.Contains(x.Id)).ToReadOnlyListAsync(ct);
    }

    public async Task<Visitor> GetAsync(VisitorId id, CancellationToken ct)
    {
        var visitor = await ReadRaw()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(visitor, "Посетитель не найден!");

        return visitor;
    }

    public async Task<Visitor> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct)
    {
        var visitor = await ReadRaw()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken: ct);

        NotFoundException.ThrowIfNull(visitor, "Посетитель не найден!");

        return visitor;
    }
}
