# SpecEdu Development Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## Current Implementation Status (2026-04-16)

High-level overview of what is **done**, **partial**, and **not done**.

### Done
- **Clean Architecture solution**: Domain / Application / Infrastructure / two Web projects (Public + App)
- **Identity & roles**: 6 roles (Admin, SchoolAdmin, Teacher, SPP, Parent, Guest) with permission-based policies
- **Multi-tenant schools**: School entity, `SchoolId` on users, tenant filtering
- **Student management**: Full CRUD (Create / Edit / Details / Index), photo upload, class assignment
- **Access control**: `StudentGuardian` (parent → child) and `StudentStaffLink` (staff → student with Read/Edit level)
- **Communication diary**: 5 entry types, visibility (SchoolOnly / ParentVisible), attachments, filtering
- **PLPP editor**: Draft → Active → Archived workflow, goals (SMART), monthly evaluations, version history, PDF export
- **Calendar / consultations**: Event CRUD, participants (internal + external), response tracking, reminder scheduling
- **Reminders**: Control-exam reminders 2 months in advance, background job (`ReminderBackgroundService`), email delivery
- **Chat / messaging**: 1:1 and group conversations, message threading, attachments, unread tracking
- **Notifications**: In-app notification bell, unread count, notification center
- **Audit log**: Every create/update/delete/view on sensitive entities, viewable by Admin
- **GDPR**: Data export (user request), user consent tracking, soft-delete across entities
- **Admin console**: Schools CRUD, Users CRUD, Audit log viewer, Integration endpoints CRUD
- **Public site**: Marketing pages (Home, HowItWorks, Pricing, Contact), feature pages, legal pages, auth pages
- **Localization**: Czech (default), English, German via `.resx` files — resource keys in both web projects
- **Two-app architecture**: `Web.Public` (specedu.cz) + `Web.App` (app.specedu.cz) with shared auth cookie, DataProtection keys in DB
- **Email**: Real SMTP via MailKit (STARTTLS, configurable `DefaultTo` override for testing)
- **Seed data**: 4 test accounts, 1 school, 3 students, 6 diary entries, 2 PLPPs with goals/evaluations, consultation events, notifications

### Partial
- **External integrations (PPP/SPC)**: Endpoint CRUD + audit records are real; `TestConnectionAsync` and actual HTTP data exchange are **stubbed** — waiting for the external API specification
- **User management (Admin)**: Index / Create / Edit pages exist; role-change UX and bulk actions not verified
- **GDPR export**: `GdprService` + `/Account/DataExport` page exist; full exercise of "right to erasure" flow not verified
- **Tests**: 17 test files across Domain / Application / Infrastructure; `Web.App.Tests` and `Web.Public.Tests` projects exist but are **empty placeholders**

### Not done
- **IVP module**: Sprint-plan item — not implemented. (PLPP covers Level 2 support; IVP is a separate document for Level 3+ and is missing.)
- **School information system (SIS) integration**: no importers yet
- **Broad statistics / reporting dashboards**: out of MVP scope per Base.txt
- **Mobile native app**: explicitly out of scope

---

## [Unreleased]

### Planned
- Exercise GDPR data export / deletion end-to-end
- Wire real HTTP calls in `ExternalIntegrationService` once PPP/SPC API spec is available
- Add Web.App / Web.Public test projects content
- IVP module (Level 3+ support plan, analogous to PLPP)

---

## [0.6.0] - 2026-02-17 (Two-project architecture split)

### Added
- **SpecEdu.Web.Public** project — marketing / landing / auth pages (specedu.cz, port 5000)
- **SpecEdu.Web.App** project — authenticated management app (app.specedu.cz, port 5001)
- **Shared authentication cookie** `.SpecEdu.Auth` scoped to `.specedu.cz` in production
- **DataProtection keys in DB** (`IDataProtectionKeyContext`) so both apps share cookie decryption keys
- **Cross-app redirect flow**: unauthenticated App → Public login; successful login → App Dashboard; logout → Public site
- **Separate `.resx` resources** per web project (duplicated; may be unified later)
- **Independent layouts**: Public uses `_Header` + `_Footer`; App uses `_Sidebar` + `_TopBar`

### Changed
- Old combined `SpecEdu.Web` project removed; logic split across the two new projects
- `Program.cs` hardened in both apps: cookie name/domain, HTTPS enforcement, localization middleware
- `DEVELOPMENT.md` rewritten to describe the two-project architecture

### Notes
- Both apps share the same DB and run against the same `ApplicationDbContext`
- A single commit in main changes port layouts; tests must run against `Web.App` as startup for EF tooling

---

## [0.5.0] - 2026-02-16 (Sprint 7: Chat + Notifications + Integrations + Admin)

### Added
- **Chat / inbox** (`ChatService`, `Chat/Index.cshtml`)
  - `Conversation`, `ConversationParticipant`, `ChatMessage`, `ChatAttachment` entities
  - 1:1 and group conversations, message threading via `ParentMessageId`, soft-delete of messages
  - Per-participant `LastReadAt` → unread counts
  - File attachments stored via blob path
- **Notifications** (`NotificationService`, `NotificationBell` view component, `Notifications/Index.cshtml`)
  - In-app notification types (Info / Success / Warning / Error) with optional link and related-entity pointer
  - Unread count in top bar, mark-as-read and mark-all-as-read actions
- **External integrations** (`ExternalIntegrationService`, `Admin/Integrations/Index.cshtml`)
  - `IntegrationEndpoint` entity (PPP / SPC endpoints with masked API key)
  - `DataExchangeRecord` immutable audit of every outbound/inbound call
  - `TestConnectionAsync` is a stub that writes a failed record with "API spec pending" — real HTTP calls deferred
- **Admin console**
  - `Admin/Users/*` — user CRUD with role assignment
  - `Admin/AuditLog/Index.cshtml` — filterable audit log viewer
  - `Admin/Schools/Edit` — edit existing schools (Create already existed)
- **GDPR export** (`GdprService`, `Account/DataExport.cshtml`) — user-requested data export
- **UserConsent entity** — GDPR consent tracking (granted, timestamp, IP)

### Database Migration
- `20260120020644_AddPlppVersions` (combined) — adds `PlppVersion`, `ConsultationEvent`, `ConsultationParticipant`, `Notification`, `Conversation`, `ConversationParticipant`, `ChatMessage`, `ChatAttachment`, `IntegrationEndpoint`, `DataExchangeRecord`, `AuditLog`, `UserConsent` tables

---

## [0.4.0] - 2026-01-23 (Sprints 5–6: PLPP editor + Calendar)

### Added — PLPP (Plán pedagogické podpory)
- **Entities**: `Plpp`, `PlppGoal`, `PlppEvaluation`, `PlppVersion`
- **Workflow**: Draft → Active (activation captures a `PlppVersion` snapshot) → Archived
- **Goals**: SMART-style with order, subject, success criteria, status (`NotStarted` / `InProgress` / `Completed`), progress notes, target date, responsible person
- **Monthly evaluations**: What student manages, what needs improvement, recommended adjustments, parent-consultation notes, 1–5 progress rating, parent-notified flag
- **Version history** (`PlppVersionService`): JSON snapshot per version, change summary, source (`Activation` / `Modification` / `Manual`), diff viewing
- **PDF export** (`PdfService`, ~550 lines): full PLPP + goals + evaluations, with/without internal notes
- **Pages**: `Plpps.cshtml`, `PlppCreate`, `PlppEdit`, `PlppVersions`, `PlppDownload`, `PlppVersionDownload`
- **Parent view**: `MyChildren/PlppDownload.cshtml` for parent-visible PLPPs only
- **Duplicate for new school year** helper on `IPlppService`

### Added — Calendar / Consultations
- **Entities**: `ConsultationEvent`, `ConsultationParticipant`
- **Event types**: IndividualConsultation, GroupConsultation, SchoolEvent, ParentMeeting, StaffMeeting
- **Participants**: internal users (by `UserId`) + external (name + email), with response status (Pending / Accepted / Declined), organizer flag, required flag
- **Scheduling**: start/end time, location, online meeting link, optional link to student and/or PLPP
- **Reminders**: `ReminderMinutesBefore` default 1440 (24 h), `ReminderSent` tracking
- **Pages**: `SchoolAdmin/Calendar/{Index,Create,Details,Edit}`, `Dashboard/Calendar` (staff view), `MyChildren/Calendar` (parent view)
- **Shared partial**: `_CalendarEventModal.cshtml`, sidebar filter `_CalendarSidebarContent.cshtml`

### Database Migration
- `20260119193658_AddPlpp` — PLPP, PlppGoal, PlppEvaluation tables

---

## [0.3.0] - 2026-01-19 (Sprint 4: Reminders for control examinations)

### Added
- **Reminder entity** with `StudentId`, `DueDate`, `NotifyAt` (= `DueDate` − 2 months), `Channel` (Email), `Status` (Pending / Sent / Failed / Cancelled), retry count, last error
- **ReminderService** — CRUD + pending/sent/failed transitions
- **ReminderBackgroundService** — `IHostedService` polling for pending reminders and dispatching via `IEmailService`
- **EmailService** (`MailKit`) — STARTTLS SMTP, configurable `DefaultTo` override for safe testing
- **Reminders UI** — `SchoolAdmin/Students/Reminders.cshtml`
- **Test infrastructure additions** — `SpecEdu.Domain.Tests`, `SpecEdu.Application.Tests`, `SpecEdu.Infrastructure.Tests` gain the first real tests (17 files total across the three)

### Database Migration
- `20260119160341_AddReminders`

---

## [0.2.0] - 2026-01-18 (Sprints 2–3: Students + Access control + Diary)

### Added — Student & access entities
- **Student** (`SchoolId`, `FirstName`, `LastName`, `BirthDate`, `Class`, `PhotoId`, `IsActive`) — soft-deletable
- **StudentGuardian** — links parent user to student with `RelationshipType` (Mother / Father / LegalGuardian / Other)
- **StudentStaffLink** — links teacher / assistant / SPP / PPP / SPC to student with `AccessLevel` (Read / Edit)
- **StudentAccessService** — central gate: "which students can user X see, and at what level?" (used by every page that touches student data)
- **Student pages**: `SchoolAdmin/Students/{Index,Create,Edit,Details}`, `Guardians`, `StaffLinks`
- **Parent pages**: `MyChildren/{Index,ChildDetails}`
- **Authorization**: `StudentAccessRequirement` + handler; role × link filtering enforced at query level (not just UI)

### Added — Communication diary
- **DiaryEntry** — types: Note, PhoneCall, Meeting, ParentCollaboration, PppSpcCollaboration; `Visibility` = SchoolOnly or ParentVisible
- **DiaryAttachment** — binary file data stored inline (files; path-based storage planned)
- **DiaryService** — CRUD, visibility-aware queries, attachment upload/download, entry-count-by-type statistics
- **Diary pages**: `SchoolAdmin/Students/Diary`, `DiaryCreate`, `DiaryEdit`, `DiaryDownload`; parent-facing `MyChildren/ChildDiary`

### Database Migration
- `20260118220214_AddStudentAndAccessEntities`
- `20260118222751_AddDiaryEntities`

### Test accounts (added to seeder)
| Email | Password | Role |
|-------|----------|------|
| rodic@testskola.cz | Rodic123! | Parent (linked to Jan + Marie Novák) |

### Seed data
- Test school "Testovací ZŠ", 3 students (Jan Novák, Marie Nováková, Petr Svoboda)
- Guardian links for parent, staff links for teacher (Edit on Jan & Marie, Read on Petr)
- 6 diary entries demonstrating each type and both visibility settings

---

## [0.1.0] - 2026-01-17 (Sprint 1: Identity + Role + Multi-tenant School)

### Added
- **School entity (multi-tenant)**: Schools table with Name, ICO, Address, City, PostalCode, ContactEmail, ContactPhone, InstitutionType, IsActive, LicenseExpiresAt
- **Permission constants**: Student (View/Edit/Create/Delete), Document (View/Edit/Create/Delete), Administration (ManageSchool/ManageUsers/ManageRoles/ViewAuditLog), System (SystemAdmin/ManageSchools)
- **SchoolAdmin role**: New role for school-level administrators
- **ApplicationUserDto, SchoolDto, AuthResult**: DTOs for user, school, and authentication data
- **IIdentityService**: Interface for user management (authenticate, CRUD, role management)
- **ISchoolService**: Interface for school CRUD operations
- **IdentityService**: Full implementation of user authentication and management
- **SchoolService**: Full implementation of school CRUD
- **Permission-based authorization**: PermissionRequirement, PermissionAuthorizationHandler, RolePermissions
- **Authorization policies**: RequireAdmin, RequireSchoolAdmin, CanViewStudent, CanEditStudent, CanCreateStudent, CanDeleteStudent, CanViewDocument, CanEditDocument, CanManageUsers, CanManageSchool
- **Login page**: `/Account/Login` with email/password, remember me, lockout handling
- **Logout page**: `/Account/Logout` with confirmation
- **AccessDenied page**: `/Account/AccessDenied` with user-friendly message
- **Admin dashboard**: `/Admin` with school/user counts and quick actions
- **Schools management**: `/Admin/Schools` list and `/Admin/Schools/Create` form
- **DbSeeder**: Automatic seeding of roles, admin user, and test data (development only)
- **Cookie authentication configuration**: HttpOnly, Secure, SameSite=Strict, 24h expiration

### Changed
- **ApplicationUser**: Added SchoolId (Guid?) and School navigation property
- **ICurrentUserService**: Added SchoolId, IsAuthenticated, Roles, IsInRole()
- **IJwtTokenService**: Added schoolId parameter to GenerateToken()
- **JwtTokenService**: Added school_id claim to JWT tokens
- **Roles constant**: Updated Admin description, added SchoolAdmin to All array
- **Header**: Shows Login/Register for anonymous, user menu with logout for authenticated, Admin link for admins
- **Program.cs**: Added cookie configuration and DbSeeder call

### Database Migration
- Added `Schools` table with proper indexes (ICO unique, Name, IsActive)
- Added `SchoolId` column to `AspNetUsers` with FK to Schools (ON DELETE SET NULL)
- Added indexes on AspNetUsers (SchoolId, IsActive)

### Test Accounts (Development)
| Email | Password | Role |
|-------|----------|------|
| admin@specedu.cz | Admin123! | Admin (global) |
| spravce@testskola.cz | Spravce123! | SchoolAdmin |
| ucitel@testskola.cz | Ucitel123! | Teacher |

### Security Features
- Cookie HttpOnly, Secure, SameSite=Strict
- Anti-forgery tokens on all forms (Razor Pages default)
- Password policy: 8+ chars, uppercase, lowercase, digit, special
- Account lockout: 5 failed attempts = 5 minute lockout
- SchoolId in JWT claims for tenant filtering

---

## [0.0.8] - 2025-01-17

### Fixed
- Layout shift/flick when navigating between pages
- Replaced absolute positioning footer with flexbox-based sticky footer
- Added `spec-content-wrapper` class for proper flex layout

### Added
- `Test.cshtml` page for testing components and styles
- Test page includes: buttons, cards, forms, colors, typography examples
- Test link added to navigation header

### Changed
- Body now uses `display: flex; flex-direction: column; min-height: 100vh`
- Main content wrapper uses `flex: 1` to push footer to bottom
- Footer no longer uses `position: absolute`

---

## [0.0.7] - 2025-01-17

### Added
- `Log.cs` centralized logging using `LoggerMessage` source generators
- High-performance structured logging with compile-time generation
- Event ID categorization system
- Startup logging in `Program.cs`

### Event ID Ranges
| Range | Category | Description |
|-------|----------|-------------|
| 1000-1999 | Database | CRUD operations, migrations |
| 2000-2999 | Authentication | Login, logout, tokens, registration |
| 3000-3999 | Authorization | Role checks, permissions |
| 4000-4999 | Application | Students, documents, IVP, PLPP |
| 5000-5999 | Infrastructure | Email, file storage, external services |
| 9000-9999 | System | Startup, config, health checks |

---

## [0.0.6] - 2025-01-17

### Added
- CSS variables for consistent theming (`--spec-primary`, `--spec-secondary`, etc.)
- Light blue color scheme as per requirements
- `_Header.cshtml` partial with responsive navigation
- `_Footer.cshtml` partial with copyright and links
- Custom button styles (`.btn-spec-primary`, `.btn-spec-outline`)
- Card styles (`.spec-card`, `.spec-card-header`, `.spec-card-body`)
- Utility classes for colors and backgrounds
- Czech language labels in navigation

### Changed
- Updated `_Layout.cshtml` to use header/footer partials
- Changed `lang="en"` to `lang="cs"` for Czech
- Background color now uses `--spec-gray-100` for better contrast

---

## [0.0.5] - 2025-01-17

### Added
- ASP.NET Identity with custom `ApplicationUser` entity
- JWT Bearer authentication
- `ApplicationUser` with FirstName, LastName, IsActive properties
- `Roles` constants in Domain layer (7 roles defined)
- `JwtSettings` configuration class
- `IJwtTokenService` interface in Application layer
- `JwtTokenService` implementation for token generation
- JWT configuration in `appsettings.json`

### Changed
- `ApplicationDbContext` now inherits from `IdentityDbContext`
- Updated `DependencyInjection.cs` with Identity and JWT registration
- Added `UseAuthentication()` middleware in `Program.cs`

---

## [0.0.4] - 2025-01-17

### Added
- Entity Framework Core 9.0 with SQL Server provider
- `ApplicationDbContext` in Infrastructure layer
- Automatic audit field population on `SaveChanges()`
- `ICurrentUserService` interface in Application layer
- `CurrentUserService` implementation in Web layer
- `DependencyInjection.cs` extension method for clean service registration
- Connection string configuration in `appsettings.json`

---

## [0.0.3] - 2025-01-17

### Added
- `Common/` folder in Domain layer for shared base classes
- `Entities/` folder in Domain layer for future entity classes
- `BaseEntity` abstract class with `Id` (Guid) property
- `IAuditableEntity` interface defining audit fields contract
- `AuditableEntity` abstract class combining BaseEntity + IAuditableEntity

---

## [0.0.2] - 2025-01-17

### Added
- Clean Architecture solution structure implemented
- `src/SpecEdu.Domain` - Domain layer
- `src/SpecEdu.Application` - Application layer
- `src/SpecEdu.Infrastructure` - Infrastructure layer
- `src/SpecEdu.Web` - Web layer (later split into Web.Public + Web.App in 0.6.0)
- Test projects for Domain / Application / Infrastructure

---

## [0.0.1] - 2025-01-17

### Added
- Initial Razor Pages project created (default template)
- Google Sans font self-hosted in `wwwroot/fonts/`
- Global font applied via `site.css`
- Project documentation (`Base.txt`) with full scope and requirements
- Git repository initialized
- `.gitignore` configured for .NET projects
- `CHANGELOG.md` for tracking development progress

---

## Version History

| Version | Date | Description |
|---------|------|-------------|
| 0.6.0 | 2026-02-17 | Two-project architecture split (Web.Public + Web.App) |
| 0.5.0 | 2026-02-16 | Chat, notifications, integrations, admin console, audit log, GDPR |
| 0.4.0 | 2026-01-23 | PLPP editor (goals, evaluations, versions, PDF) + Calendar |
| 0.3.0 | 2026-01-19 | Control-exam reminders + email service |
| 0.2.0 | 2026-01-18 | Students, access control (guardians / staff links), communication diary |
| 0.1.0 | 2026-01-17 | Identity + roles + multi-tenant schools |
| 0.0.8 | 2025-01-17 | Flexbox sticky footer, Test page |
| 0.0.7 | 2025-01-17 | Centralized logging with LoggerMessage source generators |
| 0.0.6 | 2025-01-17 | Layout, header, footer, CSS variables, light blue theme |
| 0.0.5 | 2025-01-17 | ASP.NET Identity + JWT authentication, roles |
| 0.0.4 | 2025-01-17 | EF Core + SQL Server, ApplicationDbContext, audit fields |
| 0.0.3 | 2025-01-17 | BaseEntity, IAuditableEntity, AuditableEntity |
| 0.0.2 | 2025-01-17 | Clean Architecture structure, test projects |
| 0.0.1 | 2025-01-17 | Initial project setup, font configuration |

---

## Legend

- **Added**: New features
- **Changed**: Changes to existing functionality
- **Deprecated**: Features that will be removed in future
- **Removed**: Features that were removed
- **Fixed**: Bug fixes
- **Security**: Security-related changes
- **Technical Decisions**: Architecture and technology choices with reasoning
