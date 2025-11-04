using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "base_group_trainings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    duration_in_minutes = table.Column<int>(type: "integer", nullable: false),
                    complexity = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_base_group_trainings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    instruction_add_before = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gym",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    image_relative_path = table.Column<string>(type: "text", nullable: true),
                    in_upload_process = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gym", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gym_admin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gym_admin", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gym_zone",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gym_zone", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "training_type",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_training_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    surname = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    last_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_online = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    user_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "visitor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_visitor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipment_instruction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    video_url = table.Column<string>(type: "text", nullable: false),
                    equipment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_instruction", x => x.id);
                    table.ForeignKey(
                        name: "fk_equipment_instruction_equipment_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "equipment_gym",
                columns: table => new
                {
                    equipments_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gyms_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_gym", x => new { x.equipments_id, x.gyms_id });
                    table.ForeignKey(
                        name: "fk_equipment_gym_equipment_equipments_id",
                        column: x => x.equipments_id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_equipment_gym_gym_gyms_id",
                        column: x => x.gyms_id,
                        principalTable: "gym",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gym_gym_admin",
                columns: table => new
                {
                    admins_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gyms_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gym_gym_admin", x => new { x.admins_id, x.gyms_id });
                    table.ForeignKey(
                        name: "fk_gym_gym_admin_gym_admin_admins_id",
                        column: x => x.admins_id,
                        principalTable: "gym_admin",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_gym_gym_admin_gym_gyms_id",
                        column: x => x.gyms_id,
                        principalTable: "gym",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gym_gym_zone",
                columns: table => new
                {
                    gyms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    zones_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gym_gym_zone", x => new { x.gyms_id, x.zones_id });
                    table.ForeignKey(
                        name: "fk_gym_gym_zone_gym_gyms_id",
                        column: x => x.gyms_id,
                        principalTable: "gym",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_gym_gym_zone_gym_zone_zones_id",
                        column: x => x.zones_id,
                        principalTable: "gym_zone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "video_trainings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    complexity = table.Column<int>(type: "integer", nullable: false),
                    duration_in_minutes = table.Column<int>(type: "integer", nullable: false),
                    video_url = table.Column<string>(type: "text", nullable: true),
                    training_type_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "trainer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trainer", x => x.id);
                    table.ForeignKey(
                        name: "fk_trainer_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "muscle_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true),
                    video_training_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_muscle_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_muscle_group_muscle_group_parent_id",
                        column: x => x.parent_id,
                        principalTable: "muscle_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_muscle_group_video_trainings_video_training_id",
                        column: x => x.video_training_id,
                        principalTable: "video_trainings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_training",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    base_group_training_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    trainer_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_training", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_training_base_group_trainings_base_group_training_id",
                        column: x => x.base_group_training_id,
                        principalTable: "base_group_trainings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_group_training_trainer_trainer_id",
                        column: x => x.trainer_id,
                        principalTable: "trainer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "personal_training",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    visitor_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    trainer_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_personal_training", x => x.id);
                    table.ForeignKey(
                        name: "fk_personal_training_trainer_trainer_id",
                        column: x => x.trainer_id,
                        principalTable: "trainer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_personal_training_visitor_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "group_training_visitor",
                columns: table => new
                {
                    group_trainings_id = table.Column<Guid>(type: "uuid", nullable: false),
                    participants_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_training_visitor", x => new { x.group_trainings_id, x.participants_id });
                    table.ForeignKey(
                        name: "fk_group_training_visitor_group_training_group_trainings_id",
                        column: x => x.group_trainings_id,
                        principalTable: "group_training",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_group_training_visitor_visitor_participants_id",
                        column: x => x.participants_id,
                        principalTable: "visitor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_equipment_gym_gyms_id",
                table: "equipment_gym",
                column: "gyms_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_instruction_equipment_id",
                table: "equipment_instruction",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_muscle_group_muscle_groups_id",
                table: "equipment_muscle_group",
                column: "muscle_groups_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_training_base_group_training_id",
                table: "group_training",
                column: "base_group_training_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_training_trainer_id",
                table: "group_training",
                column: "trainer_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_training_visitor_participants_id",
                table: "group_training_visitor",
                column: "participants_id");

            migrationBuilder.CreateIndex(
                name: "ix_gym_gym_admin_gyms_id",
                table: "gym_gym_admin",
                column: "gyms_id");

            migrationBuilder.CreateIndex(
                name: "ix_gym_gym_zone_zones_id",
                table: "gym_gym_zone",
                column: "zones_id");

            migrationBuilder.CreateIndex(
                name: "ix_muscle_group_parent_id",
                table: "muscle_group",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_muscle_group_video_training_id",
                table: "muscle_group",
                column: "video_training_id");

            migrationBuilder.CreateIndex(
                name: "ix_personal_training_trainer_id",
                table: "personal_training",
                column: "trainer_id");

            migrationBuilder.CreateIndex(
                name: "ix_personal_training_visitor_id",
                table: "personal_training",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "ix_trainer_user_id",
                table: "trainer",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_video_trainings_training_type_id",
                table: "video_trainings",
                column: "training_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "equipment_gym");

            migrationBuilder.DropTable(
                name: "equipment_instruction");

            migrationBuilder.DropTable(
                name: "equipment_muscle_group");

            migrationBuilder.DropTable(
                name: "group_training_visitor");

            migrationBuilder.DropTable(
                name: "gym_gym_admin");

            migrationBuilder.DropTable(
                name: "gym_gym_zone");

            migrationBuilder.DropTable(
                name: "personal_training");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "muscle_group");

            migrationBuilder.DropTable(
                name: "group_training");

            migrationBuilder.DropTable(
                name: "gym_admin");

            migrationBuilder.DropTable(
                name: "gym");

            migrationBuilder.DropTable(
                name: "gym_zone");

            migrationBuilder.DropTable(
                name: "visitor");

            migrationBuilder.DropTable(
                name: "video_trainings");

            migrationBuilder.DropTable(
                name: "base_group_trainings");

            migrationBuilder.DropTable(
                name: "trainer");

            migrationBuilder.DropTable(
                name: "training_type");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
