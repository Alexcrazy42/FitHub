using FitHub.Application.Common;
using FitHub.Application.Equipments.Gyms;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Equipments.Gyms;

public class GymRepository : DefaultPendingRepository<Gym, GymId, DataContext>, IGymRepository
{
    public GymRepository(DataContext context) : base(context)
    {
    }

    public async Task<PagedResult<Gym>> GetGymsAsync(PagedQuery pagedQuery, CancellationToken ct = default)
    {
        var dbQuery = ReadRaw();

        var totalItems = await dbQuery.CountAsync(ct);

        dbQuery = dbQuery
            .OrderBy(x => x.Id)
            .Skip((pagedQuery.PageNumber - 1) * pagedQuery.PageSize)
            .Take(pagedQuery.PageSize);

        var items = await dbQuery.ToListAsync(ct);

        return PagedResult<Gym>.Create(items, totalItems, pagedQuery.PageNumber, pagedQuery.PageSize);
    }

    public async Task<Gym> GetById(GymId id, CancellationToken ct)
    {
        var gym = await ReadRaw().FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(gym, "Зал не найден!");

        return gym;
    }
}
