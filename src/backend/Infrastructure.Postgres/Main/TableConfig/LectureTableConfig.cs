using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Main.TableConfig;

public sealed class LectureTableConfig : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> entity)
    {
        entity.ToTable("lectures");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .IsRequired();

        entity.Property(x => x.CourseId)
            .IsRequired()
            .HasMaxLength(32);

        entity.HasOne<Course>()
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(x => x.OrderNo)
            .IsRequired();

        entity.ToTable(t => t.HasCheckConstraint(
            "CK_lectures_OrderNo",
            "\"OrderNo\" > 0"
        ));

        entity.HasIndex(x => new { x.CourseId, x.OrderNo })
            .IsUnique();

        entity.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(x => x.VideoUrl)
            .HasMaxLength(2000);

        entity.Property(x => x.Description)
            .HasMaxLength(2000);

        entity.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(16)
            .HasConversion<string>();

        entity.ToTable(t => t.HasCheckConstraint(
            "CK_lectures_Status",
            "\"Status\" IN ('Published','Draft')"
        ));

        entity.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp") // without time zone
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.Property(x => x.UpdatedAt)
            .HasColumnType("timestamp"); // without time zone
    }
}
