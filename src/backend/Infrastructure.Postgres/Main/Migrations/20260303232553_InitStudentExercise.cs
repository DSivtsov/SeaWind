using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Postgres.Main.Migrations
{
    /// <inheritdoc />
    public partial class InitStudentExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_exercises",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Mark = table.Column<int>(type: "integer", nullable: true),
                    RequestedCheckAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CheckedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ThreadId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_exercises", x => x.Id);
                    table.CheckConstraint("CK_student_exercises_Mark", "\"Mark\" IS NULL OR (\"Mark\" BETWEEN 0 AND 2)");
                    table.CheckConstraint("CK_student_exercises_Status", "\"Status\" IN ('OnWork','OnCheck','Checked')");
                    table.ForeignKey(
                        name: "FK_student_exercises_courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "main",
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_student_exercises_exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalSchema: "main",
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_exercises_CourseId",
                schema: "main",
                table: "student_exercises",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_student_exercises_ExerciseId",
                schema: "main",
                table: "student_exercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_student_exercises_StudentId_ExerciseId",
                schema: "main",
                table: "student_exercises",
                columns: new[] { "StudentId", "ExerciseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_exercises",
                schema: "main");
        }
    }
}
