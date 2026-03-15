using FitHub.Application.Common;
using FitHub.Application.Equipments.GymEquipments;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Contracts.V1.Equipments.GymEquipments;
using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Equipments.GymEquipments;

public class GymEquipmentRepository : DefaultPendingRepository<GymEquipment, GymEquipmentId, DataContext>, IGymEquipmentRepository
{
    public GymEquipmentRepository(DataContext context) : base(context)
    {
    }

    protected override IQueryable<GymEquipment> ReadRaw()
    {
        return DbSet
            .Include(x => x.Gym)
            .Include(x => x.Equipment)
                .ThenInclude(x => x.Brand);
    }

    public async Task<PagedResult<GymEquipment>> GetAsync(PagedQuery pagedQuery, SearchGymEquipmentRequest? request, CancellationToken ct)
    {
        var query = ReadRaw();

        if (request?.EquipmentId is not null)
        {
            query = query.Where(x => x.EquipmentId == EquipmentId.Parse(request.EquipmentId));
        }

        if (request?.GymId is not null)
        {
            query = query.Where(x => x.GymId == GymId.Parse(request.GymId));
        }

        if (request?.IsActive is not null)
        {
            query = query.Where(x => x.IsActive == request.IsActive);
        }

        if (request?.Condition is not null)
        {
            query = query.Where(x => x.Condition == request.Condition);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Id)
            .Skip((pagedQuery.PageNumber - 1) * pagedQuery.PageSize)
            .Take(pagedQuery.PageSize)
            .ToReadOnlyListAsync(ct);

        return PagedResult<GymEquipment>.Create(items, totalItems: total, currentPage: pagedQuery.PageNumber, pageSize: pagedQuery.PageSize);
    }

    public async Task<GymEquipment> GetAsync(GymEquipmentId id, CancellationToken ct)
    {
        var entity = await ReadRaw()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(entity, "Тренажер не найден!");

        return entity;
    }
}
