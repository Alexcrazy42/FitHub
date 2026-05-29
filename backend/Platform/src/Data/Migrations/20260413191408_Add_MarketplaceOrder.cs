using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_MarketplaceOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "marketplace_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    reservation_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marketplace_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_marketplace_orders_marketplace_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "marketplace_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_marketplace_orders_stock_reservations_reservation_id",
                        column: x => x.reservation_id,
                        principalTable: "stock_reservations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "marketplace_order_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    product_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    brand_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    sku = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    variant_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    unit_price_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    image_file_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    attribute_summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marketplace_order_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_marketplace_order_items_marketplace_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "marketplace_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "marketplace_order_status_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marketplace_order_status_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_marketplace_order_status_history_marketplace_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "marketplace_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_marketplace_order_items_order_id",
                table: "marketplace_order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_marketplace_order_status_history_order_id",
                table: "marketplace_order_status_history",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_marketplace_orders_payment_id",
                table: "marketplace_orders",
                column: "payment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_marketplace_orders_reservation_id",
                table: "marketplace_orders",
                column: "reservation_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "marketplace_order_items");

            migrationBuilder.DropTable(
                name: "marketplace_order_status_history");

            migrationBuilder.DropTable(
                name: "marketplace_orders");
        }
    }
}
