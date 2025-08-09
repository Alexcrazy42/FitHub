using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FitHub.Utilities.System;

namespace FitHub.Entities.Identity;

/// <summary>
/// Базовый класс типизированного идентификатора
/// </summary>
/// <typeparam name="TId">Тип идентификатора</typeparam>
public abstract class GuidIdentifier<TId> : IGuidIdentifier<TId>
    where TId : GuidIdentifier<TId>, IIdentifierDescription
{
    private static readonly ConcurrentDictionary<Type, Func<Guid, TId>> Activators = new();

    private const char PrefixSeparator = '-';

    /// <remarks>
    /// Конструкторы всех наследников должны
    /// 1. Быть private
    /// 2. Принимать в качестве единственного параметра строковое значение
    ///
    /// Они требуются для вызова из этого класса
    /// </remarks>
    protected GuidIdentifier(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public override string ToString() => Value.ToString();

    /// <summary>
    /// Сформировать приставку для идентификатора сущности
    /// </summary>
    /// <param name="shortServiceName">Сокращённое имя сервиса</param>
    /// <param name="shortEntityTypeName">Сокращённый тип сущности, использующий идентификатор</param>
    protected static string FormatPrefix(string shortServiceName, string shortEntityTypeName)
        => $"{shortServiceName}{PrefixSeparator}{shortEntityTypeName}{PrefixSeparator}";

    public static TId New()
    {
        var value = Guid.CreateVersion7();
        var activator = Activators.GetOrAdd(typeof(TId), _ => ExpressionActivator.BuildFactory<Guid, TId>());

        return activator(value);
    }

    public static TId Parse(string? text) => Parse(text, null);

    public static TId Parse(string? text, IFormatProvider? provider)
        => TryParse(text, provider, out var id)
            ? id
            : throw new ValidationException($"Не удается распарсить идентификатор сущности \"{typeof(TId).Name}\": {text}");

    public static bool TryParse([NotNullWhen(true)] string? text, [NotNullWhen(true)][MaybeNullWhen(false)] out TId result)
        => TryParse(text, null, out result);

    public static bool TryParse([NotNullWhen(true)] string? text, IFormatProvider? provider, [NotNullWhen(true)][MaybeNullWhen(false)] out TId result)
    {
        text = text?.Trim();

        if (String.IsNullOrWhiteSpace(text)
            || !Guid.TryParse(text, out var value))
        {
            result = null;
            return false;
        }

        var activator = Activators.GetOrAdd(typeof(TId), _ => ExpressionActivator.BuildFactory<Guid, TId>());

        result = activator(value);
        return true;
    }

    public static TId Parse(Guid? guid) => Parse(guid, null);

    public static TId Parse(Guid? guid, IFormatProvider? provider)
        => TryParse(guid, provider, out var id)
            ? id
            : throw new ValidationException($"Не удается распарсить идентификатор сущности \"{typeof(TId).Name}\": {guid}");


    public static bool TryParse([NotNullWhen(true)] Guid? guid, [NotNullWhen(true)][MaybeNullWhen(false)] out TId result)
        => TryParse(guid, null, out result);

    public static bool TryParse([NotNullWhen(true)] Guid? guid, IFormatProvider? provider, [NotNullWhen(true)][MaybeNullWhen(false)] out TId result)
    {
        if (guid is null)
        {
            result = null;
            return false;
        }

        var activator = Activators.GetOrAdd(typeof(TId), _ => ExpressionActivator.BuildFactory<Guid, TId>());

        result = activator(guid.Value);
        return true;
    }

    public bool Equals(TId? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as TId);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(GuidIdentifier<TId>? left, GuidIdentifier<TId>? right) => Equals(left, right);

    public static bool operator !=(GuidIdentifier<TId>? left, GuidIdentifier<TId>? right) => !Equals(left, right);
}
