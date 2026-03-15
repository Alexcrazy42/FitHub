using FitHub.Application.EmailNotifications;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.EmailNotifications;

public class EmailNotificationRepository :
    DefaultPendingRepository<EmailNotification, EmailNotificationId, DataContext>,
    IEmailNotificationRepository
{
    public EmailNotificationRepository(DataContext context) : base(context)
    {
    }

    public Task<EmailNotification?> GetFirstNotSent(CancellationToken ct)
    {
        return ReadRaw()
            .OrderBy(x => x.CreatedAt)
            .Where(x => x.SentAt == null)
            .FirstOrDefaultAsync(ct);
    }
}
