using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Postgres.Main.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentExercise_Add_AssignedMentorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedMentorId",
                schema: "main",
                table: "student_exercises",
                type: "character varying(36)",
                maxLength: 36,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedMentorId",
                schema: "main",
                table: "student_exercises");
        }
    }
}
