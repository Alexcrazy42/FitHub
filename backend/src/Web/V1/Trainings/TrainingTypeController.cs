using FitHub.Application.Trainings.TrainingTypes;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Trainings.TrainingTypes;
using FitHub.Domain.Trainings;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Trainings;

[ApiController]
public class TrainingTypeController : ControllerBase
{
    private readonly ITrainingTypeService service;
    private readonly ITrainingTypeRepository repository;

    public TrainingTypeController(ITrainingTypeService service, ITrainingTypeRepository repository)
    {
        this.service = service;
        this.repository = repository;
    }

    [HttpGet(ApiRoutesV1.TrainingTypes)]
    public async Task<ListResponse<TrainingTypeResponse>> GetAll(CancellationToken ct)
    {
        var all = await repository.GetAllAsync(x => true, ct);
        var responses = all.ToResponses();

        return ListResponse<TrainingTypeResponse>.Create(responses);
    }

    [HttpGet(ApiRoutesV1.TrainingTypeById)]
    public async Task<TrainingTypeResponse> GetById([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = TrainingTypeId.Parse(id);

        var entity = await repository.GetSingleOrDefaultAsync(x => x.Id == entityId, ct);

        if (entity is null)
        {
            throw new NotFoundException("Тип тренировки не найден!");
        }

        return entity.ToResponse();
    }

    [HttpPost(ApiRoutesV1.TrainingTypes)]
    public async Task<TrainingTypeResponse> CreateAsync([FromBody] CreateTrainingTypeRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var entity = await service.CreateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpPut(ApiRoutesV1.TrainingTypes)]
    public async Task<TrainingTypeResponse> UpdateAsync([FromBody] UpdateTrainingTypeRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var entity = await service.UpdateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.TrainingTypeById)]
    public async Task Delete([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = TrainingTypeId.Parse(id);

        await service.DeleteAsync(entityId, ct);
    }
}
