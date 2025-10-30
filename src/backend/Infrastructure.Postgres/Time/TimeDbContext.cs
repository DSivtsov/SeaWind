using Application.ModelsTime;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Time;

public sealed class TimeDbContext : DbContext
{
    public TimeDbContext(DbContextOptions<TimeDbContext> opts) : base(opts) { }

    public const string Schema = "public";

    public DbSet<Tester> Testers => Set<Tester>();
    public DbSet<Car> Cars => Set<Car>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        // Таблицы будут создаваться в схеме public
        builder.Entity<Tester>( action =>
        {
            action.HasKey(x => x.Id);
            action.Property(x => x.Name).IsRequired().HasMaxLength(200);
            action.Property(x => x.Age).IsRequired();
            action.ToTable("testers");
        }) ;

        builder.Entity<Car>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Model).IsRequired().HasMaxLength(200);
            entity.Property(x => x.RegNumber).IsRequired().HasMaxLength(50);

            entity
                .HasOne<Tester>()
                .WithMany()
                .HasForeignKey(x => x.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("cars");
        });
    }
}
