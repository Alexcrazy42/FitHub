using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FitHub.Common.Entities;

/// <summary>
/// Объект не найден
/// </summary>
public sealed class NotFoundException : CommonException
{
    public NotFoundException(string? message, Exception? innerException = null) : base(message, innerException)
    {
    }

    public static T ThrowIfNull<T>([NotNull] T? value, string? message = default, [CallerArgumentExpression("value")] string? paramName = default)
        where T : class
        => value ?? throw new NotFoundException(message ?? $"Параметр {paramName} не может быть null");

    public static T ThrowIfNull<T>([NotNull] T? value, string? message = default, [CallerArgumentExpression("value")] string? paramName = default)
        where T : struct
        => value ?? throw new NotFoundException(message ?? $"Параметр {paramName} не может быть null");
}
