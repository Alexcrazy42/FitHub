namespace FitHub.Common.Entities;

/// <summary>
/// Сушность уже существует
/// </summary>
public sealed class AlreadyExistsException : CommonException
{
    public AlreadyExistsException(string? message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
