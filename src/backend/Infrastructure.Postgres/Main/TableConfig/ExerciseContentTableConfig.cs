using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Models;

public sealed class ExerciseContentTableConfig : IEntityTypeConfiguration<ExerciseContent>
{
    public void Configure(EntityTypeBuilder<ExerciseContent> entity)
    {
        entity.ToTable("exercise_content", t =>
        {
            t.HasCheckConstraint(
                "CK_ExerciseContent_ContentBlocks_IsArray",
                "jsonb_typeof(content_blocks) = 'array'"
            );
        });

        entity.HasKey(x => x.ExerciseId);

        entity.HasOne<Exercise>()
            .WithOne()
            .HasForeignKey<ExerciseContent>(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(x => x.Details)
            .IsRequired()
            .HasColumnType("text");

        entity.Property(x => x.ContentBlocksJson)
            .HasColumnName("content_blocks")
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
