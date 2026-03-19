using Application.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Postgres.Main.TableConfig;

public sealed class StudentExerciseTableConfig : IEntityTypeConfiguration<StudentExercise>
{
    public void Configure(EntityTypeBuilder<StudentExercise> entity)
    {
        entity.ToTable("student_exercises");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.StudentId)
            .IsRequired()
            .HasMaxLength(36);

        /* FK в другой контекст не поддерживается,
            поэтому связь через код - JOIN "ручками"

         entity.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        */

        entity.Property(x => x.ExerciseId)
            .IsRequired();

        entity.HasOne<Exercise>()
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.StudentId, x.ExerciseId })
            .IsUnique();

        entity.Property(x => x.AssignedMentorId)
            .HasMaxLength(36);

        entity.Property(x => x.CourseId)
            .IsRequired()
            .HasMaxLength(32);

        entity.HasOne<Course>()
            .WithMany()
            .HasForeignKey(x => x.CourseId);

        entity.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(12)
            .HasConversion<string>();

        entity.ToTable(t => t.HasCheckConstraint(
            "CK_student_exercises_Status",
            "\"Status\" IN ('OnStudent','OnMentor')"
        ));

        entity.Property(x => x.Mark);

        entity.ToTable(t => t.HasCheckConstraint(
            "CK_student_exercises_Mark",
            "\"Mark\" IS NULL OR (\"Mark\" BETWEEN 0 AND 2)"
        ));

        entity.Property(x => x.RequestedCheckAt)
            .HasColumnType("timestamp"); // without time zone

        entity.Property(x => x.CheckedAt)
            .HasColumnType("timestamp"); // without time zone

        entity.Property(x => x.ThreadId)
            .IsRequired()
            .HasMaxLength(24);
    }
}
