using FitHub.Application.Equipments.GymEquipments;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments.GymEquipments;
using FitHub.Domain.Equipments;
using FitHub.Web.Common;
using FitHub.Web.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FitHub.Common.Entities.ValidationException;

namespace FitHub.Web.V1.Equipments;

[ApiController]
public class GymEquipmentController : ControllerBase
{
    private readonly IGymEquipmentService gymEquipmentService;

    public GymEquipmentController(IGymEquipmentService gymEquipmentService)
    {
        this.gymEquipmentService = gymEquipmentService;
    }

    [HttpGet(ApiRoutesV1.GymEquipments)]
    [Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
    public async Task<ListResponse<GymEquipmentResponse>> GetAllAsync([FromQuery] PagedRequest? pagedRequest, [FromQuery] SearchGymEquipmentRequest? request, CancellationToken ct)
    {
        var pagedQuery = pagedRequest.ToQuery();

        var result = await gymEquipmentService.GetAsync(pagedQuery, request, ct);

        return result.ToListResponse(EquipmentResponseExtensions.ToResponse);
    }

    [HttpGet(ApiRoutesV1.GymEquipmentById)]
    [Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
    public async Task<GymEquipmentResponse> GetAsync([FromRoute] string? id, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(id, "id != null");
        var entityId = GymEquipmentId.Parse(id);

        var entity = await gymEquipmentService.GetAsync(entityId, ct);

        return entity.ToResponse();
    }

    [HttpPost(ApiRoutesV1.GymEquipments)]
    [Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
    public async Task<GymEquipmentResponse> CreateAsync(
        [FromBody] AddOrUpdateGymEquipmentRequest? request,
        [FromServices] IValidator<AddOrUpdateGymEquipmentRequest>? validator,
        CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request);

        await validator.HandleValidationAsync(request, ct);

        ValidationException.ThrowIfNull(request);

        var entity = await gymEquipmentService.CreateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpPut(ApiRoutesV1.GymEquipmentById)]
    [Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
    public async Task<GymEquipmentResponse> CreateAsync(
        [FromRoute] string? id,
        [FromBody] AddOrUpdateGymEquipmentRequest? request,
        [FromServices] IValidator<AddOrUpdateGymEquipmentRequest>? validator,
        CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        await validator.HandleValidationAsync(request, ct);

        ValidationException.ThrowIfNull(id, "id != null");
        ValidationException.ThrowIfNull(request);

        var entityId = GymEquipmentId.Parse(id);

        var entity = await gymEquipmentService.UpdateAsync(entityId, request, ct);

        return entity.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.GymEquipmentById)]
    [Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
    public async Task DeleteAsync([FromRoute] string? id, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(id, "id != null");

        var entityId = GymEquipmentId.Parse(id);
        await gymEquipmentService.DeleteAsync(entityId, ct);
    }
}
