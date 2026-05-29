namespace FitHub.BankManager.Application.Payments;

public interface IBankManagerOutboxPublisherService
{
    Task<BankManagerOutboxPublishResult> PublishPendingAsync(int batchSize, CancellationToken ct);
}

public record BankManagerOutboxPublishResult(int PublishedCount, int FailedCount);
