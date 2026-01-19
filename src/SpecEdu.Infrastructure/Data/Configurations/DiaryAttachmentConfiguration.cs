using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the DiaryAttachment entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class DiaryAttachmentConfiguration : IEntityTypeConfiguration<DiaryAttachment>
{
    public void Configure(EntityTypeBuilder<DiaryAttachment> builder)
    {
        builder.ToTable("DiaryAttachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.FileData)
            .IsRequired();

        builder.Property(a => a.FileSize)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(450);

        builder.Property(a => a.ModifiedBy)
            .HasMaxLength(450);

        // Relationship to DiaryEntry
        builder.HasOne(a => a.DiaryEntry)
            .WithMany(d => d.Attachments)
            .HasForeignKey(a => a.DiaryEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for querying attachments by entry
        builder.HasIndex(a => a.DiaryEntryId);
    }
}
