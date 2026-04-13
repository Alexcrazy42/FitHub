using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface ICourierRepository : IPendingRepository<Courier, CourierId>
{
    Task<Courier?> GetByIdAsync(CourierId courierId, CancellationToken ct);

    Task<Courier?> GetByNameAsync(string name, CancellationToken ct);

    Task<IReadOnlyList<Courier>> GetAvailableForAssignmentAsync(int take, CancellationToken ct);
}
