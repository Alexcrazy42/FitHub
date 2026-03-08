namespace FitHub.Application.Security;

public record LinkSecurityCheckResult(
    string Domain,
    bool HasSsl,
    int MaliciousCount,
    int SuspiciousCount,
    int TotalEngines,
    SecurityStatus Status);

public enum SecurityStatus
{
    Safe,
    Suspicious,
    Malicious,
    Unknown
}
