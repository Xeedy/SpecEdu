using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the StudentStaffLink entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class StudentStaffLinkConfiguration : IEntityTypeConfiguration<StudentStaffLink>
{
    public void Configure(EntityTypeBuilder<StudentStaffLink> builder)
    {
        builder.ToTable("StudentStaffLinks");

        builder.HasKey(ssl => ssl.Id);

        builder.Property(ssl => ssl.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(ssl => ssl.LinkType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ssl => ssl.AccessLevel)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ssl => ssl.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ssl => ssl.CreatedAt)
            .IsRequired();

        builder.Property(ssl => ssl.CreatedBy)
            .HasMaxLength(450);

        builder.Property(ssl => ssl.ModifiedBy)
            .HasMaxLength(450);

        // Relationship to Student
        builder.HasOne(ssl => ssl.Student)
            .WithMany(s => s.StaffLinks)
            .HasForeignKey(ssl => ssl.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        // Unique constraint: one staff can only have one link type per student
        builder.HasIndex(ssl => new { ssl.StudentId, ssl.UserId, ssl.LinkType })
            .IsUnique();

        builder.HasIndex(ssl => ssl.UserId);
    }
}
