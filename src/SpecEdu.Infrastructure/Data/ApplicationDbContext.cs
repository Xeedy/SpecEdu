using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Domain.Common;
using SpecEdu.Domain.Entities;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Data;

/// <summary>
/// Main database context for SpecEdu application.
/// Inherits from IdentityDbContext for user/role management.
/// Handles automatic population of audit fields on save.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>, IDataProtectionKeyContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Data protection keys for cross-app authentication.
    /// </summary>
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    /// <summary>
    /// Schools/institutions (tenants) in the system.
    /// </summary>
    public DbSet<School> Schools => Set<School>();

    /// <summary>
    /// Students with special educational needs.
    /// </summary>
    public DbSet<Student> Students => Set<Student>();

    /// <summary>
    /// Guardian relationships linking parents to students.
    /// </summary>
    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();

    /// <summary>
    /// Staff links connecting staff members to students with specific access levels.
    /// </summary>
    public DbSet<StudentStaffLink> StudentStaffLinks => Set<StudentStaffLink>();

    /// <summary>
    /// Audit logs for compliance and security tracking.
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>
    /// Diary entries (communication log) for students.
    /// </summary>
    public DbSet<DiaryEntry> DiaryEntries => Set<DiaryEntry>();

    /// <summary>
    /// File attachments for diary entries.
    /// </summary>
    public DbSet<DiaryAttachment> DiaryAttachments => Set<DiaryAttachment>();

    /// <summary>
    /// Reminders for upcoming events (e.g., control examinations).
    /// Czech: Připomínky na nadcházející události
    /// </summary>
    public DbSet<Reminder> Reminders => Set<Reminder>();

    /// <summary>
    /// Pedagogical support plans (PLPP) for students.
    /// Czech: Plány pedagogické podpory
    /// </summary>
    public DbSet<Plpp> Plpps => Set<Plpp>();

    /// <summary>
    /// Goals within pedagogical support plans.
    /// Czech: Cíle PLPP
    /// </summary>
    public DbSet<PlppGoal> PlppGoals => Set<PlppGoal>();

    /// <summary>
    /// Monthly evaluations of pedagogical support plans.
    /// Czech: Měsíční hodnocení PLPP
    /// </summary>
    public DbSet<PlppEvaluation> PlppEvaluations => Set<PlppEvaluation>();

    /// <summary>
    /// Version history of pedagogical support plans.
    /// Czech: Historie verzí PLPP
    /// </summary>
    public DbSet<PlppVersion> PlppVersions => Set<PlppVersion>();

    /// <summary>
    /// Consultation events (meetings, consultations, school events).
    /// Czech: Konzultace a schůzky
    /// </summary>
    public DbSet<ConsultationEvent> ConsultationEvents => Set<ConsultationEvent>();

    /// <summary>
    /// Participants of consultation events.
    /// Czech: Účastníci konzultací
    /// </summary>
    public DbSet<ConsultationParticipant> ConsultationParticipants => Set<ConsultationParticipant>();

    /// <summary>
    /// In-app notifications for users.
    /// Czech: Notifikace v aplikaci
    /// </summary>
    public DbSet<Notification> Notifications => Set<Notification>();

    /// <summary>
    /// GDPR user consent records.
    /// Czech: GDPR souhlasy uživatelů
    /// </summary>
    public DbSet<UserConsent> UserConsents => Set<UserConsent>();

    /// <summary>
    /// Saves changes and automatically populates audit fields.
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Saves changes asynchronously and automatically populates audit fields.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Configures entity mappings and relationships.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Automatically sets CreatedAt, CreatedBy, ModifiedAt, ModifiedBy
    /// for entities implementing IAuditableEntity.
    /// </summary>
    private void UpdateAuditFields()
    {
        var now = DateTime.UtcNow;
        var userId = _currentUserService?.UserId;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = userId;
                    break;
            }
        }
    }
}
