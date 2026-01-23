using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the ConsultationParticipant entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class ConsultationParticipantConfiguration : IEntityTypeConfiguration<ConsultationParticipant>
{
    public void Configure(EntityTypeBuilder<ConsultationParticipant> builder)
    {
        builder.ToTable("ConsultationParticipants");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .HasMaxLength(450);

        builder.Property(p => p.ExternalName)
            .HasMaxLength(200);

        builder.Property(p => p.ExternalEmail)
            .HasMaxLength(256);

        builder.Property(p => p.RoleDescription)
            .HasMaxLength(100);

        builder.Property(p => p.ResponseStatus)
            .IsRequired();

        builder.Property(p => p.ResponseNote)
            .HasMaxLength(500);

        builder.Property(p => p.IsOrganizer)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsRequired)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.NotificationSent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(450);

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(450);

        // Relationship to ConsultationEvent
        builder.HasOne(p => p.ConsultationEvent)
            .WithMany(e => e.Participants)
            .HasForeignKey(p => p.ConsultationEventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.ConsultationEventId);

        builder.HasIndex(p => p.UserId);

        builder.HasIndex(p => new { p.ConsultationEventId, p.UserId });

        builder.HasIndex(p => p.ResponseStatus);

        builder.HasIndex(p => new { p.NotificationSent, p.ConsultationEventId });
    }
}
