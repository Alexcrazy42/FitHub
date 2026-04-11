using System.Text.Json;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Set<Log>().AnyAsync())
            return;

        var logs = new List<Log>
        {
            MakeLog("log", 25, new[] { 10, 1 }),
            MakeLog("Joe", 25, new[] { 5, 8 }),
            MakeLog("Alice", 30, new[] { 100, 200 }),
            MakeLog("Bob", 40, new[] { 7, 9 }, city: "Berlin"),
            MakeLog("Carol", 22, new[] { 1, 2 }, city: "Paris"),
            MakeLog("Dave", 28, new[] { 50, 60 }),
            MakeLog("Eve", 35, new[] { 3, 4 }, city: "Frankfurt"),
            MakeLog("Frank", 18, new[] { 11, 12 }, city: "Hamburg"),
            MakeLog("Grace", 29, new[] { 13, 14 }),
            MakeLog("Heidi", 31, new[] { 15, 16 }, city: "Munich")
        };

        db.Set<Log>().AddRange(logs);
        await db.SaveChangesAsync();
    }

    private static Log MakeLog(string name, int age, int[] orderPrices, string? city = null)
    {
        var orders = orderPrices
            .Select(x => new { Price = x })
            .ToArray();

        object payload = city is null
            ? new
            {
                Name = name,
                Age = age,
                Orders = orders
            }
            : new
            {
                Name = name,
                Age = age,
                City = city,
                Orders = orders
            };

        return new Log
        {
            Id = Guid.NewGuid(),
            Payload = CreatePayload(payload)
        };
    }

    private static JsonDocument CreatePayload<T>(T value)
    {
        var json = JsonSerializer.Serialize(value);
        return JsonDocument.Parse(json);
    }
}