namespace FitHub.Contracts;

public sealed class PagedRequest
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
