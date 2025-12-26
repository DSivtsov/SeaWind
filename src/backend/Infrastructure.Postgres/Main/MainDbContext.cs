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


/*        builder.Entity<Course>(action =>
        {
            action.HasKey(c => c.Id);
            action.Property(c => c.Title).IsRequired().HasMaxLength(255);
            action.Property(c => c.Code).IsRequired().HasMaxLength(255);
            action.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp") // без time zone
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            action.Property(c => c.UpdatedAt).HasColumnType("timestamp"); // без time zone
        });*/
    }
}
