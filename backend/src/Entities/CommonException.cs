namespace FitHub.Common.Entities;

/// <summary>
/// Исключение из нашей логики (либовской или бизнес)
/// </summary>
public abstract class CommonException : Exception
{
    protected CommonException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
