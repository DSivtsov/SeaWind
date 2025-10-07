using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<Tester> Testers => Set<Tester>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Tester>( action =>
        {
            action.HasKey(x => x.Id);
            action.Property(x => x.Name).IsRequired().HasMaxLength(200);
            action.Property(x => x.Age).IsRequired();
            action.ToTable("testers");
        }) ;
    }
}
