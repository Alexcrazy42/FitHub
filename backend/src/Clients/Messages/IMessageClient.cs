using FitHub.Contracts;
using FitHub.Contracts.V1.Messaging.Messages;

namespace FitHub.Clients.Messages;

/// <summary>
/// Апи клиент для работы с сообщения
/// </summary>
public interface IMessageClient
{
    /// <summary>
    /// Получить сообщения
    /// </summary>
    Task<ListResponse<MessageResponse>> GetMessagesAsync(GetMessagesRequest request, PagedRequest paged, CancellationToken ct);

    /// <summary>
    /// Создать сообщение
    /// </summary>
    Task<MessageResponse?> CreateMessageAsync(CreateMessageRequest request, CancellationToken ct);

    /// <summary>
    /// Обновить сообщение
    /// </summary>
    Task<MessageResponse?> UpdateMessageAsync(string id, UpdateMessageRequest request, CancellationToken ct);

    /// <summary>
    /// Удалить сообщение
    /// </summary>
    Task DeleteMessageAsync(string id, CancellationToken ct);
}
