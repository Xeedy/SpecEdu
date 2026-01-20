using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the PlppVersion entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class PlppVersionConfiguration : IEntityTypeConfiguration<PlppVersion>
{
    public void Configure(EntityTypeBuilder<PlppVersion> builder)
    {
        builder.ToTable("PlppVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.Snapshot)
            .IsRequired();

        builder.Property(v => v.ChangeSummary)
            .HasMaxLength(1000);

        builder.Property(v => v.Source)
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.CreatedBy)
            .HasMaxLength(450);

        builder.Property(v => v.ModifiedBy)
            .HasMaxLength(450);

        // Relationship to Plpp
        builder.HasOne(v => v.Plpp)
            .WithMany(p => p.Versions)
            .HasForeignKey(v => v.PlppId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(v => v.PlppId);

        builder.HasIndex(v => new { v.PlppId, v.VersionNumber })
            .IsUnique();

        builder.HasIndex(v => v.Source);

        builder.HasIndex(v => v.CreatedAt);
    }
}
