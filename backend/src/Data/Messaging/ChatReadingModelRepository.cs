using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;

namespace FitHub.Data.Messaging;

internal sealed class ChatReadingModelRepository : DefaultPendingRepository<ChatReadingModel, ChatReadingModelId, DataContext>, IChatReadingModelRepository
{
    public ChatReadingModelRepository(DataContext context) : base(context)
    {
    }
}
