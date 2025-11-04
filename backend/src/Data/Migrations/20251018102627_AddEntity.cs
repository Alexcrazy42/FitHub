using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    max_file_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "entity",
                columns: new[] { "id", "entity_type", "max_file_count" },
                values: new object[] { new Guid("6206cfea-d518-4b09-9257-0f790473545e"), "Gym", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity");
        }
    }
}
