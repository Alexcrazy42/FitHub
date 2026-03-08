namespace FitHub.Contracts.V1.Security;

public class CheckUrlResponse
{
    public string Domain { get; set; } = String.Empty;
    public bool HasSsl { get; set; }
    public int MaliciousCount { get; set; }
    public int SuspiciousCount { get; set; }
    public int TotalEngines { get; set; }
    public string Status { get; set; } = String.Empty;
}
