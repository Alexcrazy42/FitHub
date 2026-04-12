namespace FitHub.Application.Outbox;

public interface IOutboxPublisherService
{
    Task<OutboxPublishResult> PublishPendingAsync(int batchSize, CancellationToken ct);
}

public record OutboxPublishResult(int PublishedCount, int FailedCount);
