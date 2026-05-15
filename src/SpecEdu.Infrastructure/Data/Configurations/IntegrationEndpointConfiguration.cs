using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class IntegrationEndpointConfiguration : IEntityTypeConfiguration<IntegrationEndpoint>
{
    public void Configure(EntityTypeBuilder<IntegrationEndpoint> builder)
    {
        builder.ToTable("IntegrationEndpoints");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.BaseUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.ApiKeyPlaceholder)
            .HasMaxLength(500);

        builder.Property(e => e.LastTestResult)
            .HasMaxLength(500);

        builder.HasIndex(e => e.SystemType)
            .HasDatabaseName("IX_IntegrationEndpoints_SystemType");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_IntegrationEndpoints_IsActive");

        builder.HasIndex(e => new { e.SystemType, e.IsActive })
            .HasDatabaseName("IX_IntegrationEndpoints_SystemType_IsActive");

        builder.HasMany(e => e.ExchangeRecords)
            .WithOne(r => r.Endpoint)
            .HasForeignKey(r => r.EndpointId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
