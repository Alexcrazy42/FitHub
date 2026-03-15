using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity");

            migrationBuilder.DropTable(
                name: "equipment_muscle_group");

            migrationBuilder.DropColumn(
                name: "video_url",
                table: "video_trainings");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "muscle_group");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "equipment");

            migrationBuilder.RenameColumn(
                name: "video_url",
                table: "equipment_instruction",
                newName: "name");

            migrationBuilder.AddColumn<string>(
                name: "additional_description",
                table: "equipment_instruction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "equipment_instruction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "additional_descroption",
                table: "equipment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "equipment",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "equipment_instruction_muscle_group",
                columns: table => new
                {
                    equipment_instruction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    muscle_groups_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_instruction_muscle_group", x => new { x.equipment_instruction_id, x.muscle_groups_id });
                    table.ForeignKey(
                        name: "fk_equipment_instruction_muscle_group_equipment_instruction_eq",
                        column: x => x.equipment_instruction_id,
                        principalTable: "equipment_instruction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_equipment_instruction_muscle_group_muscle_group_muscle_grou",
                        column: x => x.muscle_groups_id,
                        principalTable: "muscle_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_equipment_instruction_muscle_group_muscle_groups_id",
                table: "equipment_instruction_muscle_group",
                column: "muscle_groups_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "equipment_instruction_muscle_group");

            migrationBuilder.DropColumn(
                name: "additional_description",
                table: "equipment_instruction");

            migrationBuilder.DropColumn(
                name: "description",
                table: "equipment_instruction");

            migrationBuilder.DropColumn(
                name: "additional_descroption",
                table: "equipment");

            migrationBuilder.DropColumn(
                name: "description",
                table: "equipment");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "equipment_instruction",
                newName: "video_url");

            migrationBuilder.AddColumn<string>(
                name: "video_url",
                table: "video_trainings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "muscle_group",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "equipment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "entity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    max_file_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipment_muscle_group",
                columns: table => new
                {
                    equipments_id = table.Column<Guid>(type: "uuid", nullable: false),
                    muscle_groups_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_muscle_group", x => new { x.equipments_id, x.muscle_groups_id });
                    table.ForeignKey(
                        name: "fk_equipment_muscle_group_equipment_equipments_id",
                        column: x => x.equipments_id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_equipment_muscle_group_muscle_group_muscle_groups_id",
                        column: x => x.muscle_groups_id,
                        principalTable: "muscle_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "entity",
                columns: new[] { "id", "entity_type", "max_file_count" },
                values: new object[] { new Guid("6206cfea-d518-4b09-9257-0f790473545e"), "Gym", 1 });

            migrationBuilder.CreateIndex(
                name: "ix_equipment_muscle_group_muscle_groups_id",
                table: "equipment_muscle_group",
                column: "muscle_groups_id");
        }
    }
}
