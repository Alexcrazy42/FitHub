using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Instructions;

public interface IEquipmentInstructionRepository : IPendingRepository<EquipmentInstruction, EquipmentInstructionId>
{
    Task<PagedResult<EquipmentInstruction>> GetAll(PagedQuery pagedQuery, CancellationToken ct);
}
