using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Equipments.GymEquipments;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.GymEquipments;

public interface IGymEquipmentRepository : IPendingRepository<GymEquipment, GymEquipmentId>
{
    Task<PagedResult<GymEquipment>> GetAsync(PagedQuery pagedQuery, SearchGymEquipmentRequest? request, CancellationToken ct);

    Task<GymEquipment> GetAsync(GymEquipmentId id, CancellationToken ct);
}
