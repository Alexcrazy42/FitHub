using Microsoft.Extensions.Logging;

namespace FitHub.Application.EmailNotifications;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> logger;

    public EmailService(ILogger<EmailService> logger)
    {
        this.logger = logger;
    }

    public Task SendEmailAsync(Email email, CancellationToken ct)
    {
        logger.LogInformation("Sending email {Email}", email);
        return Task.CompletedTask;
    }
}
