using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the PlppGoal entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class PlppGoalConfiguration : IEntityTypeConfiguration<PlppGoal>
{
    public void Configure(EntityTypeBuilder<PlppGoal> builder)
    {
        builder.ToTable("PlppGoals");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Order)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(g => g.Subject)
            .HasMaxLength(200);

        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(g => g.SuccessCriteria)
            .HasMaxLength(2000);

        builder.Property(g => g.Methods)
            .HasMaxLength(2000);

        builder.Property(g => g.ResponsiblePerson)
            .HasMaxLength(200);

        builder.Property(g => g.Status)
            .IsRequired();

        builder.Property(g => g.ProgressNotes)
            .HasMaxLength(4000);

        builder.Property(g => g.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.CreatedBy)
            .HasMaxLength(450);

        builder.Property(g => g.ModifiedBy)
            .HasMaxLength(450);

        // Relationship to Plpp
        builder.HasOne(g => g.Plpp)
            .WithMany(p => p.Goals)
            .HasForeignKey(g => g.PlppId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for filtering and sorting
        builder.HasIndex(g => g.PlppId);

        builder.HasIndex(g => new { g.PlppId, g.Order });

        builder.HasIndex(g => new { g.PlppId, g.Status });

        builder.HasIndex(g => g.Status);

        builder.HasIndex(g => g.IsActive);
    }
}
