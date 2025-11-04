namespace FitHub.Common.Entities;

/// <summary>
/// Ошибка валидации определенного свойства
/// </summary>
public class ValidationError
{
    public ValidationError(string message, string propertyName)
    {
        Message = message;
        PropertyName = propertyName;
    }

    public string Message { get; set; }
    public string PropertyName { get; set; }
}
