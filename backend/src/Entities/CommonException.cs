namespace FitHub.Common.Entities;

/// <summary>
/// Исключение из нашей логики (либовской или бизнес)
/// </summary>
public class CommonException : Exception
{
    public CommonException(string? message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
