using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Main.TableConfig;

public sealed class CourseTableConfig : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> entity)
    {
        entity.ToTable("courses");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(32);

        // Postgres regex check constraint:
        entity.ToTable(t => t.HasCheckConstraint(
            "CK_courses_Id_format",
            "\"Id\" ~ '^[a-z0-9]+(-[a-z0-9]+)*$'"
        ));

        // Запрет изменения PK после insert (EF-level)
        entity.Property(x => x.Id)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        entity.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(x => x.Description)
            .HasMaxLength(2000);

        entity.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp") // без time zone
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.Property(c => c.UpdatedAt).HasColumnType("timestamp"); // без time zone
    }
}
