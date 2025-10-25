using FitHub.Application.Equipments;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments;
using FitHub.Domain.Equipments;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Equipments;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService service;
    private readonly IEquipmentRepository repository;

    public EquipmentController(IEquipmentService service, IEquipmentRepository repository)
    {
        this.service = service;
        this.repository = repository;
    }

    [HttpGet(ApiRoutesV1.Equipments)]
    public async Task<ListResponse<EquipmentResponse>> GetAllAsync([FromQuery] PagedRequest? pagedRequest, CancellationToken ct)
    {
        var query = pagedRequest.ToDomain();
        var pagedResult = await service.GetAllAsync(query, ct);
        return pagedResult.ToResponse(EquipmentResponseExtensions.ToEquipmentResponse);
    }

    [HttpGet(ApiRoutesV1.EquipmentById)]
    public async Task<EquipmentResponse> GetAsync([FromRoute] string? id, CancellationToken ct)
    {
        var equipmentId = EquipmentId.Parse(id);
        var entity = await service.GetByIdAsync(equipmentId, ct);

        if (entity is null)
        {
            throw new NotFoundException("Тренажёр не найден");
        }

        return entity.ToEquipmentResponse();
    }

    [HttpPost(ApiRoutesV1.Equipments)]
    public async Task<EquipmentResponse> CreateAsync([FromBody] CreateEquipmentRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var equipment = await service.CreateAsync(request, ct);

        return equipment.ToEquipmentResponse();
    }


    [HttpPut(ApiRoutesV1.EquipmentById)]
    public async Task<EquipmentResponse> UpdateAsync([FromRoute] string? id, [FromBody] UpdateEquipmentRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");
        var entityId = EquipmentId.Parse(id);

        var equipment = await service.UpdateAsync(entityId, request, ct);

        return equipment.ToEquipmentResponse();
    }

    [HttpDelete(ApiRoutesV1.EquipmentById)]
    public async Task DeleteAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");

        await service.DeleteAsync(EquipmentId.Parse(id), ct);
    }
}
