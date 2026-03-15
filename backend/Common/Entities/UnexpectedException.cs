using System.Diagnostics.CodeAnalysis;

namespace FitHub.Common.Entities;

/// <summary>
/// Неожиданное исключение
/// </summary>
/// <remarks>
/// Должно возникать в исключительных ситуациях, например когда
/// данные перестали быть согласованы в БД
/// </remarks>
public class UnexpectedException : CommonException
{
    public UnexpectedException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    public static T ThrowIfNull<T>([NotNull] T? value, string message)
        where T : class
        => value ?? throw new UnexpectedException(message);

    public static T ThrowIfNull<T>([NotNull] T? value, string message)
        where T : struct
        => value ?? throw new UnexpectedException(message);

    public static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new UnexpectedException(message);
        }
    }
}
