using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Actualize_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nickname",
                table: "user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nickname",
                table: "user",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
