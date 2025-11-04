namespace FitHub.Common.Entities;

/// <summary>
/// Исключение уникального констрэйнта в БД
/// </summary>
public sealed class UniqueConstraintException : UnexpectedException
{
    public UniqueConstraintException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

