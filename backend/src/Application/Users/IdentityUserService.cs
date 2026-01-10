using FitHub.Application.Common;
using FitHub.Application.EmailNotifications;
using FitHub.Application.Equipments.Gyms;
using FitHub.Application.Users.Commands;
using FitHub.Application.Users.GymAdmins;
using FitHub.Application.Users.Trainers;
using FitHub.Application.Users.Visitors;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.AspNetCore.Tokens;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Auth;
using FitHub.Contracts.V1.Users.GymAdmins;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Domain.Equipments;
using FitHub.Domain.Notifications;
using FitHub.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
    private readonly HttpContext context;
    private readonly IAuthOptions authOptions;
    private readonly ITokenService tokenService;
    private readonly ILogger<IdentityUserService> logger;
    private readonly ISessionService sessionService;
    private readonly IVisitorRepository visitorRepository;
    private readonly IGymService gymService;
    private readonly IVisitorGymRelationRepository visitorGymRelationRepository;

    public IdentityUserService(IUserRepository userRepository,
        IGymAdminRepository gymAdminRepository,
        IUnitOfWork unitOfWork,
        IGymRepository gymRepository,
        IEmailNotificationRepository emailNotificationRepository,
        ITokenRepository tokenRepository,
        ITrainerRepository trainerRepository,
        IHttpContextAccessor httpContextAccessor,
        IOptions<IAuthOptions> authOptions,
        ITokenService tokenService,
        ILogger<IdentityUserService> logger,
        ISessionService sessionService,
        IVisitorRepository visitorRepository,
        IGymService gymService,
        IVisitorGymRelationRepository visitorGymRelationRepository)
    {
        this.userRepository = userRepository;
        this.gymAdminRepository = gymAdminRepository;
        this.unitOfWork = unitOfWork;
        this.gymRepository = gymRepository;
        this.emailNotificationRepository = emailNotificationRepository;
        this.tokenRepository = tokenRepository;
        this.trainerRepository = trainerRepository;
        context = httpContextAccessor.HttpContext ?? throw new ArgumentException("Не получается получить HttpContext");
        this.tokenService = tokenService;
        this.logger = logger;
        this.sessionService = sessionService;
        this.visitorRepository = visitorRepository;
        this.gymService = gymService;
        this.visitorGymRelationRepository = visitorGymRelationRepository;
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

    public async Task<IdentityUser> GetAsync(IdentityUserId id, CancellationToken cancellationToken)
    {
        var user = await GetOrDefaultAsync(id, cancellationToken);
        NotFoundException.ThrowIfNull(user, "Пользователь не найден!");
        return user;
    }

    public Task<bool> IsSessionValid(IdentityUserId userId, string sessionId, CancellationToken ct = default)
    {
        var parsedId = SessionId.Parse(sessionId);
        return sessionService.IsSessionValid(parsedId, userId, ct);
    }

    public async Task<User> GetUserAsync(IdentityUserId userId, CancellationToken ct)
    {
        var user = await userRepository.GetFirstOrDefaultAsync(x => x.Id == userId, ct);
        NotFoundException.ThrowIfNull(user, "Пользователь не найден!");
        return user;
    }

    public async Task<IReadOnlyList<User>> GetUsersAsync(List<IdentityUserId> userIds, CancellationToken ct)
    {
        var users = await userRepository.GetUsersAsync(userIds, ct);
        if (users.Count != userIds.Count)
        {
            throw new NotFoundException("Не найдены все пользователи");
        }

        return users;
    }

    public async Task<User> StartRegister(StartRegisterRequest request, CancellationToken ct)
    {
        var gymId = GymId.Parse(request.GymId);

        var gym = await gymService.GetByIdAsync(gymId, ct);

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
            userType: IdentityUserType.GymVisitor,
            startRegistrationAt: DateTimeOffset.UtcNow,
            lastSeenAt: DateTimeOffset.UtcNow
        );

        var visitor = Visitor.Create(user);
        var visitorGymRelation = VisitorGymRelation.Create(gym, visitor, true);

        var token = Token.Create(user, TokenType.ConfirmEmail);

        var notification = EmailNotification.Create(email, "Подтвердите свою почту", bodyHtml: $"userId: {user.Id} token: {token.TokenString}");

        logger.LogInformation("Create notification: {Notification}", notification);

        await userRepository.PendingAddAsync(user, ct);
        await emailNotificationRepository.PendingAddAsync(notification, ct);
        await tokenRepository.PendingAddAsync(token, ct);
        await visitorRepository.PendingAddAsync(visitor, ct);
        await visitorGymRelationRepository.PendingAddAsync(visitorGymRelation, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
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

        logger.LogInformation("Create notification: {Notification}", notification);

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

        var gymAdmin = GymAdmin.Create(user);
        gymAdmin.SetGym(gym);

        var token = Token.Create(user, TokenType.ConfirmEmail);

        var notification = EmailNotification.Create(email, "Подтвердите свою почту", bodyHtml: $"userId: {user.Id} token: {token.TokenString}");

        logger.LogInformation("Create notification: {Notification}", notification);

        await userRepository.PendingAddAsync(user, ct);
        await gymAdminRepository.PendingAddAsync(gymAdmin, ct);
        await emailNotificationRepository.PendingAddAsync(notification, ct);
        await tokenRepository.PendingAddAsync(token, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<User> RegisterTrainerAsync(CreateTrainerRequest request, CancellationToken ct = default)
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
            userType: IdentityUserType.Trainer,
            startRegistrationAt: DateTimeOffset.UtcNow,
            lastSeenAt: DateTimeOffset.UtcNow
        );

        var trainer = Trainer.Create(user);
        trainer.SetGym(gym);

        var token = Token.Create(user, TokenType.ConfirmEmail);

        var notification = EmailNotification.Create(email, "Подтвердите свою почту", bodyHtml: $"userId: {user.Id} token: {token.TokenString}");

        logger.LogInformation("Create notification: {Notification}", notification);

        await userRepository.PendingAddAsync(user, ct);
        await emailNotificationRepository.PendingAddAsync(notification, ct);
        await tokenRepository.PendingAddAsync(token, ct);
        await trainerRepository.PendingAddAsync(trainer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<bool> CheckConfirmEmail(ConfirmEmailRequest request, CancellationToken ct = default)
    {
        var userId = IdentityUserId.Parse(request.UserId);
        var user = await GetAsync(userId, ct);

        await CheckToken(request.Token.Required(), TokenType.ConfirmEmail, user, ct);
        return true;
    }

    public async Task<LoginResponse> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken ct = default)
    {
        var userId = IdentityUserId.Parse(request.UserId);

        var user = await GetAsync(userId, ct);

        await CheckToken(request.Token.Required(), TokenType.ConfirmEmail, user, ct);

        user.SetEmailConfirmed(true);
        await unitOfWork.SaveChangesAsync(ct);

        return await GetLoginResponse(user, ct);
    }

    public async Task<LoginResponse> SetPasswordAsync(SetPasswordRequest request, CancellationToken ct = default)
    {
        var userId = IdentityUserId.Parse(request.UserId);

        var user = await GetAsync(userId, ct);

        if (!user.IsTemporaryPassword)
        {
            throw new AlreadyExistsException("Пароль не является временным!");
        }

        var token = await CheckToken(request.Token.Required(), TokenType.ConfirmEmail, user, ct);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password.Required()).Required();
        user.SetPassword(passwordHash);
        user.SetActive(true);
        user.SetActiveAt(DateTimeOffset.UtcNow);
        token.SetAppliedAt(DateTimeOffset.UtcNow);

        await unitOfWork.SaveChangesAsync(ct);
        return await GetLoginResponse(user, ct);
    }

    public async Task InitResetPasswordAsync(string email, CancellationToken ct = default)
    {
        var user = await userRepository.GetFirstOrDefaultAsync(x => x.Email == email, ct);
        NotFoundException.ThrowIfNull(user, "Пользователь не найден!");

        var token = Token.Create(user, TokenType.ResetPassword);
        var notification = EmailNotification.Create(user.Email, "Смена пароля", $"UserId: {user.Id} token: {token.TokenString}");

        logger.LogInformation("Create notification: {Notification}", notification);

        await tokenRepository.PendingAddAsync(token, ct);
        await emailNotificationRepository.PendingAddAsync(notification, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<bool> CheckResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var userId = IdentityUserId.Parse(request.UserId);
        var user = await GetAsync(userId, ct);
        await CheckToken(request.Token.Required(), TokenType.ResetPassword, user, ct);
        return true;
    }

    public async Task<LoginResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        await CheckResetPasswordAsync(request, ct);

        var userId = IdentityUserId.Parse(request.UserId);
        var user = await GetAsync(userId, ct);

        var token = await CheckToken(request.Token.Required(), TokenType.ResetPassword, user, ct);

        if (BCrypt.Net.BCrypt.Verify(request.NewPassword.Required(), user.PasswordHash))
        {
            throw new ValidationException("Пароль не может быть старым!");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword.Required()).Required();
        user.SetPassword(passwordHash);
        token.SetAppliedAt(DateTimeOffset.UtcNow);

        await unitOfWork.SaveChangesAsync(ct);
        return await GetLoginResponse(user, ct);
    }

    public async Task Logout(IdentityUserId userId, SessionId sessionId, CancellationToken ct = default)
    {
        var session = await sessionService.Get(sessionId, ct);
        session.SetActive(false);
        await unitOfWork.SaveChangesAsync(ct);
        context.Response.Cookies.Delete(IAuthOptions.CookieName);
    }

    public Task<PagedResult<User>> GetUsersAsync(GetUserQuery query, PagedQuery pagedQuery, CancellationToken ct = default)
    {
        return userRepository.GetPagedUsersAsync(query, pagedQuery, ct);
    }

    public async Task StartOnlineAt(IdentityUserId userId, CancellationToken ct)
    {
        var user = await GetUserAsync(userId, ct);

        user.SetOnline();

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<DateTimeOffset> EndOnlineAt(IdentityUserId userId, CancellationToken ct)
    {
        var user = await GetUserAsync(userId, ct);

        var lastSeen = user.SetOffline();

        await unitOfWork.SaveChangesAsync(ct);

        return lastSeen;
    }

    public async Task<LoginResponse> LoginAsync(string login, string password, CancellationToken cancellationToken)
    {
        var user = await GetByEmailAsync(login, cancellationToken);
        NotFoundException.ThrowIfNull(user, "Пользователь не найден!");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new ValidationException("Пароль неверен!");
        }

        return await GetLoginResponse(user, cancellationToken);
    }

    private async Task<LoginResponse> GetLoginResponse(IdentityUser user, CancellationToken ct)
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

            var userEntity = await userRepository.GetFirstOrDefaultAsync(x => x.Id == user.Id, ct);
            NotFoundException.ThrowIfNull(userEntity);

            var session = Session.Create(userEntity, true, expiresAt);

            await sessionService.Create(session, ct);

            var claims = ITokenService.CreateCommonClaims(user.Id.ToString(), userEntity.GetFullName(), session.Id.ToString(), user.UserType);

            var tokenString = tokenService.Create(claims);

            loginResponse.JwtToken = tokenString;

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

    private async Task<Token> CheckToken(string token, TokenType tokenType, IdentityUser user, CancellationToken ct)
    {
        var tokenEntity = await tokenRepository.GetFirstOrDefaultAsync(x => x.TokenType == tokenType && x.UserId == user.Id && x.TokenString == token, ct);

        if (tokenEntity is not null && tokenEntity.ExpiresOn > DateTimeOffset.UtcNow && tokenEntity.AppliedAt is null)
        {
            return tokenEntity;
        }

        throw new NotFoundException("Не найден нужный токен!");
    }
}
