using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Reminder entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("Reminders");
        builder.HasKey(r => r.Id);

        // String properties with MaxLength
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.Property(r => r.LastError)
            .HasMaxLength(1000);

        // Date properties
        builder.Property(r => r.DueDate)
            .IsRequired();

        builder.Property(r => r.NotifyAt)
            .IsRequired();

        // Enum properties
        builder.Property(r => r.Channel)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        // Boolean properties with defaults
        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Integer with default
        builder.Property(r => r.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Audit field configuration
        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(450);

        builder.Property(r => r.ModifiedBy)
            .HasMaxLength(450);

        // Relationships
        builder.HasOne(r => r.Student)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for efficient querying
        builder.HasIndex(r => r.StudentId);
        builder.HasIndex(r => r.NotifyAt);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => new { r.Status, r.NotifyAt, r.IsActive }); // For pending reminders query
        builder.HasIndex(r => new { r.StudentId, r.DueDate });
    }
}
