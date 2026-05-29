using FitHub.Common.Entities.Storage;
using FitHub.RabbitMQ.Configuration;
using FitHub.RabbitMQ.Producers;
using Microsoft.Extensions.Logging;

namespace FitHub.Application.Outbox;

public class OutboxPublisherService : IOutboxPublisherService
{
    private readonly IOutboxRepository outboxRepository;
    private readonly IBasicProducer<RabbitMqClusterOptions> producer;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<OutboxPublisherService> logger;

    public OutboxPublisherService(
        IOutboxRepository outboxRepository,
        IBasicProducer<RabbitMqClusterOptions> producer,
        IUnitOfWork unitOfWork,
        ILogger<OutboxPublisherService> logger)
    {
        this.outboxRepository = outboxRepository;
        this.producer = producer;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<OutboxPublishResult> PublishPendingAsync(int batchSize, CancellationToken ct)
    {
        var messages = await outboxRepository.GetPendingAsync(batchSize, ct);
        var publishedCount = 0;
        var failedCount = 0;

        foreach (var message in messages)
        {
            try
            {
                await producer.BasicPublishRawJsonAsync(message.ExchangeName, message.Payload, message.RoutingKey, ct);
                message.MarkPublished();
                publishedCount++;
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
                failedCount++;
                logger.LogError(ex, "Failed to publish Platform outbox message {MessageId}.", message.Id);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }

        return new OutboxPublishResult(publishedCount, failedCount);
    }
}
