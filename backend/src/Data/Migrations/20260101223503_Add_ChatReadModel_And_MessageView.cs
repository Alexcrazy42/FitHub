using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_ChatReadModel_And_MessageView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat_reading_model",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    chat_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    last_message_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    last_message_text = table.Column<string>(type: "text", nullable: false),
                    last_message_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    unread_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_reading_model", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_reading_model_chat_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_chat_reading_model_message_last_message_id",
                        column: x => x.last_message_id,
                        principalTable: "message",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_chat_reading_model_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "message_view",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    viewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_view", x => x.id);
                    table.ForeignKey(
                        name: "fk_message_view_message_message_id",
                        column: x => x.message_id,
                        principalTable: "message",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_view_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chat_reading_model_chat_id_user_id",
                table: "chat_reading_model",
                columns: new[] { "chat_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_chat_reading_model_last_message_id",
                table: "chat_reading_model",
                column: "last_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_reading_model_user_id",
                table: "chat_reading_model",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_view_message_id",
                table: "message_view",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_view_user_id",
                table: "message_view",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_reading_model");

            migrationBuilder.DropTable(
                name: "message_view");
        }
    }
}
