using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users;

public interface IUserRepository : IPendingRepository<User, IdentityUserId>;
