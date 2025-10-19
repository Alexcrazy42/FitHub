using FitHub.Application.Equipments.Brands;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments.Brands;
using FitHub.Domain.Equipments;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Equipments;

public class BrandController : ControllerBase
{
    private readonly IBrandService brandService;

    public BrandController(IBrandService brandService)
    {
        this.brandService = brandService;
    }

    [HttpGet(ApiRoutesV1.Brands)]
    public async Task<ListResponse<BrandResponse>> GetAll([FromQuery] PagedRequest? request, CancellationToken ct)
    {
        var query = request.ToDomain();
        var pagedResult = await brandService.GetAllAsync(query, ct);
        return pagedResult.ToResponse(EquipmentResponseExtensions.ToBrandResponse);
    }

    [HttpGet(ApiRoutesV1.BrandsById)]
    public async Task<BrandResponse> GetById([FromRoute] string? id, CancellationToken ct)
    {
        var brandId = BrandId.Parse(id);
        var brand = await brandService.GetByIdAsync(brandId, ct);
        return brand.ToBrandResponse();
    }

    [HttpPost(ApiRoutesV1.Brands)]
    public async Task<BrandResponse> Create([FromBody] CreateBrandRequest? createBrandRequest, CancellationToken ct)
    {
        var brand = await brandService.CreateAsync(createBrandRequest, ct);
        return brand.ToBrandResponse();
    }

    [HttpPut(ApiRoutesV1.BrandsById)]
    public async Task<BrandResponse> Update([FromBody] UpdateBrandRequest? updateBrandRequest, CancellationToken ct)
    {
        var brand = await brandService.UpdateAsync(updateBrandRequest, ct);
        return brand.ToBrandResponse();
    }

    [HttpDelete(ApiRoutesV1.BrandsById)]
    public async Task Delete([FromRoute] string? id, CancellationToken ct)
    {
        var brandId = BrandId.Parse(id);
        await brandService.DeleteAsync(brandId, ct);
    }
}
