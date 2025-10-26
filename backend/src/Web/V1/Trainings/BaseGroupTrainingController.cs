using FitHub.Application.Trainings.BaseGroupTrainings;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Domain.Trainings;
using FitHub.Web.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FitHub.Common.Entities.ValidationException;

namespace FitHub.Web.V1.Trainings;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
public class BaseGroupTrainingController : ControllerBase
{
    private readonly IBaseGroupTrainingService service;
    private readonly IBaseGroupTrainingRepository repository;

    public BaseGroupTrainingController(IBaseGroupTrainingService service, IBaseGroupTrainingRepository repository)
    {
        this.service = service;
        this.repository = repository;
    }

    [HttpGet(ApiRoutesV1.BaseGroupTrainings)]
    public async Task<ListResponse<BaseGroupTrainingResponse>> GetAllAsync(CancellationToken ct)
    {
        var all = await repository.GetAllAsync(x => true, ct);

        var responses = all.ToResponses();

        return ListResponse<BaseGroupTrainingResponse>.Create(responses);
    }


    [HttpGet(ApiRoutesV1.BaseGroupTrainingsById)]

    public async Task<BaseGroupTrainingResponse> GetByIdAsync([FromRoute] string? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = BaseGroupTrainingId.Parse(id);
        var entity = await repository.GetOrDefaultAsync(entityId, ct);

        if (entity is null)
        {
            throw new NotFoundException("Базовая групповая тренировка не найдена!");
        }

        return entity.ToResponse();
    }

    [HttpPost(ApiRoutesV1.BaseGroupTrainings)]
    public async Task<BaseGroupTrainingResponse> CreateAsync(
        [FromBody] CreateBaseGroupTrainingRequest request,
        [FromServices] IValidator<CreateBaseGroupTrainingRequest>? validator,
        CancellationToken ct)
    {
        await validator.HandleValidationAsync(request, ct);

        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var entity = await service.CreateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpPut(ApiRoutesV1.BaseGroupTrainings)]
    public async Task<BaseGroupTrainingResponse> UpdateAsync([FromBody] UpdateBaseGroupTrainingRequest request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var entity = await service.UpdateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.BaseGroupTrainingsById)]
    public async Task DeleteByIdAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = BaseGroupTrainingId.Parse(id);
        await service.DeleteAsync(entityId, ct);
    }
}
