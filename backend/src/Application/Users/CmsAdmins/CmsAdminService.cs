using FitHub.Application.Common;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.CmsAdmins;

public class CmsAdminService : ICmsAdminService
{
    private readonly IUserRepository userRepository;

    public CmsAdminService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public Task<PagedResult<User>> GetCmsAdmins(PagedQuery query, CancellationToken ct)
    {
        return userRepository.GetCmsAdminsAsync(query, ct);
    }
}
