using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_BaseGroupTraining_Types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type_id",
                table: "base_group_trainings");

            migrationBuilder.CreateTable(
                name: "base_group_training_training_type",
                columns: table => new
                {
                    base_group_training_id = table.Column<Guid>(type: "uuid", nullable: false),
                    training_types_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_base_group_training_training_type", x => new { x.base_group_training_id, x.training_types_id });
                    table.ForeignKey(
                        name: "fk_base_group_training_training_type_base_group_trainings_base",
                        column: x => x.base_group_training_id,
                        principalTable: "base_group_trainings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_base_group_training_training_type_training_type_training_ty",
                        column: x => x.training_types_id,
                        principalTable: "training_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_base_group_training_training_type_training_types_id",
                table: "base_group_training_training_type",
                column: "training_types_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "base_group_training_training_type");

            migrationBuilder.AddColumn<Guid>(
                name: "type_id",
                table: "base_group_trainings",
                type: "uuid",
                maxLength: 255,
                nullable: true);
        }
    }
}
