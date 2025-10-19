using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments;

public interface IEquipmentRepository : IPendingRepository<Equipment, EquipmentId>
{
    Task<PagedResult<Equipment>> GetAllAsync(PagedQuery pagedQuery, CancellationToken ct = default);
}
