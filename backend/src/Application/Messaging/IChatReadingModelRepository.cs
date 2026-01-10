using FitHub.Application.Common;
using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IChatReadingModelRepository : IPendingRepository<ChatReadingModel, ChatReadingModelId>
{
    Task<PagedResult<ChatReadingModel>> GetChatReadingModelAsync(PagedQuery paged, IdentityUserId userId, CancellationToken ct);
}
