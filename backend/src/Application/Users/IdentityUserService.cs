using FitHub.Application.EmailNotifications;
using FitHub.Application.Equipments.Gyms;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.AspNetCore.Tokens;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Auth;
using FitHub.Domain.Equipments;
using FitHub.Domain.Notifications;
using FitHub.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FitHub.Application.Users;

public class IdentityUserService : IIdentityUserService, IUserService, IAuthenticationService
{
    private readonly IUserRepository userRepository;
    private readonly IGymAdminRepository gymAdminRepository;
    private readonly IGymRepository gymRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IEmailNotificationRepository emailNotificationRepository;
    private readonly ITokenRepository tokenRepository;
    private readonly ITrainerRepository trainerRepository;
    private readonly ICurrentIdentityUserIdAccessor accessor;
    private readonly HttpContext context;
    private readonly IAuthOptions authOptions;
    private readonly ITokenService tokenService;

    public IdentityUserService(IUserRepository userRepository,
        IGymAdminRepository gymAdminRepository,
        IUnitOfWork unitOfWork,
        IGymRepository gymRepository,
        IEmailNotificationRepository emailNotificationRepository,
        ITokenRepository tokenRepository,
        ITrainerRepository trainerRepository,
        ICurrentIdentityUserIdAccessor accessor,
        IHttpContextAccessor httpContextAccessor,
        IOptions<IAuthOptions> authOptions,
        ITokenService tokenService)
    {
        this.userRepository = userRepository;
        this.gymAdminRepository = gymAdminRepository;
        this.unitOfWork = unitOfWork;
        this.gymRepository = gymRepository;
        this.emailNotificationRepository = emailNotificationRepository;
        this.tokenRepository = tokenRepository;
        this.trainerRepository = trainerRepository;
        this.accessor = accessor;
        this.context = httpContextAccessor.HttpContext ?? throw new ArgumentException("Не получается получить HttpContext");
        this.tokenService = tokenService;
        this.authOptions = authOptions.Value;
    }

    public async Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await userRepository.GetFirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<IdentityUser?> GetOrDefaultAsync(IdentityUserId id, CancellationToken cancellationToken)
    {
        return await userRepository.GetFirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User> RegisterCmsAdminAsync(CreateCmsAdminRequest request, CancellationToken ct = default)
    {

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(DefaultUserConsts.DefaultPassword).Required();

        var email = ValidationException.ThrowIfNull(request.Email, "Почта не может быть пустой");

        if (await GetByEmailAsync(email, ct) is not null)
        {
            throw new AlreadyExistsException("Пользователь с таким Email уже существует!");
        }

        var user = User.Create(
            surname: ValidationException.ThrowIfNull(request.Surname, "Фамилия не может быть пустой"),
            name: ValidationException.ThrowIfNull(request.Name, "Имя не может быть пустым"),
            email: email,
            passwordHash: passwordHash,
            userType: IdentityUserType.CmsAdmin,
            startRegistrationAt: DateTimeOffset.UtcNow,
            lastSeenAt: DateTimeOffset.UtcNow
        );

        var token = Token.Create(user, TokenType.ConfirmEmail);

        var notification = EmailNotification.Create(email, "Подтвердите свою почту", bodyHtml: $"userId: {user.Id} token: {token.TokenString}");

        await userRepository.PendingAddAsync(user, ct);
        await emailNotificationRepository.PendingAddAsync(notification, ct);
        await tokenRepository.PendingAddAsync(token, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<User> RegisterGymAdminAsync(CreateGymAdminRequest request, CancellationToken ct = default)
    {
        var gymId = GymId.Parse(request.GymId);

        var gym = await gymRepository.GetById(gymId, ct);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(DefaultUserConsts.DefaultPassword).Required();

        var email = ValidationException.ThrowIfNull(request.Email, "Почта не может быть пустой");

        if (await GetByEmailAsync(email, ct) is not null)
        {
            throw new AlreadyExistsException("Пользователь с таким Email уже существует!");
        }

        var user = User.Create(
            surname: ValidationException.ThrowIfNull(request.Surname, "Фамилия не может быть пустой"),
            name: ValidationException.ThrowIfNull(request.Name, "Имя не может быть пустым"),
            email: email,
            passwordHash: passwordHash,
            userType: IdentityUserType.GymAdmin,
            startRegistrationAt: DateTimeOffset.UtcNow,
            lastSeenAt: DateTimeOffset.UtcNow
        );

        var gymAdmin = GymAdmin.Create();
        gymAdmin.SetGym(gym);

        var token = Token.Create(user, TokenType.ConfirmEmail);

        var notification = EmailNotification.Create(email, "Подтвердите свою почту", bodyHtml: $"userId: {user.Id} token: {token.TokenString}");

        await userRepository.PendingAddAsync(user, ct);
        await gymAdminRepository.PendingAddAsync(gymAdmin, ct);
        await emailNotificationRepository.PendingAddAsync(notification, ct);
        await tokenRepository.PendingAddAsync(token, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<User> RegisterTrainerAsync(CreateTrainerRequest request, CancellationToken ct = default)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(DefaultUserConsts.DefaultPassword).Required();

        var email = ValidationException.ThrowIfNull(request.Email, "Почта не может быть пустой");

        if (await GetByEmailAsync(email, ct) is not null)
        {
            throw new AlreadyExistsException("Пользователь с таким Email уже существует!");
        }

        var user = User.Create(
            surname: ValidationException.ThrowIfNull(request.Surname, "Фамилия не может быть пустой"),
            name: ValidationException.ThrowIfNull(request.Name, "Имя не может быть пустым"),
            email: email,
            passwordHash: passwordHash,
            userType: IdentityUserType.Trainer,
            startRegistrationAt: DateTimeOffset.UtcNow,
            lastSeenAt: DateTimeOffset.UtcNow
        );

        var trainer = Trainer.Create(user);

        var token = Token.Create(user, TokenType.ConfirmEmail);

        var notification = EmailNotification.Create(email, "Подтвердите свою почту", bodyHtml: $"userId: {user.Id} token: {token.TokenString}");

        await userRepository.PendingAddAsync(user, ct);
        await emailNotificationRepository.PendingAddAsync(notification, ct);
        await tokenRepository.PendingAddAsync(token, ct);
        await trainerRepository.PendingAddAsync(trainer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<LoginResponse> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken ct = default)
    {
        var userId = IdentityUserId.Parse(request.UserId);

        var user = await GetOrDefaultAsync(userId, ct);
        if (user == null)
        {
            throw new NotFoundException("Пользователь не найден!");
        }

        var token = await tokenRepository.GetFirstOrDefaultAsync(x => x.TokenType == TokenType.ConfirmEmail && x.UserId == userId, ct);
        if (token is null)
        {
            throw new NotFoundException("Не удается найти токен!");
        }

        if (token.ExpiresOn < DateTimeOffset.UtcNow)
        {
            throw new ValidationException("Токен просрочен!");
        }

        if (token.TokenString != request.Token)
        {
            throw new ValidationException("Токен неправильный!");
        }

        user.SetEmailConfirmed(true);
        await unitOfWork.SaveChangesAsync(ct);

        return GetLoginResponse(user);
    }

    public async Task<LoginResponse> SetPasswordAsync(SetPasswordRequest request, CancellationToken ct = default)
    {
        var userId = IdentityUserId.Parse(request.UserId);

        var user = await GetOrDefaultAsync(userId, ct);
        if (user == null)
        {
            throw new NotFoundException("Пользователь не найден!");
        }

        if (!user.IsTemporaryPassword)
        {
            throw new AlreadyExistsException("Пароль не является временным!");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password.Required()).Required();
        user.SetPassword(passwordHash);
        user.SetActive(true);

        await unitOfWork.SaveChangesAsync(ct);

        return GetLoginResponse(user);
    }

    public async Task InitResetPasswordAsync(CancellationToken ct = default)
    {
        var userId = accessor.GetCurrentUserId();
        var user = await userRepository.GetFirstOrDefaultAsync(x => x.Id == userId, ct);
        NotFoundException.ThrowIfNull(user, "Пользователь не найден!");

        var token = Token.Create(user, TokenType.ResetPassword);
        var notification = EmailNotification.Create(user.Email, "Смена пароля", $"UserId: {user.Id} token: {token.TokenString}");

        await tokenRepository.PendingAddAsync(token, ct);
        await emailNotificationRepository.PendingAddAsync(notification, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<bool> CheckResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var userId = IdentityUserId.Parse(request.UserId);
        var token = await tokenRepository.GetFirstOrDefaultAsync(x => x.TokenType == TokenType.ResetPassword && x.UserId == userId && x.TokenString == request.Token, ct);
        return token != null && token.ExpiresOn > DateTimeOffset.UtcNow;
    }

    public async Task<LoginResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var check = await CheckResetPasswordAsync(request, ct);
        if (!check)
        {
            throw new ValidationException("Не получается восстановить пароль!");
        }
        var userId = IdentityUserId.Parse(request.UserId);
        var user = await userRepository.GetFirstOrDefaultAsync(x => x.Id == userId, ct);
        NotFoundException.ThrowIfNull(user, "Пользователь не найден!");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword.Required()).Required();
        user.SetPassword(passwordHash);

        await unitOfWork.SaveChangesAsync(ct);

        return GetLoginResponse(user);
    }

    public async Task<LoginResponse> LoginAsync(string login, string password, CancellationToken cancellationToken)
    {
        // var identityUser = new IdentityUser(
        //     IdentityUserId.Parse(Guid.NewGuid()),
        //     "email@mail.ru",
        //     password,
        //     IdentityUserType.CmsAdmin
        // );
        // identityUser.SetPassword(password);
        // identityUser.SetEmailConfirmed(true);
        // identityUser.SetActive(true);

        //return Task.FromResult(GetLoginResponse(identityUser));

        var user = await GetByEmailAsync(login, cancellationToken);
        NotFoundException.ThrowIfNull(user, "Пользователь не найден!");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new ValidationException("Пароль неверен!");
        }

        return GetLoginResponse(user);
    }

    private LoginResponse GetLoginResponse(IdentityUser user)
    {
        var loginResponse = new LoginResponse()
        {
            Email = user.Email,
            UserId = user.Id.ToString(),
            IsTemporaryPassword = user.IsTemporaryPassword,
            IsActive = user.IsActive,
            LoginFlowDone = user.IsLoginFlowDone(),
            RoleNames = user.UserType.ToRoleNames()
        };

        if (loginResponse.LoginFlowDone.Required())
        {
            var expiresAt = DateTimeOffset.UtcNow
                .Add(authOptions.RequiredCookieExpiration)
                .DateTime;

            var claims = ITokenService.CreateCommonClaims(user.Id.ToString(), user.UserType);

            var tokenString = tokenService.Create(claims);

            context.Response.Cookies.Append(IAuthOptions.CookieName, tokenString, new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAt,
                Path = "/"
            });
        }

        return loginResponse;
    }
}
