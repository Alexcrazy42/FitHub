namespace TickerQ.Jobs.Application.Notifications;

internal sealed class EmailNotificationService : IEmailNotificationService
{
    public Task SendEmailAsync(
        IReadOnlyList<string> recipients,
        IReadOnlyList<string> bccRecipients,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{DateTime.Now}] Sent email to recipients " +
                          $"[{String.Join(',', recipients)} + {String.Join(',', bccRecipients)}]: " +
                          $"{subject}\n\n" +
                          $"{htmlBody}");

        return Task.CompletedTask;
    }
}
