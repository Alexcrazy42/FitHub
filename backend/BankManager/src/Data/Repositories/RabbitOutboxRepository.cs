using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Domain;
using Microsoft.EntityFrameworkCore;

namespace FitHub.BankManager.Data.Repositories;

public class RabbitOutboxRepository : IBankManagerOutboxRepository
{
    private readonly BankManagerDataContext context;

    public RabbitOutboxRepository(BankManagerDataContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<RabbitOutboxMessage>> GetPendingAsync(int batchSize, CancellationToken ct)
    {
        return await context.RabbitOutboxMessages
            .Where(x => x.Status == OutboxMessageStatus.Pending || x.Status == OutboxMessageStatus.Failed)
            .OrderBy(x => x.CreatedAt)
            .Take(batchSize)
            .ToListAsync(ct);
    }
}
