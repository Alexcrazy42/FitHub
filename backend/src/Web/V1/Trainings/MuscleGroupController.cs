using FitHub.Application.Trainings.MuscleGroups;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Domain.Trainings;
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
    public async Task<ListResponse<MuscleGroupResponse>> GetAllAsync(CancellationToken ct)
    {
        var muscleGroups = await muscleGroupRepository.GetAllAsync(x => true, ct);

        var responses = muscleGroups.ToResponses();

        return ListResponse<MuscleGroupResponse>.Create(responses);
    }

    [HttpPost(ApiRoutesV1.MuscleGroups)]
    public async Task<MuscleGroupResponse> CreateMuscleGroupAsync([FromBody] CreateMuscleGroupRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var muscleGroup = await muscleGroupService.CreateMuscleGroupAsync(request, ct);

        return muscleGroup.ToResponse();
    }

    [HttpPut(ApiRoutesV1.MuscleGroupById)]
    public async Task<MuscleGroupResponse> UpdateMuscleGroupAsync([FromBody] UpdateMuscleGroupRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, nameof(request));

        var muscleGroup = await muscleGroupService.UpdateMuscleGroupAsync(request, ct);

        return muscleGroup.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.MuscleGroupById)]
    public async Task DeleteMuscleGroupAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");

        await muscleGroupService.DeleteMuscleGroupAsync(MuscleGroupId.Parse(id), ct);
    }
}
