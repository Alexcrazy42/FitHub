using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Notifications;

public class EmailNotification : IEntity<EmailNotificationId>, IAuditableEntity
{
    private EmailNotification(EmailNotificationId id, string toEmail, string subject)
    {
        Id = id;
        ToEmail = toEmail;
        Subject = subject;
    }

    public EmailNotificationId Id { get; }

    public string ToEmail { get; private set; }

    public string Subject { get; private set; }

    public string? BodyText { get; private set; }

    public string? BodyHtml { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? SentAt { get; private set; }

    public void SetSentAt(DateTimeOffset date)
    {
        SentAt = date;
    }

    public static EmailNotification Create(string toEmail,
        string subject,
        string? bodyText = null,
        string? bodyHtml = null)
    {
        if (bodyText is null && bodyHtml is null)
        {
            throw new ValidationException("BodyText and BodyHtml cannot be both null.");
        }

        return new EmailNotification(EmailNotificationId.New(), toEmail, subject)
        {
            BodyText = bodyText,
            BodyHtml = bodyHtml
        };
    }

    public override string ToString()
    {
        return $"bodyHtml: {BodyHtml}, bodyText: {BodyText}";
    }
}
