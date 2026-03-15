using FitHub.RabbitMQ.Contracts;

namespace FitHub.RabbitMQ.Producers;

/// <summary>
/// Интерфейс продюсера
/// </summary>
public interface IRabbitProducer<T> where T : class, IRabbitMqContract
{
    /// <summary>
    /// Запаблишить сообщение
    /// </summary>
    Task BasicPublishAsync(T message, CancellationToken ct = default);
}
