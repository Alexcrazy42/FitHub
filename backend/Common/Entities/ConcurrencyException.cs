namespace FitHub.Common.Entities;

/// <summary>
/// Исключение конкуренции
/// </summary>
public sealed class ConcurrencyException : UnexpectedException
{
    public ConcurrencyException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public ConcurrencyException(Exception ex)
    {
    }

    public ConcurrencyException()
    {
    }
}
