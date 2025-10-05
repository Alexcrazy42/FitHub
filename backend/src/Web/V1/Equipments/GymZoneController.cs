using FitHub.Application.Equipments.Gyms;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Domain.Equipments;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Equipments;

public class GymZoneController : ControllerBase
{
    private readonly IGymZoneService gymZoneService;
    private readonly IGymZoneRepository gymZoneRepository;

    public GymZoneController(IGymZoneService gymZoneService, IGymZoneRepository gymZoneRepository)
    {
        this.gymZoneService = gymZoneService;
        this.gymZoneRepository = gymZoneRepository;
    }

    [HttpGet(ApiRoutesV1.GymZones)]
    public async Task<ListResponse<GymZoneResponse>> GetAllAsync(CancellationToken ct)
    {
        var all = await gymZoneRepository.GetAllAsync(x => true, ct);
        return ListResponse<GymZoneResponse>.Create(all.ToGymZoneResponses());
    }


    [HttpGet(ApiRoutesV1.GymZoneById)]
    public async Task<GymZoneResponse> GetById([FromRoute] Guid? id, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(id, "Id cannot be null");
        var gymZoneId = GymZoneId.Parse(id);
        var gymZone = await gymZoneRepository.GetSingleOrDefaultAsync(x => x.Id == gymZoneId, ct);

        if (gymZone == null)
        {
            throw new NotFoundException("Зона зала не найдена!");
        }

        return gymZone.ToZoneResponse();
    }

    [HttpPost(ApiRoutesV1.GymZones)]
    public async Task<GymZoneResponse> CreateGymAsync([FromBody] CreateGymZoneRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var gym = await gymZoneService.CreateGymZoneAsync(request, ct);

        return gym.ToZoneResponse();
    }


    [HttpPut(ApiRoutesV1.GymZones)]
    public async Task<GymZoneResponse> UpdateGymAsync([FromBody] UpdateGymZoneRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var gym = await gymZoneService.UpdateGymZoneAsync(request, ct);

        return gym.ToZoneResponse();
    }

    [HttpDelete(ApiRoutesV1.GymZoneById)]
    public async Task DeleteGymAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        var gymZoneId = GymZoneId.Parse(id.ValidateForNull());
        await gymZoneService.DeleteAsync(gymZoneId, ct);
    }
}
