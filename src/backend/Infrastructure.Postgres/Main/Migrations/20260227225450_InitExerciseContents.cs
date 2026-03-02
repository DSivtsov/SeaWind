using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Postgres.Main.Migrations
{
    /// <inheritdoc />
    public partial class InitExerciseContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise_content",
                schema: "main",
                columns: table => new
                {
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    content_blocks = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_content", x => x.ExerciseId);
                    table.CheckConstraint("CK_ExerciseContent_ContentBlocks_IsArray", "jsonb_typeof(content_blocks) = 'array'");
                    table.ForeignKey(
                        name: "FK_exercise_content_exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalSchema: "main",
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_content",
                schema: "main");
        }
    }
}
