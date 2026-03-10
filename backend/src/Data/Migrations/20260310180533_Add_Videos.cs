using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Videos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "video",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    original_file_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    poster_s3key = table.Column<string>(type: "text", nullable: true),
                    failure_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_video", x => x.id);
                    table.ForeignKey(
                        name: "fk_video_file_entity_original_file_id",
                        column: x => x.original_file_id,
                        principalTable: "file_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "video_resolution",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    video_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    quality = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    s3key = table.Column<string>(type: "text", nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    width_px = table.Column<int>(type: "integer", nullable: false),
                    height_px = table.Column<int>(type: "integer", nullable: false),
                    bitrate_kbps = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_video_resolution", x => x.id);
                    table.ForeignKey(
                        name: "fk_video_resolution_video_video_id",
                        column: x => x.video_id,
                        principalTable: "video",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_video_original_file_id",
                table: "video",
                column: "original_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_video_resolution_video_id",
                table: "video_resolution",
                column: "video_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "video_resolution");

            migrationBuilder.DropTable(
                name: "video");
        }
    }
}
