using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBrands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "brand_id",
                table: "equipment",
                type: "uuid",
                maxLength: 255,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_equipment_brand_id",
                table: "equipment",
                column: "brand_id");

            migrationBuilder.AddForeignKey(
                name: "fk_equipment_brand_brand_id",
                table: "equipment",
                column: "brand_id",
                principalTable: "brand",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_equipment_brand_brand_id",
                table: "equipment");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropIndex(
                name: "ix_equipment_brand_id",
                table: "equipment");

            migrationBuilder.DropColumn(
                name: "brand_id",
                table: "equipment");
        }
    }
}
