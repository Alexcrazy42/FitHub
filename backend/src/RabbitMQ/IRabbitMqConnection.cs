using FitHub.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace FitHub.RabbitMQ;

/// <summary>
/// Конекшн с рэббитом
/// </summary>
public interface IRabbitMqConnection<TOptions>
    where TOptions : IRabbitMqOptions
{
    /// <summary>
    /// Создать канал подключения
    /// </summary>
    Task<IChannel> CreateChannelAsync();
}
