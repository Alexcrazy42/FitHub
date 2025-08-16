using FitHub.Contracts.V1.Equipments;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments;

public interface IEquipmentService
{
    Task<Equipment> CreateAsync(CreateEquipmentRequest request, CancellationToken ct = default);

    Task<Equipment> UpdateAsync(UpdateEquipmentRequest request, CancellationToken ct = default);

    Task DeleteAsync(EquipmentId id, CancellationToken ct = default);
}
