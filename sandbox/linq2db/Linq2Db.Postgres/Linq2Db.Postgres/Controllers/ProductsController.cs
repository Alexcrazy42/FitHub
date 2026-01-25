using Linq2Db.Postgres.Services;
using Microsoft.AspNetCore.Mvc;

namespace Linq2Db.Postgres.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly OrderService _orderService;

    public ProductsController(OrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Продукты, которые были заказаны (подзапрос IN)
    /// </summary>
    [HttpGet("ordered")]
    public async Task<IActionResult> GetOrderedProducts()
    {
        var result = await _orderService.GetOrderedProducts();
        return Ok(result);
    }

    /// <summary>
    /// Топ-3 самых популярных продукта
    /// </summary>
    [HttpGet("top")]
    public async Task<IActionResult> GetTopProducts()
    {
        var result = await _orderService.GetTopProductsByOrders();
        return Ok(result);
    }
}