using TickerQ.Jobs.Application.Notifications;
using TickerQ.Jobs.Contracts.AutoPushes;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Models;


namespace TickerQ.Jobs.Web.AutoPushes;

public sealed class HostSideAutopushController
{
    private readonly IEmailNotificationService emailNotificationService;

    public HostSideAutopushController(IEmailNotificationService emailNotificationService)
    {
        this.emailNotificationService = emailNotificationService;
    }

    [TickerFunction(nameof(NotifyHostSidePartner))]
    public async Task NotifyHostSidePartner(
        TickerFunctionContext<HostSidePartnerNotificationRequest> context,
        CancellationToken cancellationToken)
    {
        await emailNotificationService.SendEmailAsync(
            context.Request.Emails,
            context.Request.BccEmails,
            $"Subject for {context.Request.PartnerId}",
            $"htmlbody",
            cancellationToken);
    }
}
