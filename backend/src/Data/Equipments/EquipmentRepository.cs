using FitHub.Application.Common;
using FitHub.Application.Equipments;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Equipments;

public class EquipmentRepository : DefaultPendingRepository<Equipment, EquipmentId, DataContext>, IEquipmentRepository
{
    private readonly DataContext context;

    public EquipmentRepository(DataContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<PagedResult<Equipment>> GetAllAsync(PagedQuery pagedQuery, CancellationToken ct = default)
    {
        var dbQuery = ReadRaw();

        var totalItems = await dbQuery.CountAsync(ct);

        dbQuery = dbQuery
            .Include(x => x.Brand)
            .Include(x => x.Instructions)
            .OrderBy(x => x.Id)
            .Skip((pagedQuery.PageNumber - 1) * pagedQuery.PageSize)
            .Take(pagedQuery.PageSize);

        var items = await dbQuery.ToListAsync(ct);
        return PagedResult<Equipment>.Create(items, totalItems, pagedQuery.PageNumber, pagedQuery.PageSize);
    }
}
