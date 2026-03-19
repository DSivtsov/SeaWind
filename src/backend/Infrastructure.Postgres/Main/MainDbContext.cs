using Application.Models;
using Infrastructure.Postgres.Main.TableConfig;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main;

public class MainDbContext : DbContext
{
    public const string Schema = "main";
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Lecture> Lectures => Set<Lecture>();

    public DbSet<Exercise> Exercises => Set<Exercise>();

    public DbSet<ExerciseContent> ExerciseContents => Set<ExerciseContent>();

    public DbSet<StudentExercise> StudentExercises => Set<StudentExercise>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Таблицы будут создаваться в схеме main
        builder.HasDefaultSchema(Schema);

        builder.ApplyConfigurationsFromAssembly(typeof(CourseTableConfig).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(LectureTableConfig).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ExerciseTableConfig).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(ExerciseContentTableConfig).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(StudentExercise).Assembly);
    }
}
