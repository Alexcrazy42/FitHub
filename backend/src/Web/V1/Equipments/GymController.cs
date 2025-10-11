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
        var query = request.ToDomain();
        var gymPagedResult = await gymService.GetGymsAsync(query, ct);
        return gymPagedResult.ToResponse(EquipmentResponseExtensions.ToGymResponse);
    }

    [HttpGet(ApiRoutesV1.GymById)]
    public async Task<GymResponse> GetById([FromRoute] Guid? id, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(id, "Id cannot be null");
        var gymId = GymId.Parse(id);
        var gym = await gymService.GetGymOrDefaultAsync(gymId, ct);

        if (gym == null)
        {
            throw new NotFoundException("Зал не найден!");
        }

        return gym.ToGymResponse();
    }

    [HttpPost(ApiRoutesV1.Gyms)]
    public async Task<GymResponse> CreateGymAsync([FromBody] CreateGymRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var gym = await gymService.CreateGymAsync(request, ct);

        return gym.ToGymResponse();
    }


    [HttpPut(ApiRoutesV1.Gyms)]
    public async Task<GymResponse> UpdateGymAsync([FromBody] UpdateGymRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var gym = await gymService.UpdateGymAsync(request, ct);

        return gym.ToGymResponse();
    }

    // [HttpPost(ApiRoutesV1.GymPhoto)]
    // [Consumes("multipart/form-data")]
    // public async Task<GymResponse> AddPhotoAsync([FromForm] AddFileRequest request, CancellationToken ct)
    // {
    //     var gym = await gymService.AddFileAsync(request, ct);
    //     return gym.ToGymResponse();
    // }
    //
    // [HttpDelete(ApiRoutesV1.GymPhotoById)]
    // public async Task DeletePhotoAsync([FromRoute] string? id, CancellationToken ct)
    // {
    //     var gymId = GymId.Parse(id);
    //     await gymService.RemoveFileAsync(gymId, ct);
    // }
}
