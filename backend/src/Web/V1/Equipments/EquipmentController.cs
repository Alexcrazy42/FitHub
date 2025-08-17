using FitHub.Application.Equipments;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments;
using FitHub.Domain.Equipments;
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
    public async Task<ListResponse<EquipmentResponse>> GetAllAsync(CancellationToken ct)
    {
        var all = await repository.GetAllAsync(x => true, ct);

        var responses = all.ToResponses();

        return ListResponse<EquipmentResponse>.Create(responses);
    }

    [HttpGet(ApiRoutesV1.EquipmentById)]
    public async Task<EquipmentResponse> GetAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entity = await repository.GetFirstOrDefaultAsync(x => x.Id == EquipmentId.Parse(id), ct);

        if (entity is null)
        {
            throw new NotFoundException("Тренажёр не найден");
        }

        return entity.ToResponse();
    }

    [HttpPost(ApiRoutesV1.Equipments)]
    public async Task<EquipmentResponse> CreateAsync([FromBody] CreateEquipmentRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var equipment = await service.CreateAsync(request, ct);

        return equipment.ToResponse();
    }


    [HttpPut(ApiRoutesV1.Equipments)]
    public async Task<EquipmentResponse> UpdateAsync([FromBody] UpdateEquipmentRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var equipment = await service.UpdateAsync(request, ct);

        return equipment.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.EquipmentById)]
    public async Task DeleteAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");

        await service.DeleteAsync(EquipmentId.Parse(id), ct);
    }
}
