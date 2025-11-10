using FitHub.Application.Users;
using FitHub.Application.Users.Trainers;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Users;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class TrainerController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ITrainerService trainerService;

    public TrainerController(IUserService userService, ITrainerService trainerService)
    {
        this.userService = userService;
        this.trainerService = trainerService;
    }

    [HttpGet(ApiRoutesV1.Trainers)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<ListResponse<TrainerResponse>> GetAll([FromQuery] PagedRequest? request, CancellationToken ct)
    {
        var query = request.ToDomain();
        var result = await trainerService.GetAll(query, ct);
        return result.ToResponse(UserExtensions.ToResponse);
    }

    [HttpPost(ApiRoutesV1.Trainers)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<UserResponse> CreateTrainer([FromBody] CreateTrainerRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.RegisterTrainerAsync(request, ct);
        return user.ToResponse();
    }
}
