using Application.Models;
using Infrastructure.Postgres.Main.TableConfig;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main;

public class MainDbContext : DbContext
{
    public const string Schema = "main";
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Таблицы будут создаваться в схеме main
        builder.HasDefaultSchema(Schema);

        builder.ApplyConfigurationsFromAssembly(typeof(CourseTableConfig).Assembly);

    }
}
