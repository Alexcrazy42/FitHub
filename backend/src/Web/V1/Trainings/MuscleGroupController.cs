using FitHub.Application.Common;
using FitHub.Application.Trainings.MuscleGroups;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Domain.Trainings;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Trainings;

[ApiController]
public class MuscleGroupController : ControllerBase
{
    private readonly IMuscleGroupService muscleGroupService;

    public MuscleGroupController(IMuscleGroupService muscleGroupService)
    {
        this.muscleGroupService = muscleGroupService;
    }

    [HttpGet(ApiRoutesV1.MuscleGroups)]
    public async Task<ListResponse<MuscleGroupResponse>> GetAllAsync([FromQuery] PagedRequest? pagedRequest, CancellationToken ct)
    {
        var query = pagedRequest.ToQuery();
        var pagedResult = await muscleGroupService.GetAll(query, ct);
        return pagedResult.ToListResponse(TrainingResponseExtensions.ToMuscleGroupResponse);
    }

    [HttpPost(ApiRoutesV1.MuscleGroups)]
    public async Task<MuscleGroupResponse> CreateMuscleGroupAsync([FromBody] CreateMuscleGroupRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var muscleGroup = await muscleGroupService.CreateMuscleGroupAsync(request, ct);

        return muscleGroup.ToMuscleGroupResponse();
    }

    [HttpPut(ApiRoutesV1.MuscleGroupById)]
    public async Task<MuscleGroupResponse> UpdateMuscleGroupAsync([FromRoute] string? id, [FromBody] CreateMuscleGroupRequest? request, CancellationToken ct)
    {
        var muscleGroupId = MuscleGroupId.Parse(id);
        request = ValidationException.ThrowIfNull(request, nameof(request));
        var muscleGroup = await muscleGroupService.UpdateMuscleGroupAsync(muscleGroupId, request, ct);
        return muscleGroup.ToMuscleGroupResponse();
    }

    [HttpDelete(ApiRoutesV1.MuscleGroupById)]
    public async Task DeleteMuscleGroupAsync([FromRoute] string? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        await muscleGroupService.DeleteMuscleGroupAsync(MuscleGroupId.Parse(id), ct);
    }
}
