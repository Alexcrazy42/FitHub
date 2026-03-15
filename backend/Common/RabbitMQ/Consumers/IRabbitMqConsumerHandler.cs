using FitHub.RabbitMQ.Contracts;

namespace FitHub.RabbitMQ.Consumers;

/// <summary>
/// Handler сообщения из RabbitMq
/// </summary>
public interface IRabbitMqConsumerHandler<T> where T : class, IRabbitMqContract
{
    /// <summary>
    /// Метод обработки сообщения
    /// </summary>
    Task HandleAsync(T message, CancellationToken ct);
}
