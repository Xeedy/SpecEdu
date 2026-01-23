using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class ConsultationEventConfiguration : IEntityTypeConfiguration<ConsultationEvent>
{
    public void Configure(EntityTypeBuilder<ConsultationEvent> builder)
    {
        builder.ToTable("ConsultationEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(4000);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.StartTime)
            .IsRequired();

        builder.Property(e => e.EndTime)
            .IsRequired();

        builder.Property(e => e.Location)
            .HasMaxLength(500);

        builder.Property(e => e.OnlineMeetingLink)
            .HasMaxLength(1000);

        builder.Property(e => e.Notes)
            .HasMaxLength(4000);

        builder.Property(e => e.OrganizerId)
            .HasMaxLength(450);

        builder.Property(e => e.IsVisibleToParents)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.AllowResponses)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.ReminderMinutesBefore)
            .HasDefaultValue(1440);

        builder.Property(e => e.ReminderSent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(450);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(450);

        builder.HasOne(e => e.School)
            .WithMany()
            .HasForeignKey(e => e.SchoolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Plpp)
            .WithMany()
            .HasForeignKey(e => e.PlppId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.SchoolId);

        builder.HasIndex(e => e.StudentId);

        builder.HasIndex(e => new { e.SchoolId, e.StartTime });

        builder.HasIndex(e => new { e.SchoolId, e.Status });

        builder.HasIndex(e => e.StartTime);

        builder.HasIndex(e => e.Type);

        builder.HasIndex(e => e.Status);

        builder.HasIndex(e => e.IsActive);

        builder.HasIndex(e => new { e.ReminderSent, e.StartTime })
            .HasFilter("[ReminderMinutesBefore] IS NOT NULL AND [ReminderSent] = 0");
    }
}
