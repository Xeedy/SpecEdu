using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class PlppConfiguration : IEntityTypeConfiguration<Plpp>
{
    public void Configure(EntityTypeBuilder<Plpp> builder)
    {
        builder.ToTable("Plpps");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.SchoolYear)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.SupportLevel)
            .IsRequired();

        builder.Property(p => p.ValidFrom)
            .IsRequired();

        builder.Property(p => p.ValidTo)
            .IsRequired();

        builder.Property(p => p.Strengths)
            .HasMaxLength(4000);

        builder.Property(p => p.AreasNeedingSupport)
            .HasMaxLength(4000);

        builder.Property(p => p.RecommendedMethods)
            .HasMaxLength(4000);

        builder.Property(p => p.OrganizationalAdjustments)
            .HasMaxLength(4000);

        builder.Property(p => p.ContentAdjustments)
            .HasMaxLength(4000);

        builder.Property(p => p.AssessmentMethods)
            .HasMaxLength(4000);

        builder.Property(p => p.ParentCollaboration)
            .HasMaxLength(4000);

        builder.Property(p => p.InternalNotes)
            .HasMaxLength(4000);

        builder.Property(p => p.ActivatedBy)
            .HasMaxLength(450);

        builder.Property(p => p.IsVisibleToParents)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(450);

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(450);

        builder.HasOne(p => p.Student)
            .WithMany(s => s.Plpps)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.StudentId);

        builder.HasIndex(p => new { p.StudentId, p.SchoolYear });

        builder.HasIndex(p => new { p.StudentId, p.Status });

        builder.HasIndex(p => p.Status);

        builder.HasIndex(p => new { p.ValidFrom, p.ValidTo });

        builder.HasIndex(p => p.IsActive);
    }
}
