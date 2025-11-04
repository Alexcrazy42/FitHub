using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_BaseGroupTrainingPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "base_group_training_photo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    training_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_base_group_training_photo", x => x.id);
                    table.ForeignKey(
                        name: "fk_base_group_training_photo_base_group_trainings_training_id",
                        column: x => x.training_id,
                        principalTable: "base_group_trainings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_base_group_training_photo_file_entity_file_id",
                        column: x => x.file_id,
                        principalTable: "file_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_base_group_training_photo_file_id",
                table: "base_group_training_photo",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_base_group_training_photo_training_id",
                table: "base_group_training_photo",
                column: "training_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "base_group_training_photo");
        }
    }
}
