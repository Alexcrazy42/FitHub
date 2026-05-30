namespace FitHub.Common.Entities;

/// <summary>
/// Исключение бизнес логики
/// </summary>
public sealed class LogicViolationException : CommonException
{
    public LogicViolationException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
