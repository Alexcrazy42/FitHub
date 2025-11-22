using FitHub.Web.V1.Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FitHub.Web.V1.Test;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly FakeOrderService fakeOrderService;
    private readonly ILogger<TestController> logger;

    public TestController(ILogger<TestController> logger)
    {
        fakeOrderService = new FakeOrderService();
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<OrdersResponse>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? q = null,
        [FromQuery] List<OrderStatus>? status = null,
        [FromQuery] List<PaymentMethod>? paymentMethod = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] Ordering? sortOrder = Ordering.Ascending)
    {
        try
        {
            await Task.Delay(400);

            logger.LogInformation("API request: {@Query}", new
            {
                Page = page,
                PageSize = pageSize,
                SearchQuery = q,
                Status = status,
                PaymentMethod = paymentMethod,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                SortBy = sortBy,
                SortOrder = sortOrder
            });

            var query = new OrdersQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchQuery = q,
                Filters = new OrderFilters
                {
                    Status = status,
                    PaymentMethod = paymentMethod,
                    MinAmount = minAmount,
                    MaxAmount = maxAmount
                },
                SortConfig = new SortConfig
                {
                    Field = sortBy ?? "createdAt",
                    Direction = sortOrder ?? Ordering.Ascending
                }
            };

            var response = fakeOrderService.GetOrders(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while fetching orders");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(string id)
    {
        try
        {
            await Task.Delay(200);

            var allOrders = fakeOrderService.GenerateOrders(150);
            var order = allOrders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while fetching order {OrderId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("search")]
    public async Task<ActionResult<OrdersResponse>> SearchOrders([FromBody] OrdersQuery query)
    {
        try
        {
            // Имитация задержки
            await Task.Delay(400);

            logger.LogInformation("Search orders: {@Query}", query);

            var response = fakeOrderService.GetOrders(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while searching orders");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("generate")]
    public ActionResult GenerateOrders([FromQuery] int count = 150)
    {
        try
        {
            var orders = fakeOrderService.GenerateOrders(count);
            return Ok(new { message = $"Generated {count} orders", count = orders.Count });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while generating orders");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("test")]
    public IActionResult Test([FromQuery] Test1 test)
    {
        return Ok(test);
    }

    public class Test1
    {
        public DateTimeOffset? Start { get; set; }

        public DateTimeOffset? End { get; set; }

        public TestEnum? Status { get; set; }
    }

    public enum TestEnum
    {
        Stop,
        Start,
        End
    }
}
