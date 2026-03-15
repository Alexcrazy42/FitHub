using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Remove_UnusedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_muscle_group_video_trainings_video_training_id",
                table: "muscle_group");

            migrationBuilder.DropTable(
                name: "video_trainings");

            migrationBuilder.DropIndex(
                name: "ix_muscle_group_video_training_id",
                table: "muscle_group");

            migrationBuilder.DropColumn(
                name: "video_training_id",
                table: "muscle_group");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "personal_training",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "trainer_id",
                table: "group_training",
                type: "uuid",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "group_training",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "personal_training");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "group_training");

            migrationBuilder.AddColumn<Guid>(
                name: "video_training_id",
                table: "muscle_group",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "trainer_id",
                table: "group_training",
                type: "uuid",
                maxLength: 255,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "video_trainings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    training_type_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    complexity = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    duration_in_minutes = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_video_trainings", x => x.id);
                    table.ForeignKey(
                        name: "fk_video_trainings_training_type_training_type_id",
                        column: x => x.training_type_id,
                        principalTable: "training_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_muscle_group_video_training_id",
                table: "muscle_group",
                column: "video_training_id");

            migrationBuilder.CreateIndex(
                name: "ix_video_trainings_training_type_id",
                table: "video_trainings",
                column: "training_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_muscle_group_video_trainings_video_training_id",
                table: "muscle_group",
                column: "video_training_id",
                principalTable: "video_trainings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
