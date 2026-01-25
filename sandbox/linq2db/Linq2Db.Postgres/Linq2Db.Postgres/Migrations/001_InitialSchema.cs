using FluentMigrator;

namespace Linq2Db.Postgres.Migrations;

[Migration(1)]
public class InitialSchema : Migration
{
    public override void Up()
    {
        // Users
        Create.Table("users")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("username").AsString(100).NotNullable().Unique()
            .WithColumn("email").AsString(255).NotNullable().Unique()
            .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime);

        // Products
        Create.Table("products")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(1000).Nullable()
            .WithColumn("price").AsDecimal(10, 2).NotNullable()
            .WithColumn("stock").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime);

        // Orders
        Create.Table("orders")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("order_number").AsString(50).NotNullable().Unique()
            .WithColumn("total_amount").AsDecimal(10, 2).NotNullable()
            .WithColumn("status").AsString(20).NotNullable().WithDefaultValue("pending")
            .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime);

        Create.ForeignKey("fk_orders_users")
            .FromTable("orders").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("idx_orders_user_id").OnTable("orders").OnColumn("user_id");
        Create.Index("idx_orders_status").OnTable("orders").OnColumn("status");

        // Order Items
        Create.Table("order_items")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("order_id").AsInt32().NotNullable()
            .WithColumn("product_id").AsInt32().NotNullable()
            .WithColumn("quantity").AsInt32().NotNullable()
            .WithColumn("price").AsDecimal(10, 2).NotNullable();

        Create.ForeignKey("fk_order_items_orders")
            .FromTable("order_items").ForeignColumn("order_id")
            .ToTable("orders").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_order_items_products")
            .FromTable("order_items").ForeignColumn("product_id")
            .ToTable("products").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("idx_order_items_order_id").OnTable("order_items").OnColumn("order_id");
        Create.Index("idx_order_items_product_id").OnTable("order_items").OnColumn("product_id");
    }

    public override void Down()
    {
        Delete.Table("order_items");
        Delete.Table("orders");
        Delete.Table("products");
        Delete.Table("users");
    }
}