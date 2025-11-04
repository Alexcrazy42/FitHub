using FitHub.Application.Users;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;

namespace FitHub.Data.Users;

public class UserRepository : DefaultPendingRepository<User, IdentityUserId, DataContext>, IUserRepository
{
    public UserRepository(DataContext context) : base(context)
    {
    }
}
