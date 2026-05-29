using TickerQ.Jobs.Contracts.AutoPushes;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Models;

namespace TickerQ.Jobs.Web;

public sealed class KeepAliveJobs
{
    public static string InstanceIdentifier = "Not set";

    [TickerFunction(nameof(KeepAlive))]
    public Task KeepAlive(
        TickerFunctionContext<HostSidePartnerNotificationRequest> context,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{DateTime.UtcNow}] KeepAlive: {InstanceIdentifier}");
        return Task.CompletedTask;
    }
}
