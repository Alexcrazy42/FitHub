using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Brands;

public interface IBrandRepository : IPendingRepository<Brand, BrandId>
{
    Task<PagedResult<Brand>> GetAll(SearchBrandCommand command, PagedQuery query, CancellationToken ct);
}
