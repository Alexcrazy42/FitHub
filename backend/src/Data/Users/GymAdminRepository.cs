using FitHub.Application.Common;
using FitHub.Application.Users;
using FitHub.Application.Users.GymAdmins;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Users;

public class GymAdminRepository : DefaultPendingRepository<GymAdmin, GymAdminId, DataContext>, IGymAdminRepository
{
    private readonly DataContext context;

    public GymAdminRepository(DataContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<PagedResult<GymAdmin>> GetAll(PagedQuery query, CancellationToken ct)
    {
        var dbQuery = ReadRaw()
            .Include(x => x.User)
            .Include(x => x.Gyms)
            .AsQueryable();

        var total = await dbQuery.CountAsync(ct);

        dbQuery = dbQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize);

        var items = await dbQuery.ToListAsync(ct);

        return PagedResult<GymAdmin>.Create(items: items, totalItems: total, currentPage: query.PageNumber, pageSize: query.PageSize);
    }
}
