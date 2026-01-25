using Linq2Db.Postgres.Services;
using Microsoft.AspNetCore.Mvc;

namespace Linq2Db.Postgres.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly OrderService _orderService;

    public UsersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Все пользователи с количеством заказов (LEFT JOIN)
    /// </summary>
    [HttpGet("with-order-count")]
    public async Task<IActionResult> GetUsersWithOrderCount()
    {
        var result = await _orderService.GetUsersWithOrderCount();
        return Ok(result);
    }

    /// <summary>
    /// Пользователи с незавершёнными заказами (подзапрос EXISTS)
    /// </summary>
    [HttpGet("with-pending-orders")]
    public async Task<IActionResult> GetUsersWithPendingOrders()
    {
        var result = await _orderService.GetUsersWithPendingOrders();
        return Ok(result);
    }

    /// <summary>
    /// Пользователи с суммарными тратами через CTE
    /// </summary>
    [HttpGet("spending-stats")]
    public async Task<IActionResult> GetUserSpendingStats()
    {
        var result = await _orderService.GetUserSpendingWithCTE();
        return Ok(result);
    }
}