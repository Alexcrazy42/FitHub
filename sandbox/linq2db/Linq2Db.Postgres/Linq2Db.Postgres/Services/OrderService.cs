using Linq2Db.Postgres.DataAccess;
using Linq2Db.Postgres.Models;
using LinqToDB;
using LinqToDB.Async;

namespace Linq2Db.Postgres.Services;

public class OrderService
{
    private readonly AppDataConnection _db;

    public OrderService(AppDataConnection db)
    {
        _db = db;
    }

    // === ПРИМЕР 1: INNER JOIN ===
    // Получить заказы с информацией о пользователях
    public async Task<List<object>> GetOrdersWithUsers()
    {
        var result = await (
            from order in _db.Orders
            join user in _db.Users on order.UserId equals user.Id
            select new
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Username = user.Username,
                UserEmail = user.Email,
                CreatedAt = order.CreatedAt
            }
        ).ToListAsync();

        return result.Cast<object>().ToList();
    }

    // === ПРИМЕР 2: LEFT JOIN ===
    // Все пользователи с количеством их заказов (включая без заказов)
    public async Task<List<object>> GetUsersWithOrderCount()
    {
        var result = await (
            from user in _db.Users
            from order in _db.Orders
                .Where(o => o.UserId == user.Id)
                .DefaultIfEmpty()
            group order by new { user.Id, user.Username, user.Email } into g
            select new
            {
                UserId = g.Key.Id,
                Username = g.Key.Username,
                Email = g.Key.Email,
                OrderCount = g.Count(o => o != null),
                TotalSpent = g.Sum(o => o != null ? o.TotalAmount : 0)
            }
        ).ToListAsync();

        return result.Cast<object>().ToList();
    }

    // === ПРИМЕР 3: МНОЖЕСТВЕННЫЙ JOIN ===
    // Детали заказов с информацией о продуктах и пользователях
    public async Task<List<object>> GetOrderDetails(int orderId)
    {
        var result = await (
            from order in _db.Orders
            join user in _db.Users on order.UserId equals user.Id
            join item in _db.OrderItems on order.Id equals item.OrderId
            join product in _db.Products on item.ProductId equals product.Id
            where order.Id == orderId
            select new
            {
                OrderNumber = order.OrderNumber,
                Username = user.Username,
                ProductName = product.Name,
                Quantity = item.Quantity,
                Price = item.Price,
                LineTotal = item.Quantity * item.Price
            }
        ).ToListAsync();

        return result.Cast<object>().ToList();
    }

    // === ПРИМЕР 4: ПОДЗАПРОС (IN) ===
    // Продукты, которые были заказаны хотя бы раз
    public async Task<List<Product>> GetOrderedProducts()
    {

        var result = await _db.Products
            .Where(p => _db.OrderItems.Select(oi => oi.ProductId).Distinct().Contains(p.Id))
            .ToListAsync();

        return result;
    }

    // === ПРИМЕР 5: ПОДЗАПРОС (EXISTS) ===
    // Пользователи, у которых есть заказы со статусом "pending"
    public async Task<List<User>> GetUsersWithPendingOrders()
    {
        var result = await _db.Users
            .Where(u => _db.Orders.Any(o => o.UserId == u.Id && o.Status == "pending"))
            .ToListAsync();

        return result;
    }

    // === ПРИМЕР 6: СКАЛЯРНЫЙ ПОДЗАПРОС ===
    // Заказы с количеством позиций в каждом
    public async Task<List<object>> GetOrdersWithItemCount()
    {
        var result = await (
            from order in _db.Orders
            select new
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                ItemCount = _db.OrderItems.Count(oi => oi.OrderId == order.Id)
            }
        ).ToListAsync();

        return result.Cast<object>().ToList();
    }

    // === ПРИМЕР 7: CTE (Common Table Expression) ===
    // Пользователи с суммарными тратами через CTE
    public async Task<List<object>> GetUserSpendingWithCTE()
    {
        // Определяем CTE
        var userTotals = 
            from order in _db.Orders
            group order by order.UserId into g
            select new
            {
                UserId = g.Key,
                TotalSpent = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            };

        // Используем CTE в основном запросе
        var result = await (
            from cte in userTotals.AsCte("UserTotals")
            join user in _db.Users on cte.UserId equals user.Id
            where cte.TotalSpent > 50
            orderby cte.TotalSpent descending
            select new
            {
                Username = user.Username,
                Email = user.Email,
                TotalSpent = cte.TotalSpent,
                OrderCount = cte.OrderCount
            }
        ).ToListAsync();

        return result.Cast<object>().ToList();
    }

    // === ПРИМЕР 8: РЕКУРСИВНЫЙ CTE ===
    // (для примера - категории с подкатегориями, но у нас таких нет, 
    // покажу концепт с числовой последовательностью)
    public async Task<List<int>> GetNumberSequence(int max)
    {
        // Рекурсивный CTE в Linq2DB работает через специальный синтаксис
        var anchor = _db.SelectQuery(() => new NumberRow { N = 1 });

        var cte = anchor.AsCte("NumbersCTE");

        var recursive = cte
            .Concat(
                from prev in cte
                where prev.N < max
                select new NumberRow { N = prev.N + 1 }
            )
            .AsCte("NumbersCTE");

        return await recursive.Select(n => n.N).ToListAsync();
    }

    // === ПРИМЕР 9: WINDOW FUNCTION (ранжирование) ===
    // Топ-3 продукта по количеству заказов
    public async Task<List<object>> GetTopProductsByOrders()
    {
        var result = await (
            from item in _db.OrderItems
            group item by item.ProductId into g
            join product in _db.Products on g.Key equals product.Id
            select new
            {
                ProductName = product.Name,
                TimesOrdered = g.Count(),
                TotalQuantity = g.Sum(i => i.Quantity),
                TotalRevenue = g.Sum(i => i.Quantity * i.Price)
            }
        )
        .OrderByDescending(x => x.TimesOrdered)
        .Take(3)
        .ToListAsync();

        return result.Cast<object>().ToList();
    }

    // === ПРИМЕР 10: СЛОЖНЫЙ CTE с агрегацией ===
    // Статистика по заказам: сколько заказов в каждом статусе, средний чек
    public async Task<List<object>> GetOrderStatisticsByCTE()
    {
        var orderStats = 
            from order in _db.Orders
            group order by order.Status into g
            select new
            {
                Status = g.Key,
                Count = g.Count(),
                AvgAmount = g.Average(o => o.TotalAmount),
                TotalAmount = g.Sum(o => o.TotalAmount)
            };

        var result = await orderStats
            .AsCte("OrderStatsCTE")
            .OrderByDescending(s => s.TotalAmount)
            .ToListAsync();

        return result.Cast<object>().ToList();
    }
}

// Вспомогательный класс для рекурсивного CTE
public class NumberRow
{
    public int N { get; set; }
}