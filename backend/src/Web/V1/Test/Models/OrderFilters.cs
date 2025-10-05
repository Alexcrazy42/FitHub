namespace FitHub.Web.V1.Test.Models;

public class OrderFilters
{
    public List<OrderStatus>? Status { get; set; }
    public List<PaymentMethod>? PaymentMethod { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class SortConfig
{
    public string Field { get; set; } = "createdAt";
    public Ordering Direction { get; set; } = Ordering.Ascending;
}

public class OrdersQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchQuery { get; set; }
    public OrderFilters? Filters { get; set; }
    public SortConfig? SortConfig { get; set; }
}

public class OrdersResponse
{
    public List<Order> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
