using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Visitor_AddUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "visitor",
                type: "uuid",
                maxLength: 255,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_visitor_user_id",
                table: "visitor",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_visitor_user_user_id",
                table: "visitor",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_visitor_user_user_id",
                table: "visitor");

            migrationBuilder.DropIndex(
                name: "ix_visitor_user_id",
                table: "visitor");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "visitor");
        }
    }
}
