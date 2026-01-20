using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Data.Configurations;

public class PlppEvaluationConfiguration : IEntityTypeConfiguration<PlppEvaluation>
{
    public void Configure(EntityTypeBuilder<PlppEvaluation> builder)
    {
        builder.ToTable("PlppEvaluations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EvaluationMonth)
            .IsRequired();

        builder.Property(e => e.WhatStudentManages)
            .HasMaxLength(4000);

        builder.Property(e => e.WhatNeedsImprovement)
            .HasMaxLength(4000);

        builder.Property(e => e.RecommendedAdjustments)
            .HasMaxLength(4000);

        builder.Property(e => e.ParentConsultationNotes)
            .HasMaxLength(4000);

        builder.Property(e => e.ProgressRating);

        builder.Property(e => e.Notes)
            .HasMaxLength(4000);

        builder.Property(e => e.ParentsNotified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(450);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(450);

        builder.HasOne(e => e.Plpp)
            .WithMany(p => p.Evaluations)
            .HasForeignKey(e => e.PlppId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.PlppId);

        builder.HasIndex(e => new { e.PlppId, e.EvaluationMonth });

        builder.HasIndex(e => e.EvaluationMonth);

        builder.HasIndex(e => e.ParentsNotified);

        builder.HasIndex(e => e.IsActive);
    }
}
