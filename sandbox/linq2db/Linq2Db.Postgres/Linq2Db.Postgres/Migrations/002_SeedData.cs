using FluentMigrator;

namespace Linq2Db.Postgres.Migrations;

[Migration(2)]
public class SeedData : Migration
{
    public override void Up()
    {
        // Users
        Insert.IntoTable("users").Row(new 
        { 
            username = "john_doe", 
            email = "john@example.com",
            created_at = DateTime.UtcNow
        });
        Insert.IntoTable("users").Row(new 
        { 
            username = "jane_smith", 
            email = "jane@example.com",
            created_at = DateTime.UtcNow
        });

        // Products
        Insert.IntoTable("products").Row(new
        {
            name = "Laptop",
            description = "High-performance laptop",
            price = 1299.99m,
            stock = 10,
            created_at = DateTime.UtcNow
        });
        Insert.IntoTable("products").Row(new
        {
            name = "Mouse",
            description = "Wireless mouse",
            price = 29.99m,
            stock = 50,
            created_at = DateTime.UtcNow
        });
        Insert.IntoTable("products").Row(new
        {
            name = "Keyboard",
            description = "Mechanical keyboard",
            price = 89.99m,
            stock = 30,
            created_at = DateTime.UtcNow
        });

        // Orders
        Insert.IntoTable("orders").Row(new
        {
            user_id = 1,
            order_number = "ORD-001",
            total_amount = 1419.97m,
            status = "completed",
            created_at = DateTime.UtcNow.AddDays(-5)
        });
        Insert.IntoTable("orders").Row(new
        {
            user_id = 1,
            order_number = "ORD-002",
            total_amount = 29.99m,
            status = "pending",
            created_at = DateTime.UtcNow.AddDays(-1)
        });
        Insert.IntoTable("orders").Row(new
        {
            user_id = 2,
            order_number = "ORD-003",
            total_amount = 89.99m,
            status = "completed",
            created_at = DateTime.UtcNow.AddDays(-3)
        });

        // Order Items
        Insert.IntoTable("order_items").Row(new { order_id = 1, product_id = 1, quantity = 1, price = 1299.99m });
        Insert.IntoTable("order_items").Row(new { order_id = 1, product_id = 2, quantity = 4, price = 29.99m });
        Insert.IntoTable("order_items").Row(new { order_id = 2, product_id = 2, quantity = 1, price = 29.99m });
        Insert.IntoTable("order_items").Row(new { order_id = 3, product_id = 3, quantity = 1, price = 89.99m });
    }

    public override void Down()
    {
        Delete.FromTable("order_items").AllRows();
        Delete.FromTable("orders").AllRows();
        Delete.FromTable("products").AllRows();
        Delete.FromTable("users").AllRows();
    }
}