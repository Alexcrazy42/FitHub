using Hangfire.Dashboard;

namespace Hangfire;

public class AllowAnonymousDashboardFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
