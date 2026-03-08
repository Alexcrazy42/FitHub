namespace FitHub.Application.Security;

public interface ILinkSecurityService
{
    Task<LinkSecurityCheckResult> CheckUrlAsync(string url, CancellationToken ct);
}
