using FitHub.Application.Common;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Equipments.Brands;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Brands;

public class BrandService : IBrandService
{
    private readonly IBrandRepository brandRepository;
    private readonly IUnitOfWork unitOfWork;

    public BrandService(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        this.brandRepository = brandRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Brand>> GetAllAsync(CancellationToken ct)
    {
        var all = await brandRepository.GetAllAsync(x => true, ct);
        return all;
    }

    public Task<PagedResult<Brand>> GetAllAsync(PagedQuery query, CancellationToken ct)
    {
        return brandRepository.GetAll(query, ct);
    }

    public async Task<Brand> GetByIdAsync(BrandId id, CancellationToken ct)
    {
        var brand = await brandRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(brand, "Брэнд не найден!");
        return brand;
    }

    public async Task<Brand> CreateAsync(CreateBrandRequest? request, CancellationToken ct)
    {
        var name = ValidationException.ThrowIfNull(request?.Name, "Имя не может быть пустым!");
        var description = ValidationException.ThrowIfNull(request?.Description, "Описание не может быть пустым!");
        var brand = Brand.Create(name, description);
        await brandRepository.PendingAddAsync(brand, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return brand;
    }

    public async Task<Brand> UpdateAsync(UpdateBrandRequest? request, CancellationToken ct)
    {
        var brandId = BrandId.Parse(request?.Id);
        var brand = await GetByIdAsync(brandId, ct);
        ApplyUpdate(brand, request);
        await unitOfWork.SaveChangesAsync(ct);
        return brand;
    }

    public async Task DeleteAsync(BrandId brandId, CancellationToken ct)
    {
        var brand = await GetByIdAsync(brandId, ct);
        brandRepository.PendingRemove(brand);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private void ApplyUpdate(Brand brand, UpdateBrandRequest? request)
    {
        var name = ValidationException.ThrowIfNull(request?.Name, "Имя не может быть пустым!");
        var description = ValidationException.ThrowIfNull(request?.Description, "Описание не может быть пустым!");
        brand.SetName(name);
        brand.SetDescription(description);
    }
}
