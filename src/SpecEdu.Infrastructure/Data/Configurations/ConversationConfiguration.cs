using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).HasMaxLength(200);
        builder.Property(c => c.LastMessagePreview).HasMaxLength(200);

        builder.HasIndex(c => c.LastMessageAt)
            .HasDatabaseName("IX_Conversations_LastMessageAt")
            .IsDescending();
    }
}
