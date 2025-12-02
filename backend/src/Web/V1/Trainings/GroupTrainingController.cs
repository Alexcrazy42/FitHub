using FitHub.Application.Trainings.GroupTrainings;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Trainings.GroupTrainings;
using FitHub.Domain.Trainings;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Trainings;

[ApiController]
public class GroupTrainingController : ControllerBase
{
    private readonly IGroupTrainingService groupTrainingService;

    public GroupTrainingController(IGroupTrainingService groupTrainingService)
    {
        this.groupTrainingService = groupTrainingService;
    }

    [HttpGet(ApiRoutesV1.GroupTrainings)]
    public async Task<ListResponse<GroupTrainingResponse>> GetAsync([FromQuery] PagedRequest? pagedRequest, CancellationToken ct)
    {
        var domain = pagedRequest.ToDomain();
        var result = await groupTrainingService.GetAsync(domain, ct);

        return result.ToResponse(TrainingResponseExtensions.ToResponse);
    }

    [HttpGet(ApiRoutesV1.GroupTrainingById)]
    public async Task<GroupTrainingResponse> GetByIdAsync([FromRoute] string? id, CancellationToken ct)
    {
        var groupTrainingId = GroupTrainingId.Parse(id);
        var entity = await groupTrainingService.GetByIdAsync(groupTrainingId, ct);
        return entity.ToResponse();
    }

    [HttpPost(ApiRoutesV1.GroupTrainings)]
    public async Task<GroupTrainingResponse> CreateAsync([FromBody] AddOrUpdateGroupTrainingRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request != null");
        var entity = await groupTrainingService.CreateGroupTraining(request, ct);
        return entity.ToResponse();
    }

    [HttpPut(ApiRoutesV1.GroupTrainingById)]
    public async Task<GroupTrainingResponse> UpdateAsync([FromRoute] string? id, [FromBody] AddOrUpdateGroupTrainingRequest? request, CancellationToken ct)
    {
        var groupTrainingId = GroupTrainingId.Parse(id);
        ValidationException.ThrowIfNull(request, "request != null");
        var entity = await groupTrainingService.UpdateGroupTraining(groupTrainingId, request, ct);
        return entity.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.GroupTrainingById)]
    public async Task DeleteAsync([FromRoute] string? id, CancellationToken ct)
    {
        var groupTrainingId = GroupTrainingId.Parse(id);
        await groupTrainingService.DeleteAsync(groupTrainingId, ct);
    }
}
