namespace FitHub.Common.Entities;

/// <summary>
/// Подробное исключение валидации
/// </summary>
public class DetailedValidationException : CommonException
{
    public IReadOnlyList<ValidationError> Errors { get; set; } = [];

    public DetailedValidationException(string? message, Exception? innerException = null) : base(message, innerException)
    {
    }

    /// <summary>
    /// Создаёт исключение на основе списка ошибок с внутренним исключением.
    /// </summary>
    public DetailedValidationException(IReadOnlyList<ValidationError> errors, Exception? innerException = null)
        : base(FormatMessage(errors), innerException)
    {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }

    // Вспомогательный метод для форматирования сообщения из списка ошибок.
    private static string FormatMessage(IReadOnlyList<ValidationError> errors)
    {
        return errors.Count switch
        {
            0 => "Произошла ошибка валидации.",
            1 => errors[0].Message,
            _ => "Ошибки валидации:\n" + String.Join("\n", errors.Select(e => $"- {e.Message}"))
        };
    }
}
