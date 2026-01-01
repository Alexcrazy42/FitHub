using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;

namespace FitHub.Data.Messaging;

internal sealed class MessageViewRepository : DefaultPendingRepository<MessageView, MessageViewId, DataContext>, IMessageViewRepository
{
    public MessageViewRepository(DataContext context) : base(context)
    {
    }
}
