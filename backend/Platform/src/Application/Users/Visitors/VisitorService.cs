using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Users.Visitors;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Visitors;

public class VisitorService : IVisitorService
{
    private readonly IVisitorRepository visitorRepository;
    private readonly IUnitOfWork unitOfWork;

    public VisitorService(IVisitorRepository visitorRepository, IUnitOfWork unitOfWork)
    {
        this.visitorRepository = visitorRepository;
        this.unitOfWork = unitOfWork;
    }

    public Task<PagedResult<Visitor>> GetAll(PagedQuery query, VisitorSearchRequest? request, CancellationToken ct)
    {
        return visitorRepository.GetAll(query, request, ct);
    }

    public async Task<IReadOnlyList<Visitor>> GetVisitorsAsync(IReadOnlyList<VisitorId> ids, CancellationToken ct)
    {
        var visitors = await visitorRepository.GetAsync(ids, ct);
        if (visitors.Count != ids.Count)
        {
            throw new NotFoundException("Часть посетителем не была найдена");
        }
        return visitors;
    }

    public async Task SetStatus(VisitorId id, bool status, CancellationToken ct)
    {
        var visitor = await visitorRepository.GetAsync(id, ct);
        visitor.User.SetActive(status);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public Task<Visitor> GetByUserIdAsync(IdentityUserId userId, CancellationToken ct)
    {
        return visitorRepository.GetByUserIdAsync(userId, ct);
    }
}
