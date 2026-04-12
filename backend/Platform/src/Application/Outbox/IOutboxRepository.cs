using FitHub.Domain.Outbox;

namespace FitHub.Application.Outbox;

public interface IOutboxRepository
{
    Task AddAsync(RabbitOutboxMessage message, CancellationToken ct);

    Task<IReadOnlyList<RabbitOutboxMessage>> GetPendingAsync(int batchSize, CancellationToken ct);
}
