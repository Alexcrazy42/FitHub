namespace FitHub.Web.V1.Test.Models;

public class Order
{
    public string Id { get; set; } = String.Empty;
    public string OrderNumber { get; set; } = String.Empty;
    public string CustomerName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Phone { get; set; } = String.Empty;

    public OrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public Currency Currency { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string ShippingAddress { get; set; } = String.Empty;

    public PaymentMethod PaymentMethod { get; set; }

    public int ItemsCount { get; set; }
    public string? Notes { get; set; }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

public enum Currency
{
    Usd,
    Eur,
    Rub
}

public enum PaymentMethod
{
    Card,
    Paypal,
    Cash,
    BankTransfer
}

public enum Ordering
{
    Ascending,
    Descending
}
