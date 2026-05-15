using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Domain.Common;
using SpecEdu.Domain.Entities;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Data;

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

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public DbSet<School> Schools => Set<School>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();

    public DbSet<StudentStaffLink> StudentStaffLinks => Set<StudentStaffLink>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<DiaryEntry> DiaryEntries => Set<DiaryEntry>();

    public DbSet<DiaryAttachment> DiaryAttachments => Set<DiaryAttachment>();

    public DbSet<Reminder> Reminders => Set<Reminder>();

    public DbSet<Plpp> Plpps => Set<Plpp>();

    public DbSet<PlppGoal> PlppGoals => Set<PlppGoal>();

    public DbSet<PlppEvaluation> PlppEvaluations => Set<PlppEvaluation>();

    public DbSet<PlppVersion> PlppVersions => Set<PlppVersion>();

    public DbSet<ConsultationEvent> ConsultationEvents => Set<ConsultationEvent>();

    public DbSet<ConsultationParticipant> ConsultationParticipants => Set<ConsultationParticipant>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<UserConsent> UserConsents => Set<UserConsent>();

    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();

    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    public DbSet<ChatAttachment> ChatAttachments => Set<ChatAttachment>();

    public DbSet<IntegrationEndpoint> IntegrationEndpoints => Set<IntegrationEndpoint>();

    public DbSet<DataExchangeRecord> DataExchangeRecords => Set<DataExchangeRecord>();

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

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
