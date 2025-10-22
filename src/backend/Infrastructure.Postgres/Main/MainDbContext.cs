using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main;

public class MainDbContext : DbContext
{
    public const string Schema = "main";
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

    //public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);
        // Таблицы будут создаваться в схеме main
        //builder.Entity<Course>(action =>
        //{

        //});
    }
}
