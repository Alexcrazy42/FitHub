namespace FitHub.Application.Common;

public class PagedQuery
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public PagedQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
