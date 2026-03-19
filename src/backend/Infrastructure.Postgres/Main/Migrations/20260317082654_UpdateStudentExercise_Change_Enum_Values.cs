using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Postgres.Main.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentExercise_Change_Enum_Values : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_student_exercises_Status",
                schema: "main",
                table: "student_exercises");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "main",
                table: "student_exercises",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8);

            migrationBuilder.Sql("""
                UPDATE main.student_exercises
                SET "Status" = 'OnStudent'
                WHERE "Status" = 'OnWork';
            """);

            migrationBuilder.Sql("""
                UPDATE main.student_exercises
                SET "Status" = 'OnMentor'
                WHERE "Status" = 'OnCheck';
            """);

            migrationBuilder.Sql("""
                UPDATE main.student_exercises
                SET "Status" = 'Accepted'
                WHERE "Status" = 'Checked';
            """);

            migrationBuilder.AddCheckConstraint(
                name: "CK_student_exercises_Status",
                schema: "main",
                table: "student_exercises",
                sql: "\"Status\" IN ('OnStudent','OnMentor','Accepted')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_student_exercises_Status",
                schema: "main",
                table: "student_exercises");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "main",
                table: "student_exercises",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AddCheckConstraint(
                name: "CK_student_exercises_Status",
                schema: "main",
                table: "student_exercises",
                sql: "\"Status\" IN ('OnWork','OnCheck','Checked')");
        }
    }
}
