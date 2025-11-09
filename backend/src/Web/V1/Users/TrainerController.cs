using FitHub.Application.Users;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Auth;
using FitHub.Contracts.V1.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class TrainerController : ControllerBase
{
    private readonly IUserService userService;

    public TrainerController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpPost(ApiRoutesV1.CreateTrainer)]
    [Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
    public async Task<UserResponse> CreateTrainer([FromBody] CreateTrainerRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.RegisterTrainerAsync(request, ct);
        return user.ToResponse();
    }
}
