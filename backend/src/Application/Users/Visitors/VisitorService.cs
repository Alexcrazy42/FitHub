using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
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

    public Task<PagedResult<Visitor>> GetAll(PagedQuery query, CancellationToken ct)
    {
        return visitorRepository.GetAll(query, ct);
    }

    public async Task SetStatus(VisitorId id, bool status, CancellationToken ct)
    {
        var visitor = await visitorRepository.GetAsync(id, ct);
        visitor.User.SetActive(status);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
