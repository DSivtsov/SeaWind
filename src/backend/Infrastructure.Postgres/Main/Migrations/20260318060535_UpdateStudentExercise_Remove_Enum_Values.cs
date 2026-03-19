using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Postgres.Main.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentExercise_Remove_Enum_Values : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_student_exercises_Status",
                schema: "main",
                table: "student_exercises");

            migrationBuilder.AddCheckConstraint(
                name: "CK_student_exercises_Status",
                schema: "main",
                table: "student_exercises",
                sql: "\"Status\" IN ('OnStudent','OnMentor')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_student_exercises_Status",
                schema: "main",
                table: "student_exercises");

            migrationBuilder.AddCheckConstraint(
                name: "CK_student_exercises_Status",
                schema: "main",
                table: "student_exercises",
                sql: "\"Status\" IN ('OnStudent','OnMentor','Accepted')");
        }
    }
}
