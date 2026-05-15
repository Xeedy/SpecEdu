using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipants");
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.UserId).IsRequired().HasMaxLength(450);

        builder.HasIndex(cp => new { cp.UserId, cp.IsActive })
            .HasDatabaseName("IX_ConvParticipants_UserId_IsActive");

        builder.HasIndex(cp => new { cp.ConversationId, cp.IsActive })
            .HasDatabaseName("IX_ConvParticipants_ConvId_IsActive");

        builder.HasOne(cp => cp.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
