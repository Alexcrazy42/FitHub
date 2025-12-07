using FitHub.Application.Common;
using FitHub.Contracts.V1.Equipments.GymEquipments;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.GymEquipments;

public interface IGymEquipmentService
{
    Task<PagedResult<GymEquipment>> GetAsync(PagedQuery pagedQuery, SearchGymEquipmentRequest? request, CancellationToken ct);

    Task<GymEquipment> GetAsync(GymEquipmentId id, CancellationToken ct);

    Task<GymEquipment> CreateAsync(AddOrUpdateGymEquipmentRequest request, CancellationToken ct);

    Task<GymEquipment> UpdateAsync(GymEquipmentId id, AddOrUpdateGymEquipmentRequest request, CancellationToken ct);

    Task DeleteAsync(GymEquipmentId id, CancellationToken ct);
}
