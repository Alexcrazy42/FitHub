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
    /// Routing Key на Exchange
    /// </summary>
    static abstract string ExchangeRoutingKey { get; }
}
