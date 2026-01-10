namespace FitHub.RabbitMQ.Contracts;

/// <summary>
/// Контракт RabbitMQ
/// </summary>
public interface IRabbitMqContract
{
    /// <summary>
    /// Имя Exchange
    /// </summary>
    static abstract string ExchangeName { get; }


    /// <summary>
    /// Дефолтный Routing Key
    /// </summary>
    static abstract string DefaultRoutingKey { get; }

    /// <summary>
    /// Fallback Routing Key
    /// </summary>
    static abstract string FallbackRoutingKey { get; }

}
