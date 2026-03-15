namespace FitHub.Common.Entities;

/// <summary>
/// Исключение доступа
/// </summary>
public sealed class ForbidException : CommonException
{
    public ForbidException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

