using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Time;

public sealed class TimeDbContext : DbContext
{
    public TimeDbContext(DbContextOptions<TimeDbContext> opts) : base(opts) { }

    public DbSet<Tester> Testers => Set<Tester>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Таблицы будут создаваться в схеме public
        builder.Entity<Tester>( action =>
        {
            action.HasKey(x => x.Id);
            action.Property(x => x.Name).IsRequired().HasMaxLength(200);
            action.Property(x => x.Age).IsRequired();
            action.ToTable("testers");
        }) ;
    }
}
