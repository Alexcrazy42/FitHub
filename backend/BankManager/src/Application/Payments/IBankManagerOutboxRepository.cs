using FitHub.BankManager.Domain;

namespace FitHub.BankManager.Application.Payments;

public interface IBankManagerOutboxRepository
{
    Task<IReadOnlyList<RabbitOutboxMessage>> GetPendingAsync(int batchSize, CancellationToken ct);
}
