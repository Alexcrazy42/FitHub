using System.Collections.Specialized;
using FitHub.Application.Files;
using FitHub.Application.Trainings.BaseGroupTrainings;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Domain.Files;
using FitHub.Domain.Trainings;
using FitHub.Web.Common;
using FitHub.Web.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FitHub.Common.Entities.ValidationException;

namespace FitHub.Web.V1.Trainings;

[ApiController]
public class BaseGroupTrainingController : ControllerBase
{
    private readonly IBaseGroupTrainingService service;
    private readonly IBaseGroupTrainingRepository repository;
    private readonly IAccessService accessService;

    public BaseGroupTrainingController(IBaseGroupTrainingService service, IBaseGroupTrainingRepository repository, IAccessService accessService)
    {
        this.service = service;
        this.repository = repository;
        this.accessService = accessService;
    }

    [HttpGet(ApiRoutesV1.BaseGroupTrainings)]
    public async Task<ListResponse<BaseGroupTrainingResponse>> GetAllAsync([FromQuery] PagedRequest? paged, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        var pagedQuery = paged.ToQuery();
        var all = await service.GetAsync(pagedQuery, ct);
        return all.ToListResponse(TrainingResponseExtensions.ToResponse);
    }


    [HttpGet(ApiRoutesV1.BaseGroupTrainingsById)]
    public async Task<BaseGroupTrainingResponse> GetByIdAsync([FromRoute] string? id, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = BaseGroupTrainingId.Parse(id);
        var entity = await service.GetByIdAsync(entityId, ct);
        return entity.ToResponse();
    }

    [HttpPost(ApiRoutesV1.BaseGroupTrainings)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<BaseGroupTrainingResponse> CreateAsync(
        [FromBody] CreateBaseGroupTrainingRequest? request,
        [FromServices] IValidator<CreateBaseGroupTrainingRequest>? validator,
        CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        await validator.HandleValidationAsync(request, ct);

        var entity = await service.CreateAsync(request, ct);

        return entity.ToResponse();
    }

    [HttpPost(ApiRoutesV1.BaseGroupTrainingPhotos)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task AttachPhotos([FromBody] AttachPhotosRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        await service.AttachPhotosAsync(request, ct);
    }

    [HttpDelete(ApiRoutesV1.BaseGroupTrainingPhotos)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task DeattachPhoto([FromQuery] string? fileId, CancellationToken ct)
    {
        fileId = ValidationException.ThrowIfNull(fileId, "fileId cannot be null");
        var parsedFileId = FileId.Parse(fileId);
        await service.DeattachPhotoAsync(parsedFileId, ct);
    }

    [HttpPut(ApiRoutesV1.BaseGroupTrainingsById)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<BaseGroupTrainingResponse> UpdateAsync([FromRoute] string? id, [FromBody] CreateBaseGroupTrainingRequest? request,
        [FromServices] IValidator<CreateBaseGroupTrainingRequest>? validator,
        CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");
        await validator.HandleValidationAsync(request, ct);

        var entityId = BaseGroupTrainingId.Parse(id);

        var entity = await service.UpdateAsync(entityId, request, ct);

        return entity.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.BaseGroupTrainingsById)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task DeleteByIdAsync([FromRoute] Guid? id, CancellationToken ct)
    {
        id = ValidationException.ThrowIfNull(id, "id cannot be null");
        var entityId = BaseGroupTrainingId.Parse(id);
        await service.DeleteAsync(entityId, ct);
    }
}
