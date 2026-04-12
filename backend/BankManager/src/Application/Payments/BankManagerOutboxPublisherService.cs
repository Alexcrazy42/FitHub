using FitHub.RabbitMQ.Configuration;
using FitHub.RabbitMQ.Producers;
using Microsoft.Extensions.Logging;

namespace FitHub.BankManager.Application.Payments;

public class BankManagerOutboxPublisherService : IBankManagerOutboxPublisherService
{
    private readonly IBankManagerOutboxRepository outboxRepository;
    private readonly IBasicProducer<RabbitMqClusterOptions> producer;
    private readonly IBankManagerUnitOfWork unitOfWork;
    private readonly ILogger<BankManagerOutboxPublisherService> logger;

    public BankManagerOutboxPublisherService(
        IBankManagerOutboxRepository outboxRepository,
        IBasicProducer<RabbitMqClusterOptions> producer,
        IBankManagerUnitOfWork unitOfWork,
        ILogger<BankManagerOutboxPublisherService> logger)
    {
        this.outboxRepository = outboxRepository;
        this.producer = producer;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<BankManagerOutboxPublishResult> PublishPendingAsync(int batchSize, CancellationToken ct)
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
                logger.LogError(ex, "Failed to publish BankManager outbox message {MessageId}.", message.Id);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }

        return new BankManagerOutboxPublishResult(publishedCount, failedCount);
    }
}
