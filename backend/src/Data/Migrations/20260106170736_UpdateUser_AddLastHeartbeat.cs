using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser_AddLastHeartbeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_heartbeat",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: new Guid("a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc"),
                column: "last_heartbeat",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_heartbeat",
                table: "user");
        }
    }
}
