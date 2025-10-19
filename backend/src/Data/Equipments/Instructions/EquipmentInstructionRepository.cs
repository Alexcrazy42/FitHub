using FitHub.Application.Common;
using FitHub.Application.Equipments.Instructions;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Equipments.Instructions;

public class EquipmentInstructionRepository : DefaultPendingRepository<EquipmentInstruction, EquipmentInstructionId, DataContext>, IEquipmentInstructionRepository
{
    public EquipmentInstructionRepository(DataContext context) : base(context)
    {
    }

    public async Task<PagedResult<EquipmentInstruction>> GetAll(PagedQuery pagedQuery, CancellationToken ct)
    {
        var dbQuery = ReadRaw();

        var total = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .OrderBy(x => x.Id)
            .Skip((pagedQuery.PageNumber - 1) * pagedQuery.PageSize)
            .Take(pagedQuery.PageSize)
            .ToListAsync(ct);

        return PagedResult<EquipmentInstruction>.Create(items, totalItems: total, currentPage: pagedQuery.PageNumber, pageSize: pagedQuery.PageSize);
    }
}
