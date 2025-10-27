using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseGroupTraining_AddIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "base_group_trainings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "base_group_trainings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "base_group_trainings");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "base_group_trainings");
        }
    }
}
