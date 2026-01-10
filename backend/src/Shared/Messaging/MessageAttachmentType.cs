namespace FitHub.Shared.Messaging;

/// <summary>
/// Тип вложения к сообщению
/// </summary>
public enum MessageAttachmentType
{
    /// <summary>
    /// Создание группы
    /// </summary>
    CreateGroup,

    /// <summary>
    /// Фото
    /// </summary>
    Photo,

    /// <summary>
    /// Ссылка
    /// </summary>
    Link,

    /// <summary>
    /// Упоминание пользователя
    /// </summary>
    TagUser,

    /// <summary>
    /// Приглашения пользователя
    /// </summary>
    InviteUser,

    /// <summary>
    /// Исключение пользователя
    /// </summary>
    ExcludeUser
}
