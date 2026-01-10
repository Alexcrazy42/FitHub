using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.CmsAdmins;

public class CmsAdminService : ICmsAdminService
{
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;

    public CmsAdminService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        this.userRepository = userRepository;
        this.unitOfWork = unitOfWork;
    }

    public Task<PagedResult<User>> GetCmsAdmins(PagedQuery query, CancellationToken ct)
    {
        return userRepository.GetCmsAdminsAsync(query, ct);
    }

    public async Task SetStatus(IdentityUserId id, bool status, CancellationToken ct)
    {
        var user = await userRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(user, "Cms Admin не найден!");
        user.SetActive(status);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
