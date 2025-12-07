using FitHub.Application.Common;
using FitHub.Application.Equipments.Instructions;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Equipments;

[ApiController]
public class EquipmentInstructionController : ControllerBase
{
    private readonly IEquipmentInstructionRepository repository;
    private readonly IEquipmentInstructionService service;

    public EquipmentInstructionController(IEquipmentInstructionRepository repository,
        IEquipmentInstructionService service)
    {
        this.repository = repository;
        this.service = service;
    }

    [HttpGet(ApiRoutesV1.EquipmentsInstructions)]
    public async Task<ListResponse<EquipmentInstructionResponse>> GetAllAsync([FromQuery] PagedRequest? request, CancellationToken ct)
    {
        var query = request.ToDomain();
        var pagedResult = await service.GetAll(query, ct);
        return pagedResult.ToResponse(EquipmentResponseExtensions.ToFullInstructionResponse);
    }

    [HttpPost(ApiRoutesV1.EquipmentsInstructions)]
    public async Task<EquipmentInstructionResponse> CreateAsync([FromBody] CreateEquipmentInstructionRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var entity = await service.CreateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpPut(ApiRoutesV1.EquipmentsInstructions)]
    public async Task<EquipmentInstructionResponse> UpdateAsync([FromBody] UpdateEquipmentInstructionRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var entity = await service.UpdateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.EquipmentInstructionById)]
    public async Task DeleteAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = EquipmentInstructionId.Parse(id);

        await service.DeleteAsync(entityId, ct);
    }
}
