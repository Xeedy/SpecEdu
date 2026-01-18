using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.UserId)
            .HasMaxLength(450);

        builder.Property(al => al.UserName)
            .HasMaxLength(256);

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.EntityId)
            .HasMaxLength(450);

        builder.Property(al => al.Timestamp)
            .IsRequired();

        builder.Property(al => al.IpAddress)
            .HasMaxLength(45);

        builder.Property(al => al.UserAgent)
            .HasMaxLength(500);

        builder.Property(al => al.Details)
            .HasMaxLength(4000);

        builder.HasIndex(al => al.Timestamp);

        builder.HasIndex(al => al.UserId);

        builder.HasIndex(al => al.StudentId);

        builder.HasIndex(al => al.SchoolId);

        builder.HasIndex(al => new { al.SchoolId, al.Timestamp });
    }
}
