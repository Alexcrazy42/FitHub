using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Instructions;

public class EquipmentInstructionService : IEquipmentInstructionService
{
    public Task<EquipmentInstruction> CreateAsync(CreateEquipmentInstructionRequest request, CancellationToken ct) => throw new NotImplementedException();

    public Task<EquipmentInstruction> UpdateAsync(UpdateEquipmentInstructionRequest request, CancellationToken ct) => throw new NotImplementedException();
    public Task DeleteAsync(EquipmentInstructionId id, CancellationToken ct) => throw new NotImplementedException();
}
