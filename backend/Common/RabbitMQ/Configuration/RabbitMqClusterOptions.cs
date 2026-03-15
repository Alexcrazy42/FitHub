namespace FitHub.RabbitMQ.Configuration;

/// <summary>
/// Опции кластера RabbitMQ
/// </summary>
public sealed class RabbitMqClusterOptions : IRabbitMqOptions
{
    public IReadOnlyList<string> Nodes { get; set; } = [];
    public bool NeedToPrepare { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? VirtualHost { get; set; }
}
