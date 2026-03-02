using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Main.TableConfig;

public sealed class ExerciseTableConfig : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> entity)
    {
        entity.ToTable("exercises");

        entity.HasKey(x => x.Id);

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
            "CK_exercises_OrderNo",
            "\"OrderNo\" > 0"
        ));

        entity.HasIndex(x => new { x.CourseId, x.OrderNo })
            .IsUnique();

        entity.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(x => x.ShortDescription)
            .HasMaxLength(400);
    }
}
