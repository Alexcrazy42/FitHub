using Messaging.Kafka;

namespace KafkaApi;

public class OrderCreatedHandler : IMessageHandler<string, OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger) =>
        _logger = logger;

    public async Task HandleAsync(string key, OrderCreatedEvent value, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
        _logger.LogInformation("Order {OrderId} created123", value.OrderId);
    }
}