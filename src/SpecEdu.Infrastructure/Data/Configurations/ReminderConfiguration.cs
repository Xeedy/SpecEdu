using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("Reminders");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.Property(r => r.LastError)
            .HasMaxLength(1000);

        builder.Property(r => r.DueDate)
            .IsRequired();

        builder.Property(r => r.NotifyAt)
            .IsRequired();

        builder.Property(r => r.Channel)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(450);

        builder.Property(r => r.ModifiedBy)
            .HasMaxLength(450);

        builder.HasOne(r => r.Student)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.StudentId);
        builder.HasIndex(r => r.NotifyAt);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => new { r.Status, r.NotifyAt, r.IsActive });
        builder.HasIndex(r => new { r.StudentId, r.DueDate });
    }
}
