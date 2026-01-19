using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the DiaryEntry entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class DiaryEntryConfiguration : IEntityTypeConfiguration<DiaryEntry>
{
    public void Configure(EntityTypeBuilder<DiaryEntry> builder)
    {
        builder.ToTable("DiaryEntries");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(d => d.Type)
            .IsRequired();

        builder.Property(d => d.Visibility)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.CreatedBy)
            .HasMaxLength(450);

        builder.Property(d => d.ModifiedBy)
            .HasMaxLength(450);

        // Relationship to Student
        builder.HasOne(d => d.Student)
            .WithMany(s => s.DiaryEntries)
            .HasForeignKey(d => d.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for filtering and sorting
        builder.HasIndex(d => d.StudentId);

        builder.HasIndex(d => new { d.StudentId, d.CreatedAt });

        builder.HasIndex(d => new { d.StudentId, d.Type });

        builder.HasIndex(d => new { d.StudentId, d.Visibility });

        builder.HasIndex(d => d.CreatedBy);

        builder.HasIndex(d => d.IsActive);
    }
}
