using FitHub.Contracts.V1.Messaging.Chats;

namespace FitHub.Clients.Chats;

/// <summary>
/// Апи клиент для работы с чатами
/// </summary>
public interface IChatClient
{
    /// <summary>
    /// Получить чат
    /// </summary>
    Task<ChatResponse?> GetChatAsync(string chatId, CancellationToken ct);

    /// <summary>
    /// Создать чат
    /// </summary>
    Task<ChatResponse?> CreateChatAsync(CreateChatRequest request, CancellationToken ct);
}
