using FitHub.Application.Common;
using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Instructions;

public interface IEquipmentInstructionService
{
    Task<PagedResult<EquipmentInstruction>> GetAll(PagedQuery pagedQuery, CancellationToken ct);

    Task<EquipmentInstruction> GetById(EquipmentInstructionId id, CancellationToken ct);

    Task<EquipmentInstruction> CreateAsync(CreateEquipmentInstructionRequest request, CancellationToken ct = default);

    Task<EquipmentInstruction> UpdateAsync(UpdateEquipmentInstructionRequest request, CancellationToken ct = default);

    Task DeleteAsync(EquipmentInstructionId id, CancellationToken ct = default);
}
