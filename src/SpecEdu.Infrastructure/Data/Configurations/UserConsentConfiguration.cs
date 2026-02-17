using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class UserConsentConfiguration : IEntityTypeConfiguration<UserConsent>
{
    public void Configure(EntityTypeBuilder<UserConsent> builder)
    {
        builder.ToTable("UserConsents");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(c => c.ConsentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.IpAddress)
            .HasMaxLength(45);

        builder.HasIndex(c => new { c.UserId, c.ConsentType })
            .HasDatabaseName("IX_UserConsents_UserId_ConsentType");
    }
}
