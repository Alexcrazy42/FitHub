using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Utilities.System;

namespace FitHub.RabbitMQ.Configuration;

/// <summary>
/// Опции подключения к кластеру
/// </summary>
public interface IRabbitMqOptions : IHaveConfigSection
{
    static string IHaveConfigSection.SectionName => "RabbitMQ";

    /// <summary>
    /// Список нод кластера
    /// </summary>
    public IReadOnlyList<string> Nodes { get; }

    /// <summary>
    /// Нужно ли подготавливать очереди, экчейнжи и биндинги
    /// </summary>
    public bool NeedToPrepare { get; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public string? Username { get; }

    /// <summary>
    /// Пароль
    /// </summary>
    public string? Password { get; }

    /// <summary>
    /// Виртуальный хост
    /// </summary>
    public string? VirtualHost { get; }

    public int Port => 5672;

    public TimeSpan NetworkRecoveryInterval => TimeSpan.FromSeconds(5);

    public IReadOnlyList<string> NodesRequired => Nodes.Count != 0 ? Nodes : throw new ArgumentException("Nodes count must be greater 0");
    public string UsernameRequired => Username.Required();
    public string PasswordRequired => Password.Required();
    public string VirtualHostRequired => VirtualHost ?? "/";
}
