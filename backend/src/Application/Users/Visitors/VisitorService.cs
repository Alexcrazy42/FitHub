using FitHub.Application.Common;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.Visitors;

public class VisitorService : IVisitorService
{
    private readonly IVisitorRepository visitorRepository;

    public VisitorService(IVisitorRepository visitorRepository)
    {
        this.visitorRepository = visitorRepository;
    }

    public Task<PagedResult<Visitor>> GetAll(PagedQuery query, CancellationToken ct)
    {
        return visitorRepository.GetAll(query, ct);
    }
}
