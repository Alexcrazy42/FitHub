using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Delete_NotNeededRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "equipment_gym");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "equipment_gym",
                columns: table => new
                {
                    equipments_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gyms_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_gym", x => new { x.equipments_id, x.gyms_id });
                    table.ForeignKey(
                        name: "fk_equipment_gym_equipment_equipments_id",
                        column: x => x.equipments_id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_equipment_gym_gym_gyms_id",
                        column: x => x.gyms_id,
                        principalTable: "gym",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_equipment_gym_gyms_id",
                table: "equipment_gym",
                column: "gyms_id");
        }
    }
}
