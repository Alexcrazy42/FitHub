using FitHub.Application.Equipments.Gyms;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Equipments;

[ApiController]
public class GymController : ControllerBase
{
    private readonly IGymService gymService;

    public GymController(IGymService gymService)
    {
        this.gymService = gymService;
    }

    [HttpGet(ApiRoutesV1.Gyms)]
    public async Task<ListResponse<GymResponse>> GetGymsAsync([FromQuery] PagedRequest? request, CancellationToken ct)
    {
        var query = request.ToQuery();
        var gymPagedResult = await gymService.GetGymsAsync(query, ct);
        return gymPagedResult.ToListResponse(EquipmentResponseExtensions.ToResponse);
    }

    [HttpGet(ApiRoutesV1.GymById)]
    public async Task<GymResponse> GetById([FromRoute] Guid? id, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(id, "Id cannot be null");
        var gymId = GymId.Parse(id);
        var gym = await gymService.GetByIdAsync(gymId, ct);

        if (gym == null)
        {
            throw new NotFoundException("Зал не найден!");
        }

        return gym.ToResponse();
    }

    [HttpPost(ApiRoutesV1.Gyms)]
    public async Task<GymResponse> CreateGymAsync([FromBody] CreateGymRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var gym = await gymService.CreateGymAsync(request, ct);

        return gym.ToResponse();
    }


    [HttpPut(ApiRoutesV1.Gyms)]
    public async Task<GymResponse> UpdateGymAsync([FromBody] UpdateGymRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var gym = await gymService.UpdateGymAsync(request, ct);

        return gym.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.GymById)]
    public async Task Delete([FromRoute] string? id, CancellationToken ct)
    {
        var gymId = GymId.Parse(id);
        await gymService.DeleteAsync(gymId, ct);
    }
}
