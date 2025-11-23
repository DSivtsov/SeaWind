using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders.Shared;

public class TestDbContext : DbContext
{
    public TestDbContext() : base() { }

    public const string Schema = "test";

    public DbSet<TestEntity> TestTable => Set<TestEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        // Таблицы будут создаваться в схеме public
        builder.Entity<TestEntity>(action =>
        {
            action.HasKey(x => x.Id);
            action.Property(x => x.Value).IsRequired().HasMaxLength(200);
            action.ToTable("testtable");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString()); // полная изоляция БД
        //.EnableDetailedErrors()
        //.EnableSensitiveDataLogging()
    }
}
