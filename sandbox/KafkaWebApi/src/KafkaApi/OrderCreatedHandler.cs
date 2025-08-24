using Messaging.Kafka;

namespace KafkaApi;

public class OrderCreatedHandler : IMessageHandler<string, OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger) =>
        _logger = logger;

    public Task HandleAsync(string key, OrderCreatedEvent value, CancellationToken ct)
    {
        _logger.LogInformation("Order {OrderId} created", value.OrderId);
        return Task.CompletedTask;
    }
}