using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class DataExchangeRecordConfiguration : IEntityTypeConfiguration<DataExchangeRecord>
{
    public void Configure(EntityTypeBuilder<DataExchangeRecord> builder)
    {
        builder.ToTable("DataExchangeRecords");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.EntityType)
            .HasMaxLength(100);

        builder.Property(r => r.RequestSummary)
            .HasMaxLength(2000);

        builder.Property(r => r.ResponseSummary)
            .HasMaxLength(2000);

        builder.Property(r => r.InitiatedBy)
            .HasMaxLength(450);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => r.EndpointId)
            .HasDatabaseName("IX_DataExchangeRecords_EndpointId");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_DataExchangeRecords_Status");

        builder.HasIndex(r => r.CreatedAt)
            .HasDatabaseName("IX_DataExchangeRecords_CreatedAt");

        builder.HasIndex(r => new { r.EndpointId, r.Status })
            .HasDatabaseName("IX_DataExchangeRecords_EndpointId_Status");

        builder.HasIndex(r => new { r.EndpointId, r.CreatedAt })
            .HasDatabaseName("IX_DataExchangeRecords_EndpointId_CreatedAt");

        // Filtered index for non-terminal records (Pending, InProgress)
        builder.HasIndex(r => r.Status)
            .HasFilter("[Status] IN (1, 2)")
            .HasDatabaseName("IX_DataExchangeRecords_Status_NonTerminal");
    }
}
