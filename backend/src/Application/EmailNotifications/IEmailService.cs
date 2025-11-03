namespace FitHub.Application.EmailNotifications;

public interface IEmailService
{
    Task SendEmailAsync(Email email, CancellationToken ct);
}

public record Email(IReadOnlyList<string> Recipients, string Subject, string? BodyText, string? BodyHtml);
