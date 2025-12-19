using FitHub.RabbitMQ.Configuration;

namespace FitHub.RabbitMQ.Producers;

/// <summary>
/// Базовый продюсер
/// </summary>
public interface IBasicProducer<TOptions>
    where TOptions : class, IRabbitMqOptions
{
    /// <summary>
    /// Опубликовать сообщение
    /// </summary>
    Task BasicPublishAsync(string exchangeName, object message, string routingKey, CancellationToken ct = default);
}
