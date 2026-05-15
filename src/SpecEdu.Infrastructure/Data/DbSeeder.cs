using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpecEdu.Domain.Constants;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Data;

/// <summary>
/// Seeds the database with initial data.
/// Includes roles, admin user, and test data for development.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seeds the database with required initial data.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await SeedRolesAsync(roleManager, logger);

        await SeedAdminUserAsync(userManager, configuration, logger);

        if (environment.IsDevelopment())
        {
            await SeedTestDataAsync(dbContext, userManager, logger);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role: {RoleName}", roleName);
                }
                else
                {
                    logger.LogError("Failed to create role {RoleName}: {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@specedu.cz";
        var adminPassword = configuration["AdminUser:Password"] ?? "Admin123!";
        var adminFirstName = configuration["AdminUser:FirstName"] ?? "Systém";
        var adminLastName = configuration["AdminUser:LastName"] ?? "Administrátor";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin != null)
        {
            logger.LogInformation("Admin user already exists: {Email}", adminEmail);
            return;
        }

        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = adminFirstName,
            LastName = adminLastName,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            SchoolId = null
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            logger.LogInformation("Created admin user: {Email}", adminEmail);
        }
        else
        {
            logger.LogError("Failed to create admin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedTestDataAsync(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        School? testSchool = null;

        if (!await dbContext.Schools.AnyAsync())
        {
            testSchool = new School
            {
                Name = "Testovací ZŠ",
                InstitutionType = "Škola",
                City = "Zlín",
                Address = "Testovací 123",
                PostalCode = "760 01",
                ContactEmail = "test@testskola.cz",
                IsActive = true
            };

            dbContext.Schools.Add(testSchool);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created test school: {SchoolName}", testSchool.Name);
        }
        else
        {
            testSchool = await dbContext.Schools.FirstAsync();
        }

        // Create School Admin
        var schoolAdminEmail = "spravce@testskola.cz";
        var existingSchoolAdmin = await userManager.FindByEmailAsync(schoolAdminEmail);

        if (existingSchoolAdmin == null)
        {
            var newSchoolAdmin = new ApplicationUser
            {
                UserName = schoolAdminEmail,
                Email = schoolAdminEmail,
                FirstName = "Jan",
                LastName = "Správce",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SchoolId = testSchool.Id
            };

            var result = await userManager.CreateAsync(newSchoolAdmin, "Spravce123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newSchoolAdmin, Roles.SchoolAdmin);
                logger.LogInformation("Created test school admin: {Email}", schoolAdminEmail);
            }
        }

        // Create Teacher
        var teacherEmail = "ucitel@testskola.cz";
        var teacher = await userManager.FindByEmailAsync(teacherEmail);

        if (teacher == null)
        {
            teacher = new ApplicationUser
            {
                UserName = teacherEmail,
                Email = teacherEmail,
                FirstName = "Marie",
                LastName = "Učitelová",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SchoolId = testSchool.Id
            };

            var result = await userManager.CreateAsync(teacher, "Ucitel123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(teacher, Roles.Teacher);
                logger.LogInformation("Created test teacher: {Email}", teacherEmail);
            }
        }

        // Create Parent
        var parentEmail = "rodic@testskola.cz";
        var parent = await userManager.FindByEmailAsync(parentEmail);

        if (parent == null)
        {
            parent = new ApplicationUser
            {
                UserName = parentEmail,
                Email = parentEmail,
                FirstName = "Karel",
                LastName = "Novák",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SchoolId = null // Parents are not tied to a school directly
            };

            var result = await userManager.CreateAsync(parent, "Rodic123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(parent, Roles.Parent);
                logger.LogInformation("Created test parent: {Email}", parentEmail);
            }
        }

        // Create Test Students
        if (!await dbContext.Students.AnyAsync())
        {
            var student1 = new Student
            {
                SchoolId = testSchool.Id,
                FirstName = "Jan",
                LastName = "Novák",
                BirthDate = new DateTime(2015, 3, 15),
                Class = "5.A",
                IsActive = true
            };

            var student2 = new Student
            {
                SchoolId = testSchool.Id,
                FirstName = "Marie",
                LastName = "Nováková",
                BirthDate = new DateTime(2017, 7, 22),
                Class = "3.B",
                IsActive = true
            };

            var student3 = new Student
            {
                SchoolId = testSchool.Id,
                FirstName = "Petr",
                LastName = "Svoboda",
                BirthDate = new DateTime(2016, 11, 8),
                Class = "4.A",
                IsActive = true
            };

            dbContext.Students.AddRange(student1, student2, student3);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created test students: Jan Novák, Marie Nováková, Petr Svoboda");

            // Create Guardian relationships (parent -> students)
            parent = await userManager.FindByEmailAsync(parentEmail);
            if (parent != null)
            {
                var guardian1 = new StudentGuardian
                {
                    StudentId = student1.Id,
                    ParentUserId = parent.Id,
                    RelationshipType = RelationshipType.Father,
                    IsActive = true
                };

                var guardian2 = new StudentGuardian
                {
                    StudentId = student2.Id,
                    ParentUserId = parent.Id,
                    RelationshipType = RelationshipType.Father,
                    IsActive = true
                };

                dbContext.StudentGuardians.AddRange(guardian1, guardian2);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Created guardian relationships for parent {Email}", parentEmail);
            }

            // Create Staff links (teacher -> students)
            teacher = await userManager.FindByEmailAsync(teacherEmail);
            if (teacher != null)
            {
                var staffLink1 = new StudentStaffLink
                {
                    StudentId = student1.Id,
                    UserId = teacher.Id,
                    LinkType = StaffLinkType.Teacher,
                    AccessLevel = AccessLevel.Edit,
                    IsActive = true
                };

                var staffLink2 = new StudentStaffLink
                {
                    StudentId = student2.Id,
                    UserId = teacher.Id,
                    LinkType = StaffLinkType.Teacher,
                    AccessLevel = AccessLevel.Edit,
                    IsActive = true
                };

                var staffLink3 = new StudentStaffLink
                {
                    StudentId = student3.Id,
                    UserId = teacher.Id,
                    LinkType = StaffLinkType.Teacher,
                    AccessLevel = AccessLevel.Read,
                    IsActive = true
                };

                dbContext.StudentStaffLinks.AddRange(staffLink1, staffLink2, staffLink3);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Created staff links for teacher {Email}", teacherEmail);
            }
        }

        // ---- Fetch references for additional seed data ----
        var schoolAdmin = await userManager.FindByEmailAsync(schoolAdminEmail);
        teacher = await userManager.FindByEmailAsync(teacherEmail);
        parent = await userManager.FindByEmailAsync(parentEmail);

        var allStudents = await dbContext.Students.ToListAsync();
        if (schoolAdmin == null || teacher == null || parent == null || allStudents.Count < 3)
        {
            logger.LogWarning("Cannot seed additional test data: users or students missing");
            return;
        }

        var jan = allStudents.First(s => s.FirstName == "Jan" && s.LastName == "Novák");
        var marie = allStudents.First(s => s.FirstName == "Marie" && s.LastName == "Nováková");
        var petr = allStudents.First(s => s.FirstName == "Petr" && s.LastName == "Svoboda");
        var now = DateTime.UtcNow;

        // ======================================================================
        // Diary Entries (6)
        // ======================================================================
        if (!await dbContext.DiaryEntries.AnyAsync())
        {
            dbContext.DiaryEntries.AddRange(
                new DiaryEntry
                {
                    StudentId = jan.Id,
                    Type = DiaryEntryType.Note,
                    Title = "Pozorování v hodině matematiky",
                    Content = "Jan dnes výrazně lépe pracoval s číselnou osou. Zvládl sčítání do 100 bez pomůcek.",
                    Visibility = DiaryVisibility.SchoolOnly,
                    OccurredAt = now.AddDays(-10),
                    IsActive = true
                },
                new DiaryEntry
                {
                    StudentId = jan.Id,
                    Type = DiaryEntryType.PhoneCall,
                    Title = "Telefonát s otcem – domácí příprava",
                    Content = "Domluvena pravidelná 15minutová domácí příprava čtení. Otec souhlasí s denním cvičením.",
                    Visibility = DiaryVisibility.ParentVisible,
                    OccurredAt = now.AddDays(-7),
                    IsActive = true
                },
                new DiaryEntry
                {
                    StudentId = jan.Id,
                    Type = DiaryEntryType.Meeting,
                    Title = "Schůzka s rodiči – vyhodnocení PLPP",
                    Content = "Proběhla schůzka k vyhodnocení plánu pedagogické podpory. Rodiče seznámeni s pokrokem.",
                    Visibility = DiaryVisibility.ParentVisible,
                    OccurredAt = now.AddDays(-3),
                    IsActive = true
                },
                new DiaryEntry
                {
                    StudentId = marie.Id,
                    Type = DiaryEntryType.ParentCollaboration,
                    Title = "Spolupráce s rodiči – pomůcky pro čtení",
                    Content = "Rodiče dodali speciální záložku a barevné fólie pro usnadnění čtení. Marie je používá od pondělí.",
                    Visibility = DiaryVisibility.ParentVisible,
                    OccurredAt = now.AddDays(-5),
                    IsActive = true
                },
                new DiaryEntry
                {
                    StudentId = marie.Id,
                    Type = DiaryEntryType.Note,
                    Title = "Pokrok ve čtení",
                    Content = "Marie přečetla celý odstavec bez chyby. Tempo čtení se zlepšilo o 20 %.",
                    Visibility = DiaryVisibility.SchoolOnly,
                    OccurredAt = now.AddDays(-2),
                    IsActive = true
                },
                new DiaryEntry
                {
                    StudentId = marie.Id,
                    Type = DiaryEntryType.PppSpcCollaboration,
                    Title = "Konzultace s PPP Zlín",
                    Content = "Telefonická konzultace s Mgr. Horákovou z PPP ohledně dalšího postupu. Doporučeno kontrolní vyšetření za 6 měsíců.",
                    Visibility = DiaryVisibility.SchoolOnly,
                    OccurredAt = now.AddDays(-1),
                    IsActive = true
                }
            );
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded 6 diary entries");
        }

        // ======================================================================
        // PLPPs (2) + Goals + Evaluations + Version
        // ======================================================================
        if (!await dbContext.Plpps.AnyAsync())
        {
            // PLPP 1: Jan – Active, 3 goals, 2 evaluations, 1 version
            var plpp1 = new Plpp
            {
                StudentId = jan.Id,
                SchoolYear = "2025/2026",
                Status = PlppStatus.Active,
                SupportLevel = SupportMeasureLevel.Level2,
                ValidFrom = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidTo = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                Strengths = "Jan je komunikativní, rád spolupracuje se spolužáky. Má dobrou prostorovou orientaci a logické myšlení.",
                AreasNeedingSupport = "Čtení – pomalé tempo, záměna písmen b/d. Psaní – obtíže s grafomotorikou.",
                RecommendedMethods = "Multisenzorický přístup ke čtení, využití barevných karet, čtení s okénkem.",
                OrganizationalAdjustments = "Posazení v přední lavici, delší čas na písemné práce (navýšení o 25 %).",
                ContentAdjustments = "Zkrácené diktáty, možnost ústního zkoušení místo písemného.",
                AssessmentMethods = "Slovní hodnocení v ČJ, kombinované hodnocení v ostatních předmětech.",
                ParentCollaboration = "Pravidelné konzultace 1× měsíčně, denní 15min domácí čtení.",
                InternalNotes = "Doporučeno kontrolní vyšetření v PPP do konce školního roku.",
                IsVisibleToParents = true,
                ActivatedAt = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                ActivatedBy = teacher.Id,
                IsActive = true
            };

            dbContext.Plpps.Add(plpp1);
            await dbContext.SaveChangesAsync();

            // Goals for PLPP 1
            dbContext.PlppGoals.AddRange(
                new PlppGoal
                {
                    PlppId = plpp1.Id,
                    Order = 1,
                    Subject = "Český jazyk",
                    Description = "Zvýšit tempo čtení na 60 slov/min při zachování porozumění textu.",
                    SuccessCriteria = "Jan přečte 60 slov za minutu s porozuměním alespoň 70 %.",
                    Methods = "Denní čtení s okénkem, párové čtení, opakované čtení krátkých textů.",
                    ResponsiblePerson = "Mgr. Učitelová",
                    TargetDate = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                    Status = GoalStatus.InProgress,
                    ProgressNotes = "Aktuální tempo ~45 slov/min (nárůst z 35). Pokrok je patrný.",
                    IsActive = true
                },
                new PlppGoal
                {
                    PlppId = plpp1.Id,
                    Order = 2,
                    Subject = "Český jazyk",
                    Description = "Snížit počet záměn b/d při psaní na méně než 3 chyby v diktátu.",
                    SuccessCriteria = "V diktátu o 30 slovech maximálně 3 záměny b/d.",
                    Methods = "Vizuální pomůcky (obrázky \u201Ebříško/dříček\u201C), kinetická cvičení.",
                    ResponsiblePerson = "Mgr. Učitelová",
                    TargetDate = new DateTime(2026, 3, 31, 0, 0, 0, DateTimeKind.Utc),
                    Status = GoalStatus.InProgress,
                    ProgressNotes = "Aktuálně průměrně 5 záměn. Používání pomůcek pomáhá.",
                    IsActive = true
                },
                new PlppGoal
                {
                    PlppId = plpp1.Id,
                    Order = 3,
                    Subject = "Matematika",
                    Description = "Osvojit si sčítání a odčítání do 100 bez pomůcek.",
                    SuccessCriteria = "80 % úspěšnost v testech sčítání/odčítání do 100.",
                    Methods = "Manipulační činnosti, počítadlo, postupný přechod k pamětnímu počítání.",
                    ResponsiblePerson = "Mgr. Učitelová",
                    TargetDate = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                    Status = GoalStatus.NotStarted,
                    IsActive = true
                }
            );

            // Evaluations for PLPP 1
            dbContext.PlppEvaluations.AddRange(
                new PlppEvaluation
                {
                    PlppId = plpp1.Id,
                    EvaluationMonth = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                    WhatStudentManages = "Jan dobře spolupracuje, začal používat čtecí okénko. Tempo čtení se zlepšuje.",
                    WhatNeedsImprovement = "Stále časté záměny b/d, grafomotorika – neuvolněné zápěstí.",
                    RecommendedAdjustments = "Přidat grafomotorická cvičení na začátek hodin psaní.",
                    ParentConsultationNotes = "Rodiče potvrzují pravidelné domácí čtení. Otec pomáhá s přípravou.",
                    ProgressRating = 3,
                    ParentsNotified = true,
                    ParentsNotifiedAt = new DateTime(2025, 11, 5, 10, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                },
                new PlppEvaluation
                {
                    PlppId = plpp1.Id,
                    EvaluationMonth = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    WhatStudentManages = "Tempo čtení vzrostlo na 45 slov/min. Jan je motivovanější, sám si vybírá knihy.",
                    WhatNeedsImprovement = "Záměny b/d přetrvávají, ale méně často. Psaní stále pomalé.",
                    RecommendedAdjustments = "Pokračovat v nastaveném režimu. Zvážit tablet pro psaní delších textů.",
                    ProgressRating = 4,
                    ParentsNotified = true,
                    ParentsNotifiedAt = new DateTime(2025, 12, 3, 14, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                }
            );

            // Version snapshot for PLPP 1
            dbContext.PlppVersions.Add(new PlppVersion
            {
                PlppId = plpp1.Id,
                VersionNumber = 1,
                Snapshot = "{}",
                ChangeSummary = "Počáteční verze plánu při aktivaci.",
                Source = VersionSource.Activation
            });

            await dbContext.SaveChangesAsync();

            // PLPP 2: Marie – Draft, 2 goals
            var plpp2 = new Plpp
            {
                StudentId = marie.Id,
                SchoolYear = "2025/2026",
                Status = PlppStatus.Draft,
                SupportLevel = SupportMeasureLevel.Level2,
                ValidFrom = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidTo = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                Strengths = "Marie je pečlivá a snaživá. Má výbornou paměť na básničky a písničky.",
                AreasNeedingSupport = "Čtení – obtíže s dekódováním delších slov. Matematika – záměna číslic 6/9.",
                RecommendedMethods = "Analyticko-syntetická metoda, genetická metoda jako podpora.",
                IsVisibleToParents = false,
                IsActive = true
            };

            dbContext.Plpps.Add(plpp2);
            await dbContext.SaveChangesAsync();

            dbContext.PlppGoals.AddRange(
                new PlppGoal
                {
                    PlppId = plpp2.Id,
                    Order = 1,
                    Subject = "Český jazyk",
                    Description = "Správně dekódovat trojslabičná a čtyřslabičná slova.",
                    SuccessCriteria = "Přečte 8 z 10 víceslabičných slov správně na první pokus.",
                    Methods = "Slabikování, rytmické čtení, postupné skládání slov ze slabik.",
                    ResponsiblePerson = "Mgr. Učitelová",
                    TargetDate = new DateTime(2026, 4, 30, 0, 0, 0, DateTimeKind.Utc),
                    Status = GoalStatus.NotStarted,
                    IsActive = true
                },
                new PlppGoal
                {
                    PlppId = plpp2.Id,
                    Order = 2,
                    Subject = "Matematika",
                    Description = "Odstranit záměnu číslic 6 a 9 při psaní.",
                    SuccessCriteria = "Žádná záměna 6/9 ve třech po sobě jdoucích pracovních listech.",
                    Methods = "Vizuální pomůcky, modelování číslic z modelíny, dotykové číslice.",
                    ResponsiblePerson = "Mgr. Učitelová",
                    TargetDate = new DateTime(2026, 3, 31, 0, 0, 0, DateTimeKind.Utc),
                    Status = GoalStatus.NotStarted,
                    IsActive = true
                }
            );
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded 2 PLPPs with goals, evaluations, and version");
        }

        // ======================================================================
        // Consultation Events (8) + Participants
        // ======================================================================
        if (!await dbContext.ConsultationEvents.AnyAsync())
        {
            var events = new List<ConsultationEvent>
            {
                // 1. Past – Completed parent-teacher meeting
                new()
                {
                    SchoolId = testSchool.Id,
                    Title = "Třídní schůzka 5.A",
                    Description = "Pravidelná třídní schůzka – prospěch, chování, plánované akce.",
                    Type = ConsultationType.ParentTeacherMeeting,
                    Status = ConsultationEventStatus.Completed,
                    StartTime = now.AddDays(-21).Date.AddHours(16),
                    EndTime = now.AddDays(-21).Date.AddHours(17),
                    Location = "Učebna 5.A",
                    IsVisibleToParents = true,
                    OrganizerId = teacher.Id,
                    Notes = "Projednán prospěch za 1. čtvrtletí. Rodiče informováni o plánovaném výletě.",
                    ReminderSent = true,
                    IsActive = true
                },
                // 2. Past – Completed individual consultation
                new()
                {
                    SchoolId = testSchool.Id,
                    StudentId = jan.Id,
                    Title = "Konzultace k PLPP – Jan Novák",
                    Description = "Individuální konzultace s rodiči o pokroku Jana v rámci PLPP.",
                    Type = ConsultationType.IndividualConsultation,
                    Status = ConsultationEventStatus.Completed,
                    StartTime = now.AddDays(-14).Date.AddHours(15),
                    EndTime = now.AddDays(-14).Date.AddHours(15).AddMinutes(30),
                    Location = "Kabinet ČJ",
                    IsVisibleToParents = true,
                    OrganizerId = teacher.Id,
                    Notes = "Rodiče seznámeni s pokroky ve čtení. Domluvena intenzivnější domácí příprava.",
                    ReminderSent = true,
                    IsActive = true
                },
                // 3. Past – Completed PLPP review
                new()
                {
                    SchoolId = testSchool.Id,
                    StudentId = jan.Id,
                    Title = "Vyhodnocení PLPP – Jan Novák",
                    Description = "Pololetní vyhodnocení plánu pedagogické podpory.",
                    Type = ConsultationType.PlppReview,
                    Status = ConsultationEventStatus.Completed,
                    StartTime = now.AddDays(-10).Date.AddHours(14),
                    EndTime = now.AddDays(-10).Date.AddHours(15),
                    Location = "Sborovna",
                    IsVisibleToParents = true,
                    OrganizerId = schoolAdmin.Id,
                    Notes = "PLPP bude pokračovat beze změn. Cíle jsou realistické, pokrok patrný.",
                    ReminderSent = true,
                    IsActive = true
                },
                // 4. Past – Cancelled counselor meeting
                new()
                {
                    SchoolId = testSchool.Id,
                    StudentId = marie.Id,
                    Title = "Schůzka se školním psychologem – Marie",
                    Description = "Konzultace ohledně obtíží ve čtení.",
                    Type = ConsultationType.CounselorMeeting,
                    Status = ConsultationEventStatus.Cancelled,
                    StartTime = now.AddDays(-5).Date.AddHours(10),
                    EndTime = now.AddDays(-5).Date.AddHours(10).AddMinutes(45),
                    Location = "Pracovna ŠPP",
                    IsVisibleToParents = false,
                    OrganizerId = teacher.Id,
                    Notes = "Zrušeno z důvodu nemoci psychologa. Bude přesunuto.",
                    ReminderSent = true,
                    IsActive = true
                },
                // 5. Tomorrow – Scheduled individual consultation
                new()
                {
                    SchoolId = testSchool.Id,
                    StudentId = petr.Id,
                    Title = "Konzultace – Petr Svoboda",
                    Description = "Schůzka s rodiči ohledně chování a zapojení do výuky.",
                    Type = ConsultationType.IndividualConsultation,
                    Status = ConsultationEventStatus.Scheduled,
                    StartTime = now.AddDays(1).Date.AddHours(16),
                    EndTime = now.AddDays(1).Date.AddHours(16).AddMinutes(30),
                    Location = "Kabinet ČJ",
                    IsVisibleToParents = true,
                    OrganizerId = teacher.Id,
                    IsActive = true
                },
                // 6. +5 days – Scheduled parent-teacher meeting
                new()
                {
                    SchoolId = testSchool.Id,
                    Title = "Třídní schůzka 3.B",
                    Description = "Pravidelná třídní schůzka – prospěch, jarní výlet.",
                    Type = ConsultationType.ParentTeacherMeeting,
                    Status = ConsultationEventStatus.Scheduled,
                    StartTime = now.AddDays(5).Date.AddHours(17),
                    EndTime = now.AddDays(5).Date.AddHours(18),
                    Location = "Učebna 3.B",
                    IsVisibleToParents = true,
                    OrganizerId = teacher.Id,
                    IsActive = true
                },
                // 7. +10 days – Confirmed PLPP review
                new()
                {
                    SchoolId = testSchool.Id,
                    StudentId = marie.Id,
                    Title = "Příprava PLPP – Marie Nováková",
                    Description = "Schůzka k finalizaci PLPP před aktivací.",
                    Type = ConsultationType.PlppReview,
                    Status = ConsultationEventStatus.Confirmed,
                    StartTime = now.AddDays(10).Date.AddHours(14),
                    EndTime = now.AddDays(10).Date.AddHours(15),
                    Location = "Sborovna",
                    IsVisibleToParents = true,
                    OrganizerId = schoolAdmin.Id,
                    IsActive = true
                },
                // 8. +14 days – School event
                new()
                {
                    SchoolId = testSchool.Id,
                    Title = "Den otevřených dveří",
                    Description = "Škola otevírá dveře budoucím žákům a jejich rodičům.",
                    Type = ConsultationType.SchoolEvent,
                    Status = ConsultationEventStatus.Scheduled,
                    StartTime = now.AddDays(14).Date.AddHours(9),
                    EndTime = now.AddDays(14).Date.AddHours(12),
                    Location = "Celá škola",
                    IsVisibleToParents = true,
                    OrganizerId = schoolAdmin.Id,
                    IsActive = true
                }
            };

            dbContext.ConsultationEvents.AddRange(events);
            await dbContext.SaveChangesAsync();

            // Add participants to events
            var participants = new List<ConsultationParticipant>();

            // Event 0 (třídní schůzka): teacher (organizer) + parent
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[0].Id, UserId = teacher.Id, RoleDescription = "Třídní učitelka", IsOrganizer = true, ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-25), Attended = true });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[0].Id, UserId = parent.Id, RoleDescription = "Otec", ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-23), Attended = true });

            // Event 1 (individual – Jan): teacher + parent
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[1].Id, UserId = teacher.Id, RoleDescription = "Třídní učitelka", IsOrganizer = true, ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-18), Attended = true });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[1].Id, UserId = parent.Id, RoleDescription = "Otec", ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-16), Attended = true });

            // Event 2 (PLPP review – Jan): admin + teacher + parent
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[2].Id, UserId = schoolAdmin.Id, RoleDescription = "Správce školy", IsOrganizer = true, ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-15), Attended = true });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[2].Id, UserId = teacher.Id, RoleDescription = "Třídní učitelka", ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-14), Attended = true });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[2].Id, UserId = parent.Id, RoleDescription = "Otec", ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-13), Attended = true });

            // Event 3 (cancelled counselor): teacher + external
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[3].Id, UserId = teacher.Id, RoleDescription = "Třídní učitelka", IsOrganizer = true, ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-8) });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[3].Id, ExternalName = "PhDr. Jana Kolářová", ExternalEmail = "kolarova@ppp-zlin.cz", RoleDescription = "Školní psycholog", ResponseStatus = ParticipantResponseStatus.Declined, RespondedAt = now.AddDays(-6), ResponseNote = "Omlouvám se, nemoc." });

            // Event 4 (tomorrow – Petr): teacher + parent
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[4].Id, UserId = teacher.Id, RoleDescription = "Třídní učitelka", IsOrganizer = true, ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-3) });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[4].Id, UserId = parent.Id, RoleDescription = "Otec", ResponseStatus = ParticipantResponseStatus.Tentative, RespondedAt = now.AddDays(-1) });

            // Event 5 (třídní schůzka 3.B): teacher + parent
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[5].Id, UserId = teacher.Id, RoleDescription = "Třídní učitelka", IsOrganizer = true, ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-2) });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[5].Id, UserId = parent.Id, RoleDescription = "Otec", ResponseStatus = ParticipantResponseStatus.Pending });

            // Event 6 (PLPP review – Marie): admin + teacher
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[6].Id, UserId = schoolAdmin.Id, RoleDescription = "Správce školy", IsOrganizer = true, ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-5) });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[6].Id, UserId = teacher.Id, RoleDescription = "Třídní učitelka", ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-4) });

            // Event 7 (open day): admin + teacher
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[7].Id, UserId = schoolAdmin.Id, RoleDescription = "Správce školy", IsOrganizer = true, ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-7) });
            participants.Add(new ConsultationParticipant { ConsultationEventId = events[7].Id, UserId = teacher.Id, RoleDescription = "Třídní učitelka", ResponseStatus = ParticipantResponseStatus.Accepted, RespondedAt = now.AddDays(-6) });

            dbContext.ConsultationParticipants.AddRange(participants);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded 8 consultation events with participants");
        }

        // ======================================================================
        // Notifications (12)
        // ======================================================================
        if (!await dbContext.Notifications.AnyAsync())
        {
            dbContext.Notifications.AddRange(
                // Teacher notifications (4)
                new Notification { UserId = teacher.Id, Title = "Nový žák přiřazen", Message = "Žák Petr Svoboda byl přiřazen do vaší třídy.", Type = NotificationType.Info, IsRead = true, ReadAt = now.AddDays(-8), Link = "/SchoolAdmin/Students" },
                new Notification { UserId = teacher.Id, Title = "Připomínka: vyhodnocení PLPP", Message = "Za týden je potřeba provést měsíční vyhodnocení PLPP pro Jana Nováka.", Type = NotificationType.ReminderDue, IsRead = false, Link = "/SchoolAdmin/Plpp" },
                new Notification { UserId = teacher.Id, Title = "Konzultace zítra", Message = "Zítra v 16:00 máte naplánovanou konzultaci s rodiči Petra Svobody.", Type = NotificationType.ConsultationInvite, IsRead = false, Link = "/SchoolAdmin/Calendar" },
                new Notification { UserId = teacher.Id, Title = "Nová zpráva od rodiče", Message = "Karel Novák vám poslal novou zprávu.", Type = NotificationType.Info, IsRead = false, Link = "/Chat" },

                // Parent notifications (4)
                new Notification { UserId = parent.Id, Title = "PLPP aktualizováno", Message = "Plán pedagogické podpory pro Jana Nováka byl aktualizován.", Type = NotificationType.PlppUpdate, IsRead = false, Link = "/MyChildren/Plpp" },
                new Notification { UserId = parent.Id, Title = "Nový záznam v deníku", Message = "Učitelka přidala nový záznam do komunikačního deníku Marie.", Type = NotificationType.DiaryEntry, IsRead = false, Link = "/MyChildren/Diary" },
                new Notification { UserId = parent.Id, Title = "Pozvánka na třídní schůzku", Message = "Jste zváni na třídní schůzku 3.B dne " + now.AddDays(5).ToString("d. M. yyyy") + ".", Type = NotificationType.ConsultationInvite, IsRead = false, Link = "/MyChildren/Calendar" },
                new Notification { UserId = parent.Id, Title = "Systémové oznámení", Message = "Aplikace SpecEdu byla aktualizována na novou verzi s vylepšeným kalendářem.", Type = NotificationType.SystemAnnouncement, IsRead = true, ReadAt = now.AddDays(-2) },

                // SchoolAdmin notifications (4)
                new Notification { UserId = schoolAdmin.Id, Title = "Nový uživatel registrován", Message = "Nový rodič se zaregistroval v systému. Čeká na schválení.", Type = NotificationType.Info, IsRead = true, ReadAt = now.AddDays(-12), Link = "/SchoolAdmin/Users" },
                new Notification { UserId = schoolAdmin.Id, Title = "PLPP čeká na aktivaci", Message = "PLPP pro Marii Novákovou je připraveno k aktivaci.", Type = NotificationType.PlppUpdate, IsRead = false, Link = "/SchoolAdmin/Plpp" },
                new Notification { UserId = schoolAdmin.Id, Title = "Den otevřených dveří", Message = "Za 2 týdny se koná den otevřených dveří. Zkontrolujte připravenost.", Type = NotificationType.Info, IsRead = false, Link = "/SchoolAdmin/Calendar" },
                new Notification { UserId = schoolAdmin.Id, Title = "Integrace PPP", Message = "Test připojení k PPP Zlín proběhl úspěšně.", Type = NotificationType.SystemAnnouncement, IsRead = true, ReadAt = now.AddDays(-5), Link = "/Admin/Integrations" }
            );
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded 12 notifications");
        }

        // ======================================================================
        // Conversations (3) + Messages (~15)
        // ======================================================================
        if (!await dbContext.Conversations.AnyAsync())
        {
            // Conversation 1: Teacher ↔ Parent (1:1)
            var conv1 = new Conversation
            {
                Title = null,
                IsGroup = false,
                LastMessageAt = now.AddHours(-2),
                LastMessagePreview = "Děkuji za informaci, paní učitelko.",
                IsActive = true
            };

            // Conversation 2: Admin ↔ Teacher (1:1)
            var conv2 = new Conversation
            {
                Title = null,
                IsGroup = false,
                LastMessageAt = now.AddDays(-1),
                LastMessagePreview = "Dobře, připravím podklady.",
                IsActive = true
            };

            // Conversation 3: Group – all 3
            var conv3 = new Conversation
            {
                Title = "PLPP Jan Novák – koordinace",
                IsGroup = true,
                LastMessageAt = now.AddDays(-3),
                LastMessagePreview = "Domluveno, uvidíme se na schůzce.",
                IsActive = true
            };

            dbContext.Conversations.AddRange(conv1, conv2, conv3);
            await dbContext.SaveChangesAsync();

            // Participants
            dbContext.ConversationParticipants.AddRange(
                // Conv 1: Teacher + Parent
                new ConversationParticipant { ConversationId = conv1.Id, UserId = teacher.Id, JoinedAt = now.AddDays(-7), LastReadAt = now.AddHours(-2) },
                new ConversationParticipant { ConversationId = conv1.Id, UserId = parent.Id, JoinedAt = now.AddDays(-7), LastReadAt = now.AddHours(-2) },

                // Conv 2: Admin + Teacher
                new ConversationParticipant { ConversationId = conv2.Id, UserId = schoolAdmin.Id, JoinedAt = now.AddDays(-5), LastReadAt = now.AddDays(-1) },
                new ConversationParticipant { ConversationId = conv2.Id, UserId = teacher.Id, JoinedAt = now.AddDays(-5), LastReadAt = now.AddDays(-1) },

                // Conv 3: All three
                new ConversationParticipant { ConversationId = conv3.Id, UserId = schoolAdmin.Id, JoinedAt = now.AddDays(-10), LastReadAt = now.AddDays(-3) },
                new ConversationParticipant { ConversationId = conv3.Id, UserId = teacher.Id, JoinedAt = now.AddDays(-10), LastReadAt = now.AddDays(-3) },
                new ConversationParticipant { ConversationId = conv3.Id, UserId = parent.Id, JoinedAt = now.AddDays(-10), LastReadAt = now.AddDays(-4) }
            );

            // Messages – Conv 1 (Teacher ↔ Parent, 5 messages)
            dbContext.ChatMessages.AddRange(
                new ChatMessage { ConversationId = conv1.Id, SenderId = teacher.Id, Content = "Dobrý den, pane Nováku. Chtěla bych vás informovat, že Jan dnes výborně pracoval v hodině čtení.", SentAt = now.AddDays(-7).AddHours(15) },
                new ChatMessage { ConversationId = conv1.Id, SenderId = parent.Id, Content = "Dobrý den, to mě těší! Doma pravidelně čteme, jak jsme se domluvili.", SentAt = now.AddDays(-7).AddHours(16) },
                new ChatMessage { ConversationId = conv1.Id, SenderId = teacher.Id, Content = "Je to vidět, pokrok je znatelný. Chtěla bych se s vámi sejít na krátké konzultaci příští týden.", SentAt = now.AddDays(-6).AddHours(9) },
                new ChatMessage { ConversationId = conv1.Id, SenderId = parent.Id, Content = "Samozřejmě, můžeme se domluvit na úterý nebo středu odpoledne?", SentAt = now.AddDays(-6).AddHours(12) },
                new ChatMessage { ConversationId = conv1.Id, SenderId = parent.Id, Content = "Děkuji za informaci, paní učitelko.", SentAt = now.AddHours(-2) }
            );

            // Messages – Conv 2 (Admin ↔ Teacher, 5 messages)
            dbContext.ChatMessages.AddRange(
                new ChatMessage { ConversationId = conv2.Id, SenderId = schoolAdmin.Id, Content = "Dobrý den, Marie. Potřeboval bych podklady k PLPP Marie Novákové pro schůzku s PPP.", SentAt = now.AddDays(-5).AddHours(8) },
                new ChatMessage { ConversationId = conv2.Id, SenderId = teacher.Id, Content = "Dobrý den. Mám připravený návrh, pošlu vám ho do pátku.", SentAt = now.AddDays(-5).AddHours(10) },
                new ChatMessage { ConversationId = conv2.Id, SenderId = schoolAdmin.Id, Content = "Výborně. Nezapomeňte prosím i na diagnostické podklady z PPP.", SentAt = now.AddDays(-4).AddHours(9) },
                new ChatMessage { ConversationId = conv2.Id, SenderId = teacher.Id, Content = "Ano, mám je založené. Přiložím je k návrhu PLPP.", SentAt = now.AddDays(-3).AddHours(14) },
                new ChatMessage { ConversationId = conv2.Id, SenderId = teacher.Id, Content = "Dobře, připravím podklady.", SentAt = now.AddDays(-1).AddHours(9) }
            );

            // Messages – Conv 3 (Group, 5 messages)
            dbContext.ChatMessages.AddRange(
                new ChatMessage { ConversationId = conv3.Id, SenderId = schoolAdmin.Id, Content = "Dobrý den všem. Rád bych koordinoval schůzku k vyhodnocení PLPP Jana.", SentAt = now.AddDays(-10).AddHours(9) },
                new ChatMessage { ConversationId = conv3.Id, SenderId = teacher.Id, Content = "Dobrý den. Navrhuji příští středu ve 14:00 ve sborovně.", SentAt = now.AddDays(-10).AddHours(11) },
                new ChatMessage { ConversationId = conv3.Id, SenderId = parent.Id, Content = "Dobrý den. Středa mi vyhovuje, budu tam.", SentAt = now.AddDays(-9).AddHours(18) },
                new ChatMessage { ConversationId = conv3.Id, SenderId = schoolAdmin.Id, Content = "Výborně, zapíšu to do kalendáře. Prosím, připravte si poznámky k pokroku Jana.", SentAt = now.AddDays(-9).AddHours(20) },
                new ChatMessage { ConversationId = conv3.Id, SenderId = parent.Id, Content = "Domluveno, uvidíme se na schůzce.", SentAt = now.AddDays(-3).AddHours(7) }
            );

            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded 3 conversations with 15 messages");
        }

        // ======================================================================
        // Reminders (3)
        // ======================================================================
        if (!await dbContext.Reminders.AnyAsync())
        {
            dbContext.Reminders.AddRange(
                // 1. Pending – future (due in 2 months, notify in 2 weeks)
                new Reminder
                {
                    StudentId = jan.Id,
                    Title = "Kontrolní vyšetření v PPP",
                    Description = "Doporučeno kontrolní vyšetření v PPP Zlín pro přehodnocení podpůrných opatření.",
                    DueDate = now.AddMonths(2),
                    NotifyAt = now.AddDays(14),
                    Channel = NotificationChannel.Email,
                    Status = ReminderStatus.Pending,
                    IsActive = true
                },
                // 2. Pending – tomorrow
                new Reminder
                {
                    StudentId = marie.Id,
                    Title = "Odevzdat návrh PLPP",
                    Description = "Termín pro odevzdání návrhu PLPP Marie Novákové vedení školy.",
                    DueDate = now.AddMonths(3),
                    NotifyAt = now.AddDays(1),
                    Channel = NotificationChannel.InApp,
                    Status = ReminderStatus.Pending,
                    IsActive = true
                },
                // 3. Sent – past
                new Reminder
                {
                    StudentId = petr.Id,
                    Title = "Prodloužení doporučení SPC",
                    Description = "Vyprší platnost doporučení SPC. Je potřeba požádat o prodloužení.",
                    DueDate = now.AddMonths(1),
                    NotifyAt = now.AddDays(-14),
                    Channel = NotificationChannel.Email,
                    Status = ReminderStatus.Sent,
                    SentAt = now.AddDays(-14),
                    IsActive = true
                }
            );
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded 3 reminders");
        }

        // ======================================================================
        // Integration Endpoints (1) + Data Exchange Records (2)
        // ======================================================================
        if (!await dbContext.IntegrationEndpoints.AnyAsync())
        {
            var endpoint = new IntegrationEndpoint
            {
                Name = "PPP Zlín – testovací",
                SystemType = ExternalSystemType.PPP,
                BaseUrl = "https://api.ppp-zlin.cz/v1",
                ApiKeyPlaceholder = "test-api-key-placeholder",
                IsActive = true,
                LastTestedAt = now.AddDays(-5),
                LastTestResult = "OK – připojení úspěšné (testovací režim)"
            };

            dbContext.IntegrationEndpoints.Add(endpoint);
            await dbContext.SaveChangesAsync();

            dbContext.DataExchangeRecords.AddRange(
                new DataExchangeRecord
                {
                    EndpointId = endpoint.Id,
                    Direction = DataExchangeDirection.Export,
                    EntityType = "ConnectionTest",
                    Status = DataExchangeStatus.Completed,
                    RequestSummary = "POST /connection-test",
                    ResponseSummary = "200 OK – Connection successful",
                    CreatedAt = now.AddDays(-5),
                    CompletedAt = now.AddDays(-5).AddSeconds(2),
                    InitiatedBy = schoolAdmin.Id
                },
                new DataExchangeRecord
                {
                    EndpointId = endpoint.Id,
                    Direction = DataExchangeDirection.Export,
                    EntityType = "Plpp",
                    Status = DataExchangeStatus.Failed,
                    RequestSummary = "POST /students/plpp-export",
                    ResponseSummary = "501 Not Implemented – Endpoint not available in test mode",
                    CreatedAt = now.AddDays(-4),
                    CompletedAt = now.AddDays(-4).AddSeconds(1),
                    InitiatedBy = schoolAdmin.Id
                }
            );
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded 1 integration endpoint with 2 exchange records");
        }

        // ======================================================================
        // User Consents (3)
        // ======================================================================
        if (!await dbContext.UserConsents.AnyAsync())
        {
            dbContext.UserConsents.AddRange(
                new UserConsent { UserId = schoolAdmin.Id, ConsentType = "GDPR", IsGranted = true, GrantedAt = now.AddDays(-30), IpAddress = "127.0.0.1" },
                new UserConsent { UserId = teacher.Id, ConsentType = "GDPR", IsGranted = true, GrantedAt = now.AddDays(-28), IpAddress = "127.0.0.1" },
                new UserConsent { UserId = parent.Id, ConsentType = "GDPR", IsGranted = true, GrantedAt = now.AddDays(-25), IpAddress = "127.0.0.1" }
            );
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded 3 user consents");
        }
    }
}
