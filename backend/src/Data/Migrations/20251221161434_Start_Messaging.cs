using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Start_Messaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    updated_by_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chat_participant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    chat_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    blocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_participant", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_participant_chat_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_chat_participant_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    chat_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    message_text = table.Column<string>(type: "text", nullable: false),
                    reply_message_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true),
                    forwarded_message_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_by_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message", x => x.id);
                    table.ForeignKey(
                        name: "fk_message_chat_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_message_forwarded_message_id",
                        column: x => x.forwarded_message_id,
                        principalTable: "message",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_message_reply_message_id",
                        column: x => x.reply_message_id,
                        principalTable: "message",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_user_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_user_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_user_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "message_attachment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    entity_id = table.Column<string>(type: "text", nullable: true),
                    data = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    updated_by_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_attachment", x => x.id);
                    table.ForeignKey(
                        name: "fk_message_attachment_message_message_id",
                        column: x => x.message_id,
                        principalTable: "message",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_attachment_user_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_attachment_user_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chat_participant_chat_id",
                table: "chat_participant",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_participant_user_id",
                table: "chat_participant",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_chat_id",
                table: "message",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_created_by_id",
                table: "message",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_deleted_by_id",
                table: "message",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_forwarded_message_id",
                table: "message",
                column: "forwarded_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_reply_message_id",
                table: "message",
                column: "reply_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_updated_by_id",
                table: "message",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_attachment_created_by_id",
                table: "message_attachment",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_attachment_message_id",
                table: "message_attachment",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_attachment_updated_by_id",
                table: "message_attachment",
                column: "updated_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_participant");

            migrationBuilder.DropTable(
                name: "message_attachment");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "chat");
        }
    }
}
