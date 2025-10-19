using FitHub.Application.Common;
using FitHub.Contracts;
using FitHub.Contracts.V1.Equipments;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments;

public interface IEquipmentService
{
    Task<PagedResult<Equipment>> GetAllAsync(PagedQuery pagedQuery, CancellationToken ct = default);

    Task<Equipment> GetByIdAsync(EquipmentId id, CancellationToken ct = default);
    Task<Equipment> CreateAsync(CreateEquipmentRequest request, CancellationToken ct = default);

    Task<Equipment> UpdateAsync(UpdateEquipmentRequest request, CancellationToken ct = default);

    Task DeleteAsync(EquipmentId id, CancellationToken ct = default);
}
