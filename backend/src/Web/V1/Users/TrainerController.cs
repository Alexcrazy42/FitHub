using FitHub.Application.Users;
using FitHub.Application.Users.Trainers;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Users;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Domain.Users;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class TrainerController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ITrainerService trainerService;
    private readonly IAccessService accessService;
    private readonly ICurrentIdentityUserIdAccessor accessor;

    public TrainerController(IUserService userService, ITrainerService trainerService, IAccessService accessService, ICurrentIdentityUserIdAccessor accessor)
    {
        this.userService = userService;
        this.trainerService = trainerService;
        this.accessService = accessService;
        this.accessor = accessor;
    }

    [HttpGet(ApiRoutesV1.TrainerMe)]
    [Authorize(Policy = AuthorizationPolicies.TrainerOnly)]
    public async Task<TrainerResponse> GetMe(CancellationToken ct)
    {
        var userId = accessor.GetCurrentUserId();
        var trainer = await trainerService.GetByUserIdAsync(userId, ct);
        return trainer.ToResponse();
    }

    [HttpGet(ApiRoutesV1.Trainers)]
    public async Task<ListResponse<TrainerResponse>> GetAll([FromQuery] PagedRequest? request, [FromQuery] TrainerQuery? trainerQuery, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        var query = request.ToQuery();
        var result = await trainerService.GetAll(query, trainerQuery, ct);
        return result.ToListResponse(UserExtensions.ToResponse);
    }

    [HttpPost(ApiRoutesV1.Trainers)]
    public async Task<UserResponse> CreateTrainer([FromBody] CreateTrainerRequest? request, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.RegisterTrainerAsync(request, ct);
        return user.ToResponse();
    }

    [HttpPut(ApiRoutesV1.TrainerSetStatus)]
    public async Task SetStatus([FromRoute] string? id, [FromQuery] bool? status, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        var trainerId = TrainerId.Parse(id);
        await trainerService.SetStatus(trainerId, status ?? false, ct);
    }
}
