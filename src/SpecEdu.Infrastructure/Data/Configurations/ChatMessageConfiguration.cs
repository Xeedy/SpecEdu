using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderId).IsRequired().HasMaxLength(450);
        builder.Property(m => m.Content).IsRequired();

        // Primary query: "give me messages in conversation X ordered by time"
        builder.HasIndex(m => new { m.ConversationId, m.SentAt })
            .HasDatabaseName("IX_ChatMessages_ConvId_SentAt");

        // Secondary: "count unread messages sent after time X in conversation Y"
        builder.HasIndex(m => m.SenderId)
            .HasDatabaseName("IX_ChatMessages_SenderId");

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
