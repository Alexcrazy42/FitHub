using FitHub.Application.Users;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Auth;
using FitHub.Contracts.V1.Users;
using FitHub.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Auth;

public class AuthController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ICurrentIdentityUserIdAccessor accessor;
    private readonly ICurrentIdentityUserIdProvider provider;

    public AuthController(IUserService userService,
        ICurrentIdentityUserIdAccessor accessor,
        ICurrentIdentityUserIdProvider provider)
    {
        this.userService = userService;
        this.accessor = accessor;
        this.provider = provider;
    }

    [HttpPost(ApiRoutesV1.CheckConfirmEmail)]
    [AllowAnonymous]
    public async Task<bool> CheckConfirmEmail([FromBody] ConfirmEmailRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        return await userService.CheckConfirmEmail(request, ct);
    }

    [HttpPost(ApiRoutesV1.ConfirmEmail)]
    [AllowAnonymous]
    public Task<LoginResponse> ConfirmEmail([FromBody] ConfirmEmailRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        return userService.ConfirmEmailAsync(request, ct);
    }

    [HttpPost(ApiRoutesV1.SetPassword)]
    [AllowAnonymous]
    public Task<LoginResponse> SetPassword([FromBody] SetPasswordRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        return userService.SetPasswordAsync(request, ct);
    }

    [HttpPost(ApiRoutesV1.InitResetPassword)]
    [AllowAnonymous]
    public Task InitResetPassword([FromQuery] string? email, CancellationToken ct)
    {
        return userService.InitResetPasswordAsync(email.Required(), ct);
    }

    [HttpPost(ApiRoutesV1.CheckResetPassword)]
    [AllowAnonymous]
    public Task<bool> CheckResetPassword([FromBody] ResetPasswordRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        return userService.CheckResetPasswordAsync(request, ct);
    }

    [HttpPost(ApiRoutesV1.ResetPassword)]
    [AllowAnonymous]
    public Task<LoginResponse> ResetPassword([FromBody] ResetPasswordRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        return userService.ResetPasswordAsync(request, ct);
    }

    [HttpPost(ApiRoutesV1.StartRegister)]
    [AllowAnonymous]
    public async Task<UserResponse> StartRegister([FromBody] StartRegisterRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.StartRegister(request, ct);
        return user.ToResponse();
    }

    [HttpGet(ApiRoutesV1.Me)]
    [Authorize]
    public async Task<UserResponse> GetCurrentUser(CancellationToken ct)
    {
        var userId = accessor.GetCurrentUserId();
        var user = await userService.GetUserAsync(userId, ct);
        return user.ToResponse();
    }

    [HttpPost(ApiRoutesV1.CreateCmsAdmin)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<UserResponse> CreateCmsAdmin([FromBody] CreateCmsAdminRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.RegisterCmsAdminAsync(request, ct);
        return user.ToResponse();
    }

    [HttpPost(ApiRoutesV1.CreateGymAdmin)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<UserResponse> CreateGymAdmin([FromBody] CreateGymAdminRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.RegisterGymAdminAsync(request, ct);
        return user.ToResponse();
    }

    [HttpPost(ApiRoutesV1.CreateTrainer)]
    [Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
    public async Task<UserResponse> CreateTrainer([FromBody] CreateTrainerRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.RegisterTrainerAsync(request, ct);
        return user.ToResponse();
    }

    [HttpPost(ApiRoutesV1.Logout)]
    [Authorize]
    public async Task Logout(CancellationToken ct)
    {
        var userId = accessor.GetCurrentUserId();
        var sessionId = provider.GetSessionId();
        var parsedSessionId = SessionId.Parse(sessionId);

        await userService.Logout(userId, parsedSessionId, ct);
    }
}
