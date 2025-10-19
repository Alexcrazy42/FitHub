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
    private readonly IMuscleGroupRepository muscleGroupRepository;

    public MuscleGroupController(IMuscleGroupService muscleGroupService,
        IMuscleGroupRepository muscleGroupRepository)
    {
        this.muscleGroupService = muscleGroupService;
        this.muscleGroupRepository = muscleGroupRepository;
    }

    [HttpGet(ApiRoutesV1.MuscleGroups)]
    public async Task<ListResponse<MuscleGroupResponse>> GetAllAsync([FromBody] PagedRequest? pagedRequest, CancellationToken ct)
    {
        var query = pagedRequest.ToDomain();
        var pagedResult = await muscleGroupService.GetAll(query, ct);
        return pagedResult.ToResponse(TrainingResponseExtensions.ToMuscleGroupResponse);
    }

    [HttpPost(ApiRoutesV1.MuscleGroups)]
    public async Task<MuscleGroupResponse> CreateMuscleGroupAsync([FromBody] CreateMuscleGroupRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var muscleGroup = await muscleGroupService.CreateMuscleGroupAsync(request, ct);

        return muscleGroup.ToMuscleGroupResponse();
    }

    [HttpPut(ApiRoutesV1.MuscleGroupById)]
    public async Task<MuscleGroupResponse> UpdateMuscleGroupAsync([FromBody] UpdateMuscleGroupRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));
        var muscleGroup = await muscleGroupService.UpdateMuscleGroupAsync(request, ct);
        return muscleGroup.ToMuscleGroupResponse();
    }

    [HttpDelete(ApiRoutesV1.MuscleGroupById)]
    public async Task DeleteMuscleGroupAsync([FromRoute] string? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        await muscleGroupService.DeleteMuscleGroupAsync(MuscleGroupId.Parse(id), ct);
    }
}
