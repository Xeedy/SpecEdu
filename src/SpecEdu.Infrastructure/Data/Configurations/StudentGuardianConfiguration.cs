using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the StudentGuardian entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class StudentGuardianConfiguration : IEntityTypeConfiguration<StudentGuardian>
{
    public void Configure(EntityTypeBuilder<StudentGuardian> builder)
    {
        builder.ToTable("StudentGuardians");

        builder.HasKey(sg => sg.Id);

        builder.Property(sg => sg.ParentUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(sg => sg.RelationshipType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(sg => sg.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(sg => sg.CreatedAt)
            .IsRequired();

        builder.Property(sg => sg.CreatedBy)
            .HasMaxLength(450);

        builder.Property(sg => sg.ModifiedBy)
            .HasMaxLength(450);

        // Relationship to Student
        builder.HasOne(sg => sg.Student)
            .WithMany(s => s.Guardians)
            .HasForeignKey(sg => sg.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        // Unique constraint: one parent can only have one relationship type per student
        builder.HasIndex(sg => new { sg.StudentId, sg.ParentUserId })
            .IsUnique();

        builder.HasIndex(sg => sg.ParentUserId);
    }
}
