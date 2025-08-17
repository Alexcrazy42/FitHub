using FitHub.Application.Trainings.VideoTrainings;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Trainings.VideoTrainings;
using FitHub.Domain.Trainings;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Trainings;

[ApiController]
public class VideoTrainingController : ControllerBase
{
    private readonly IVideoTrainingRepository repository;
    private readonly IVideoTrainingService service;

    public VideoTrainingController(IVideoTrainingRepository repository, IVideoTrainingService service)
    {
        this.repository = repository;
        this.service = service;
    }

    [HttpGet(ApiRoutesV1.VideoTrainings)]
    public async Task<ListResponse<VideoTrainingResponse>> GetAllAsync(CancellationToken ct)
    {
        var all = await repository.GetAllAsync(x => true, ct);

        var responses = all.ToResponses();

        return ListResponse<VideoTrainingResponse>.Create(responses);
    }

    [HttpGet(ApiRoutesV1.VideoTrainingsById)]
    public async Task<VideoTrainingResponse> GetByIdAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = VideoTrainingId.Parse(id);

        var entity = await repository.GetByIdAsync(entityId, ct);

        if (entity is null)
        {
            throw new NotFoundException("Видео-тренировка не найдена");
        }

        return entity.ToResponse();
    }

    [HttpPost(ApiRoutesV1.VideoTrainings)]
    public async Task<VideoTrainingResponse> CreateAsync([FromBody] CreateVideoTrainingRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var entity = await service.CreateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpPut(ApiRoutesV1.VideoTrainings)]
    public async Task<VideoTrainingResponse> UpdateAsync([FromBody] UpdateVideoTrainingRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var entity = await service.UpdateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.VideoTrainingsById)]
    public async Task DeleteByIdAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = VideoTrainingId.Parse(id);

        await service.DeleteAsync(entityId, ct);
    }
}
