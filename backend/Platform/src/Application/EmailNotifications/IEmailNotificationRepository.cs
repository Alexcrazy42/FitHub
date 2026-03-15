using FitHub.Common.Entities.Storage;
using FitHub.Domain.Notifications;

namespace FitHub.Application.EmailNotifications;

public interface IEmailNotificationRepository :
    IPendingRepository<EmailNotification, EmailNotificationId>
{
    Task<EmailNotification?> GetFirstNotSent(CancellationToken ct);
}
