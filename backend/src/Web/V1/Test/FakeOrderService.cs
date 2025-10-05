using FitHub.Web.V1.Test.Models;

namespace FitHub.Web.V1.Test;

public class FakeOrderService
    {
        private readonly List<Order> orders;
        private readonly Random random = new();

        public FakeOrderService()
        {
            orders = GenerateOrders(150); // Генерируем 150 заказов как в твоем фронтенде
        }

        public List<Order> GenerateOrders(int count)
        {
            var orders = new List<Order>();
            var cities = new[] { "Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург", "Казань", "Нижний Новгород", "Челябинск", "Самара", "Омск", "Ростов-на-Дону" };

            for (int i = 0; i < count; i++)
            {
                var status = (OrderStatus)(i % 5);
                var currency = (Currency)(i % 3);
                var paymentMethod = (PaymentMethod)(i % 4);

                var order = new Order
                {
                    Id = $"order-{i + 1}",
                    OrderNumber = $"ORD-{(1000 + i).ToString().PadLeft(6, '0')}",
                    CustomerName = $"Покупатель {i + 1}",
                    Email = $"customer{i + 1}@example.com",
                    Phone = $"+7 (999) {(100 + i).ToString().PadLeft(4, '0')}",
                    Status = status,
                    TotalAmount = random.Next(50, 1050),
                    Currency = currency,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(0, 30)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(0, 7)),
                    ShippingAddress = $"Улица {i + 1}, Город {cities[i % 10]}",
                    PaymentMethod = paymentMethod,
                    ItemsCount = random.Next(1, 11),
                    Notes = i % 7 == 0 ? $"Особое примечание к заказу {i + 1}" : null
                };

                orders.Add(order);
            }

            return orders;
        }

        public OrdersResponse GetOrders(OrdersQuery query)
        {
            var filtered = orders.AsEnumerable();

            // Поиск по всем текстовым полям
            if (!string.IsNullOrEmpty(query.SearchQuery))
            {
                var searchQuery = query.SearchQuery.ToLower();
                filtered = filtered.Where(o =>
                    o.OrderNumber.ToLower().Contains(searchQuery) ||
                    o.CustomerName.ToLower().Contains(searchQuery) ||
                    o.Email.ToLower().Contains(searchQuery) ||
                    o.Phone.ToLower().Contains(searchQuery) ||
                    o.ShippingAddress.ToLower().Contains(searchQuery) ||
                    (o.Notes != null && o.Notes.ToLower().Contains(searchQuery)));
            }

            // Фильтрация по статусу
            if (query.Filters?.Status != null && query.Filters.Status.Any())
            {
                filtered = filtered.Where(o => query.Filters.Status.Contains(o.Status));
            }

            // Фильтрация по способу оплаты
            if (query.Filters?.PaymentMethod != null && query.Filters.PaymentMethod.Any())
            {
                filtered = filtered.Where(o => query.Filters.PaymentMethod.Contains(o.PaymentMethod));
            }

            // Фильтрация по сумме
            if (query.Filters?.MinAmount.HasValue == true)
            {
                filtered = filtered.Where(o => o.TotalAmount >= query.Filters.MinAmount.Value);
            }

            if (query.Filters?.MaxAmount.HasValue == true)
            {
                filtered = filtered.Where(o => o.TotalAmount <= query.Filters.MaxAmount.Value);
            }

            // Сортировка
            filtered = ApplySorting(filtered, query.SortConfig?.Field, query.SortConfig?.Direction);

            var total = filtered.Count();

            // Пагинация
            var paginated = filtered
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new OrdersResponse
            {
                Data = paginated,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        private IEnumerable<Order> ApplySorting(IEnumerable<Order> orders, string? sortBy, Ordering? sortOrder)
        {
            var direction = sortOrder == Ordering.Ascending ? "asc" : "desc";

            return (sortBy?.ToLower(), direction) switch
            {
                ("createdat", "asc") => orders.OrderBy(o => o.CreatedAt),
                ("createdat", "desc") => orders.OrderByDescending(o => o.CreatedAt),
                ("updatedat", "asc") => orders.OrderBy(o => o.UpdatedAt),
                ("updatedat", "desc") => orders.OrderByDescending(o => o.UpdatedAt),
                ("totalamount", "asc") => orders.OrderBy(o => o.TotalAmount),
                ("totalamount", "desc") => orders.OrderByDescending(o => o.TotalAmount),
                ("ordernumber", "asc") => orders.OrderBy(o => o.OrderNumber),
                ("ordernumber", "desc") => orders.OrderByDescending(o => o.OrderNumber),
                ("customername", "asc") => orders.OrderBy(o => o.CustomerName),
                ("customername", "desc") => orders.OrderByDescending(o => o.CustomerName),
                ("status", "asc") => orders.OrderBy(o => o.Status.ToString()),
                ("status", "desc") => orders.OrderByDescending(o => o.Status.ToString()),
                ("paymentmethod", "asc") => orders.OrderBy(o => o.PaymentMethod.ToString()),
                ("paymentmethod", "desc") => orders.OrderByDescending(o => o.PaymentMethod.ToString()),
                _ => orders.OrderByDescending(o => o.CreatedAt) // default
            };
        }
    }
