namespace TickerQ.Jobs.Contracts.AutoPushes;

public sealed class HostSidePartnerNotificationRequest
{
    public string? PartnerId { get; set; }

    public List<string> Emails { get; set; } = [];

    public List<string> BccEmails { get; set; } = [];
}
