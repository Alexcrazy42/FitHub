using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_VisitorGymRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "visitor_gym_relation",
                columns: table => new
                {
                    gym_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    visitor_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    is_default_gym = table.Column<bool>(type: "boolean", nullable: false),
                    visit_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_visitor_gym_relation", x => new { x.visitor_id, x.gym_id });
                    table.ForeignKey(
                        name: "fk_visitor_gym_relation_gym_gym_id",
                        column: x => x.gym_id,
                        principalTable: "gym",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_visitor_gym_relation_visitor_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_visitor_gym_relation_gym_id",
                table: "visitor_gym_relation",
                column: "gym_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "visitor_gym_relation");
        }
    }
}
