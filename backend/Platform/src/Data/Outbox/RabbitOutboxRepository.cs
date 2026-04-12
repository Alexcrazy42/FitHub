using FitHub.Application.Outbox;
using FitHub.Domain.Outbox;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Outbox;

public class RabbitOutboxRepository : IOutboxRepository
{
    private readonly DataContext context;

    public RabbitOutboxRepository(DataContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(RabbitOutboxMessage message, CancellationToken ct)
    {
        await context.RabbitOutboxMessages.AddAsync(message, ct);
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
