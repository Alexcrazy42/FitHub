using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActualizeMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<Guid>(
                name: "gym_id",
                table: "group_training",
                type: "uuid",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "gym_trainer",
                columns: table => new
                {
                    gyms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trainers_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gym_trainer", x => new { x.gyms_id, x.trainers_id });
                    table.ForeignKey(
                        name: "fk_gym_trainer_gym_gyms_id",
                        column: x => x.gyms_id,
                        principalTable: "gym",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_gym_trainer_trainer_trainers_id",
                        column: x => x.trainers_id,
                        principalTable: "trainer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_group_training_gym_id",
                table: "group_training",
                column: "gym_id");

            migrationBuilder.CreateIndex(
                name: "ix_gym_trainer_trainers_id",
                table: "gym_trainer",
                column: "trainers_id");

            migrationBuilder.AddForeignKey(
                name: "fk_group_training_gym_gym_id",
                table: "group_training",
                column: "gym_id",
                principalTable: "gym",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_group_training_gym_gym_id",
                table: "group_training");

            migrationBuilder.DropTable(
                name: "gym_trainer");

            migrationBuilder.DropIndex(
                name: "ix_group_training_gym_id",
                table: "group_training");

            migrationBuilder.DropColumn(
                name: "gym_id",
                table: "group_training");

            migrationBuilder.AlterColumn<Guid>(
                name: "trainer_id",
                table: "group_training",
                type: "uuid",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 255);
        }
    }
}
