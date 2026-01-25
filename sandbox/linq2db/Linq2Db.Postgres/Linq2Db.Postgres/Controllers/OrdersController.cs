using Linq2Db.Postgres.Services;
using Microsoft.AspNetCore.Mvc;

namespace Linq2Db.Postgres.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Получить заказы с информацией о пользователях (INNER JOIN)
    /// </summary>
    [HttpGet("with-users")]
    public async Task<IActionResult> GetOrdersWithUsers()
    {
        var result = await _orderService.GetOrdersWithUsers();
        return Ok(result);
    }

    /// <summary>
    /// Детали конкретного заказа (множественный JOIN)
    /// </summary>
    [HttpGet("{orderId}/details")]
    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        var result = await _orderService.GetOrderDetails(orderId);
        if (result.Count == 0)
            return NotFound($"Order {orderId} not found");
        
        return Ok(result);
    }

    /// <summary>
    /// Заказы с количеством позиций (подзапрос)
    /// </summary>
    [HttpGet("with-item-count")]
    public async Task<IActionResult> GetOrdersWithItemCount()
    {
        var result = await _orderService.GetOrdersWithItemCount();
        return Ok(result);
    }

    /// <summary>
    /// Статистика по заказам через CTE
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetOrderStatistics()
    {
        var result = await _orderService.GetOrderStatisticsByCTE();
        return Ok(result);
    }
}