using FitHub.Application.Common;
using FitHub.Application.Users;
using FitHub.Application.Users.GymAdmins;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
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

    protected override IQueryable<GymAdmin> ReadRaw()
    {
        return base.ReadRaw()
            .Include(x => x.User)
            .Include(x => x.Gyms)
            .AsQueryable();
    }

    public async Task<PagedResult<GymAdmin>> GetAll(PagedQuery query, CancellationToken ct)
    {
        var dbQuery = ReadRaw();

        var total = await dbQuery.CountAsync(ct);

        dbQuery = dbQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize);

        var items = await dbQuery.ToListAsync(ct);

        return PagedResult<GymAdmin>.Create(items: items, totalItems: total, currentPage: query.PageNumber, pageSize: query.PageSize);
    }

    public async Task<GymAdmin> GetAsync(GymAdminId id, CancellationToken ct)
    {
        var admin = await ReadRaw()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(admin, "Администратор не найден!");

        return admin;
    }

    public async Task<GymAdmin> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct)
    {
        var admin = await ReadRaw()
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        NotFoundException.ThrowIfNull(admin, "Администратор зала не найден!");
        return admin;
    }
}
