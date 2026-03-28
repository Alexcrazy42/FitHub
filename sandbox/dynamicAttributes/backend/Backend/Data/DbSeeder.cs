using System.Text.Json.Nodes;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken ct = default)
    {
        if (await db.Categories.AnyAsync(ct))
        {
            return; // Уже есть данные
        }

        // Создаём категории
        var rootCategories = new List<Category>
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Обувь" },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Одежда" },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Электроника" },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Спорт" },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Дом и сад" },
        };

        var subCategories = new List<Category>
        {
            // Обувь
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Name = "Кроссовки и кеды", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000001") },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Name = "Ботинки и сапоги", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000001") },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Name = "Кеды", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000011") },
            
            // Одежда
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000021"), Name = "Куртки", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000002") },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000022"), Name = "Футболки", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000002") },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000023"), Name = "Зимние куртки", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000021") },
            
            // Электроника
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000031"), Name = "Телевизоры", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000003") },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000032"), Name = "Смартфоны", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000003") },
            
            // Спорт
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000041"), Name = "Фитнес", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000004") },
            
            // Дом и сад
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000051"), Name = "Мебель", ParentId = Guid.Parse("00000000-0000-0000-0000-000000000005") },
        };

        var allCategories = rootCategories.Concat(subCategories).ToList();
        await db.Categories.AddRangeAsync(allCategories, ct);
        await db.SaveChangesAsync(ct);

        // Строим Closure Table
        var categoryRepo = new CategoryRepository(db);
        foreach (var category in allCategories)
        {
            await categoryRepo.RebuildHierarchyForCategoryAsync(category.Id, category.ParentId, ct);
        }

        // Создаём товары
        var products = new List<Product>
        {
            // Кроссовки
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Nike Air Max",
                Price = 12999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Nike"", ""size"": 42, ""color"": ""black"", ""soleLength"": 28.5, ""fastener"": ""laces""}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000011") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Adidas Ultraboost",
                Price = 15499,
                Attributes = JsonNode.Parse(@"{""brand"": ""Adidas"", ""size"": 43, ""color"": ""white"", ""soleLength"": 29, ""fastener"": ""laces""}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000011") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "Puma RS-X",
                Price = 9999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Puma"", ""size"": 41, ""color"": ""red"", ""soleLength"": 27.5, ""fastener"": ""laces""}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000011") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name = "New Balance 574",
                Price = 11499,
                Attributes = JsonNode.Parse(@"{""brand"": ""New Balance"", ""size"": 42, ""color"": ""grey"", ""soleLength"": 28, ""fastener"": ""laces""}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000011") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Name = "Reebok Classic",
                Price = 8999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Reebok"", ""size"": 40, ""color"": ""white"", ""soleLength"": 26.5, ""fastener"": ""laces""}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000011") } }
            },
            
            // Ботинки
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000011"),
                Name = "Timberland Classic",
                Price = 18999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Timberland"", ""size"": 43, ""color"": ""brown"", ""material"": ""leather"", ""waterproof"": true}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000012") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000012"),
                Name = "Dr. Martens 1460",
                Price = 21999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Dr. Martens"", ""size"": 42, ""color"": ""black"", ""material"": ""leather"", ""waterproof"": false}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000012") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000013"),
                Name = "Caterpillar Colorado",
                Price = 16499,
                Attributes = JsonNode.Parse(@"{""brand"": ""Caterpillar"", ""size"": 44, ""color"": ""brown"", ""material"": ""suede"", ""waterproof"": true}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000012") } }
            },
            
            // Куртки
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000021"),
                Name = "The North Face McMurdo",
                Price = 45999,
                Attributes = JsonNode.Parse(@"{""brand"": ""The North Face"", ""size"": ""L"", ""hood"": true, ""sleeveLength"": ""long"", ""season"": ""winter"", ""temperature"": -30}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000023") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000022"),
                Name = "Canada Goose Expedition",
                Price = 125999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Canada Goose"", ""size"": ""M"", ""hood"": true, ""sleeveLength"": ""long"", ""season"": ""winter"", ""temperature"": -50}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000023") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000023"),
                Name = "Patagonia Down Sweater",
                Price = 32999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Patagonia"", ""size"": ""L"", ""hood"": false, ""sleeveLength"": ""long"", ""season"": ""autumn"", ""temperature"": -10}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000021") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000024"),
                Name = "Columbia Powder Lite",
                Price = 18999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Columbia"", ""size"": ""XL"", ""hood"": true, ""sleeveLength"": ""long"", ""season"": ""winter"", ""temperature"": -20}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000021") } }
            },
            
            // Футболки
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000031"),
                Name = "Nike Sportswear",
                Price = 2499,
                Attributes = JsonNode.Parse(@"{""brand"": ""Nike"", ""size"": ""M"", ""color"": ""black"", ""material"": ""cotton"", ""sleeveLength"": ""short""}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000022") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000032"),
                Name = "Adidas Essentials",
                Price = 1999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Adidas"", ""size"": ""L"", ""color"": ""white"", ""material"": ""cotton"", ""sleeveLength"": ""short""}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000022") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000033"),
                Name = "Puma Active",
                Price = 2199,
                Attributes = JsonNode.Parse(@"{""brand"": ""Puma"", ""size"": ""S"", ""color"": ""blue"", ""material"": ""polyester"", ""sleeveLength"": ""short""}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000022") } }
            },
            
            // Телевизоры
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000041"),
                Name = "Samsung QE55Q80A",
                Price = 89999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Samsung"", ""screenSize"": 55, ""resolution"": ""4K"", ""smartTV"": true, ""hdr"": true}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000031") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000042"),
                Name = "LG OLED55C1",
                Price = 119999,
                Attributes = JsonNode.Parse(@"{""brand"": ""LG"", ""screenSize"": 55, ""resolution"": ""4K"", ""smartTV"": true, ""hdr"": true}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000031") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000043"),
                Name = "Sony KD-65XH9077",
                Price = 79999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Sony"", ""screenSize"": 65, ""resolution"": ""4K"", ""smartTV"": true, ""hdr"": true}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000031") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000044"),
                Name = "TCL 55P715",
                Price = 42999,
                Attributes = JsonNode.Parse(@"{""brand"": ""TCL"", ""screenSize"": 55, ""resolution"": ""4K"", ""smartTV"": true, ""hdr"": false}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000031") } }
            },
            
            // Смартфоны
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000051"),
                Name = "iPhone 15 Pro",
                Price = 119999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Apple"", ""screenSize"": 6.1, ""storage"": 256, ""color"": ""titanium"", ""5g"": true}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000032") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000052"),
                Name = "Samsung Galaxy S24",
                Price = 89999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Samsung"", ""screenSize"": 6.2, ""storage"": 128, ""color"": ""black"", ""5g"": true}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000032") } }
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000053"),
                Name = "Google Pixel 8",
                Price = 69999,
                Attributes = JsonNode.Parse(@"{""brand"": ""Google"", ""screenSize"": 6.2, ""storage"": 128, ""color"": ""white"", ""5g"": true}"),
                Categories = new() { new() { CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000032") } }
            },
        };

        await db.Products.AddRangeAsync(products, ct);
        await db.SaveChangesAsync(ct);
    }
}
