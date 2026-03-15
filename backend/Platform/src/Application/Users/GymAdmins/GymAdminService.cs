using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.GymAdmins;

public class GymAdminService : IGymAdminService
{
    private readonly IGymAdminRepository gymAdminRepository;
    private readonly IUnitOfWork unitOfWork;

    public GymAdminService(IGymAdminRepository gymAdminRepository, IUnitOfWork unitOfWork)
    {
        this.gymAdminRepository = gymAdminRepository;
        this.unitOfWork = unitOfWork;
    }

    public Task<PagedResult<GymAdmin>> GetAll(PagedQuery query, CancellationToken ct)
    {
        return gymAdminRepository.GetAll(query, ct);
    }

    public async Task SetStatus(GymAdminId gymAdminId, bool status, CancellationToken ct)
    {
        var gymAdmin = await gymAdminRepository.GetAsync(gymAdminId, ct);
        gymAdmin.User.SetActive(status);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public Task<GymAdmin> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct)
    {
        return gymAdminRepository.GetByUserIdAsync(userId, ct);
    }
}
