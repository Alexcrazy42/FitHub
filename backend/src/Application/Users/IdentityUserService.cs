using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public class IdentityUserService : IIdentityUserService, IUserService, IAuthenticationService
{
    private readonly IUserRepository userRepository;
    private readonly IGymAdminRepository gymAdminRepository;
    private readonly IUnitOfWork unitOfWork;

    public IdentityUserService(IUserRepository userRepository,
        IGymAdminRepository gymAdminRepository,
        IUnitOfWork unitOfWork)
    {
        this.userRepository = userRepository;
        this.gymAdminRepository = gymAdminRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await userRepository.GetFirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<IdentityUser?> GetOrDefaultAsync(IdentityUserId id, CancellationToken cancellationToken)
    {
        return await userRepository.GetFirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<User> RegisterAdminAsync(CancellationToken ct = default)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password");
        throw new NotImplementedException();
    }

    public Task<IdentityUser?> LoginAsync(string login, string password, CancellationToken cancellationToken)
    {
        return Task.FromResult<IdentityUser?>(new IdentityUser(
            IdentityUserId.Parse(Guid.NewGuid()),
            "nick",
            "email@mail.ru",
            password,
            IdentityUserType.GymAdmin | IdentityUserType.GymVisitor | IdentityUserType.CmsAdmin
        ));

        // var user = await userRepository.GetFirstOrDefaultAsync(x => x.Email == login || x.Nickname == login, cancellationToken);
        //
        // if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        // {
        //     return null;
        // }
        //
        // return user;
    }
}
