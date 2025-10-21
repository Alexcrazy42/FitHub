using FitHub.Application.Common;
using FitHub.Contracts.V1.Equipments.Brands;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Brands;

public interface IBrandService
{
    Task<PagedResult<Brand>> GetAllAsync(SearchBrandCommand command, PagedQuery query, CancellationToken ct);
    Task<Brand> GetByIdAsync(BrandId id, CancellationToken ct);
    Task<Brand> CreateAsync(CreateBrandRequest? request, CancellationToken ct);
    Task<Brand> UpdateAsync(BrandId id, UpdateBrandRequest? request, CancellationToken ct);
    Task DeleteAsync(BrandId brandId, CancellationToken ct);
}
