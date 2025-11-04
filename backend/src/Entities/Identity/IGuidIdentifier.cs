using System.Diagnostics.CodeAnalysis;

namespace FitHub.Common.Entities.Identity;

/// <summary>
/// Guid идентификатор сущности
/// </summary>
public interface IGuidIdentifier : IIdentifier
{
    /// <summary>
    /// Значение идентификатора
    /// </summary>
    Guid Value { get; }
}

/// <summary>
/// Типизированный идентификатор сущности
/// </summary>
public interface IGuidIdentifier<TId> : IGuidIdentifier, IEquatable<TId>, IParsable<TId>
    where TId : IGuidIdentifier<TId>
{
    /// <summary>
    /// Распарсить идентификатор, в случае неудачи вылетит исключение
    /// </summary>
    static abstract TId Parse(string? text);

    /// <summary>
    /// Распарсить идентификатор, в случае неудачи вылетит исключение
    /// </summary>
    static abstract TId Parse(Guid? guid);

    /// <summary>
    /// Попытаться распарсить идентификатор по его строковому представлению
    /// </summary>
    static abstract bool TryParse([NotNullWhen(true)] Guid? guid, [NotNullWhen(true)][MaybeNullWhen(false)] out TId result);

    /// <summary>
    /// Попытаться распарсить идентификатор по его строковому представлению
    /// </summary>
    static abstract bool TryParse([NotNullWhen(true)] string? text, [NotNullWhen(true)][MaybeNullWhen(false)] out TId result);
}
