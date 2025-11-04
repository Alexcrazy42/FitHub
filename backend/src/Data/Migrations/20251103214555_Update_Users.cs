using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_verified",
                table: "user",
                newName: "is_temporary_password");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_email_confirmed",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "start_active_at",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "start_registration_at",
                table: "user",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "email_notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    to_email = table.Column<string>(type: "text", nullable: false),
                    subject = table.Column<string>(type: "text", nullable: false),
                    body_text = table.Column<string>(type: "text", nullable: true),
                    body_html = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_notification", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    token_string = table.Column<string>(type: "text", nullable: false),
                    token_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_token_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "created_at", "email", "is_active", "is_email_confirmed", "is_online", "is_temporary_password", "last_seen_at", "name", "password_hash", "start_active_at", "start_registration_at", "surname", "updated_at", "user_type" },
                values: new object[] { new Guid("a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc"), new DateTimeOffset(new DateTime(2025, 11, 2, 21, 20, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "alexcrazy42@mail.ru", true, true, false, false, new DateTimeOffset(new DateTime(2025, 11, 2, 21, 20, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Александр", "$2a$11$H9cNs1CfV.iJiv/N9hIHOe4UC/23MCB8xObp4m.wKbh7YOzmsQrjO", null, new DateTimeOffset(new DateTime(2025, 11, 2, 21, 20, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Мамедов", new DateTimeOffset(new DateTime(2025, 11, 2, 21, 20, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "CmsAdmin" });

            migrationBuilder.CreateIndex(
                name: "ix_token_user_id",
                table: "token",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_notification");

            migrationBuilder.DropTable(
                name: "token");

            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "id",
                keyValue: new Guid("a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc"));

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "user");

            migrationBuilder.DropColumn(
                name: "is_email_confirmed",
                table: "user");

            migrationBuilder.DropColumn(
                name: "start_active_at",
                table: "user");

            migrationBuilder.DropColumn(
                name: "start_registration_at",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "is_temporary_password",
                table: "user",
                newName: "is_verified");
        }
    }
}
