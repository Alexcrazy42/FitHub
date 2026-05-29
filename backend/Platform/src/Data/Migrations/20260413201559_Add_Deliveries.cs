using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Deliveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "couriers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_couriers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "deliveries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    courier_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    pickup_address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    dropoff_address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_state_changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_courier_signal_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_location_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    courier_assignment_expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    watchdog_checked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_automatic_decision_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deliveries", x => x.id);
                    table.ForeignKey(
                        name: "fk_deliveries_couriers_courier_id",
                        column: x => x.courier_id,
                        principalTable: "couriers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_deliveries_marketplace_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "marketplace_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "delivery_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    delivery_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_delivery_events_deliveries_delivery_id",
                        column: x => x.delivery_id,
                        principalTable: "deliveries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "delivery_tracking_points",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    delivery_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_tracking_points", x => x.id);
                    table.ForeignKey(
                        name: "fk_delivery_tracking_points_deliveries_delivery_id",
                        column: x => x.delivery_id,
                        principalTable: "deliveries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_couriers_is_available",
                table: "couriers",
                column: "is_available");

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_courier_assignment_expires_at",
                table: "deliveries",
                column: "courier_assignment_expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_courier_id",
                table: "deliveries",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_order_id",
                table: "deliveries",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_status",
                table: "deliveries",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_events_delivery_id_created_at",
                table: "delivery_events",
                columns: new[] { "delivery_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_delivery_tracking_points_delivery_id_created_at",
                table: "delivery_tracking_points",
                columns: new[] { "delivery_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_events");

            migrationBuilder.DropTable(
                name: "delivery_tracking_points");

            migrationBuilder.DropTable(
                name: "deliveries");

            migrationBuilder.DropTable(
                name: "couriers");
        }
    }
}
