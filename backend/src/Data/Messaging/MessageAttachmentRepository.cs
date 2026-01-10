using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;

namespace FitHub.Data.Messaging;

internal sealed class MessageAttachmentRepository : DefaultPendingRepository<MessageAttachment, MessageAttachmentId, DataContext>, IMessageAttachmentRepository
{
    public MessageAttachmentRepository(DataContext context) : base(context)
    {
    }
}
