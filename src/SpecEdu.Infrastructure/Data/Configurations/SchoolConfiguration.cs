using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the School entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("Schools");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Ico)
            .HasMaxLength(8);

        builder.Property(s => s.Address)
            .HasMaxLength(200);

        builder.Property(s => s.City)
            .HasMaxLength(100);

        builder.Property(s => s.PostalCode)
            .HasMaxLength(10);

        builder.Property(s => s.ContactEmail)
            .HasMaxLength(200);

        builder.Property(s => s.ContactPhone)
            .HasMaxLength(50);

        builder.Property(s => s.InstitutionType)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Škola");

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(450);

        builder.Property(s => s.ModifiedBy)
            .HasMaxLength(450);

        builder.HasIndex(s => s.Ico)
            .IsUnique()
            .HasFilter("[Ico] IS NOT NULL");

        builder.HasIndex(s => s.Name);

        builder.HasIndex(s => s.IsActive);
    }
}
