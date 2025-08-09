namespace FitHub.Entities.Identity;

/// <summary>
/// Описание идентификатора
/// </summary>
public interface IIdentifierDescription
{
    /// <summary>
    /// Имя сущности, к которой относится идентификатор
    /// </summary>
    static abstract string EntityTypeName { get; }

    /// <summary>
    /// Префикс для значения идентификатора
    /// </summary>
    /// <remarks>
    /// Свойство должно быть реализовано явно.
    /// Для формирования значения свойства в наследниках необходимо использовать
    /// методы <c>FormatPrefix</c>
    /// </remarks>
    static abstract string Prefix { get; }
}
