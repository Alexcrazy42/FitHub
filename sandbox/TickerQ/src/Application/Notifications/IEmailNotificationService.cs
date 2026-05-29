
namespace TickerQ.Jobs.Application.Notifications;

public interface IEmailNotificationService
{
    Task SendEmailAsync(
        IReadOnlyList<string> recipients,
        IReadOnlyList<string> bccRecipients,
        string subject, string htmlBody,
        CancellationToken cancellationToken = default);
}
