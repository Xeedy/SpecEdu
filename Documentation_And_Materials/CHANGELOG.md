# SpecEdu Development Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Planned
- User management pages
- Student CRUD operations

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

### File Structure
```
src/SpecEdu.Domain/
├── Entities/
│   └── School.cs                    ← NEW: Multi-tenant school entity
└── Constants/
    ├── Permissions.cs               ← NEW: All permission constants
    └── Roles.cs                     ← UPDATED: Added SchoolAdmin

src/SpecEdu.Application/
└── Common/
    ├── Interfaces/
    │   ├── ICurrentUserService.cs   ← UPDATED: SchoolId, IsAuthenticated, Roles
    │   ├── IIdentityService.cs      ← NEW: User management interface
    │   └── ISchoolService.cs        ← NEW: School CRUD interface
    └── Models/
        ├── ApplicationUserDto.cs    ← NEW
        ├── SchoolDto.cs             ← NEW
        └── AuthResult.cs            ← NEW

src/SpecEdu.Infrastructure/
├── Authorization/
│   ├── PermissionRequirement.cs           ← NEW
│   ├── PermissionAuthorizationHandler.cs  ← NEW
│   ├── RolePermissions.cs                 ← NEW
│   └── AuthorizationPolicies.cs           ← NEW
├── Data/
│   ├── Configurations/
│   │   ├── SchoolConfiguration.cs         ← NEW
│   │   └── ApplicationUserConfiguration.cs ← NEW
│   ├── ApplicationDbContext.cs            ← UPDATED: Added Schools DbSet
│   └── DbSeeder.cs                        ← NEW
├── Identity/
│   ├── ApplicationUser.cs                 ← UPDATED: SchoolId property
│   ├── IdentityService.cs                 ← NEW
│   └── JwtTokenService.cs                 ← UPDATED: school_id claim
├── Services/
│   └── SchoolService.cs                   ← NEW
└── DependencyInjection.cs                 ← UPDATED: New registrations

src/SpecEdu.Web/
├── Pages/
│   ├── Account/
│   │   ├── Login.cshtml[.cs]              ← NEW
│   │   ├── Logout.cshtml[.cs]             ← NEW
│   │   └── AccessDenied.cshtml[.cs]       ← NEW
│   ├── Admin/
│   │   ├── Index.cshtml[.cs]              ← NEW
│   │   └── Schools/
│   │       ├── Index.cshtml[.cs]          ← NEW
│   │       └── Create.cshtml[.cs]         ← NEW
│   └── Shared/
│       └── _Header.cshtml                 ← UPDATED: Auth-aware buttons
├── Services/
│   └── CurrentUserService.cs              ← UPDATED: New interface members
└── Program.cs                             ← UPDATED: Cookie config, seeder
```

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

### Layout Structure (Flexbox)
```
body (flex container, column)
├── header.spec-header
├── div.container.spec-content-wrapper (flex: 1)
│   └── main.spec-main (flex: 1)
└── footer.spec-footer
```

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

### File Structure
```
src/SpecEdu.Web/
└── Logging/
    └── Log.cs    ← Centralized logging (50+ log methods)
```

### Usage Example
```csharp
// Instead of: _logger.LogInformation("User {Email} logged in", email);
// Use:        Log.LoginSuccess(logger, email);
```

### Benefits
- Compile-time generated (high performance, no boxing)
- Strongly typed parameters
- Centralized (easy to modify messages)
- Consistent event IDs for filtering/monitoring

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

### CSS Variables Defined
```css
--spec-primary: #4A90D9       /* Main blue */
--spec-primary-dark: #357ABD  /* Hover state */
--spec-primary-light: #E8F4FD /* Light backgrounds */
--spec-secondary: #6C757D     /* Secondary text */
--spec-success: #28A745       /* Success states */
--spec-warning: #FFC107       /* Warnings */
--spec-danger: #DC3545        /* Errors */
```

### File Structure
```
Pages/Shared/
├── _Layout.cshtml    ← Main layout (updated)
├── _Header.cshtml    ← New header partial
└── _Footer.cshtml    ← New footer partial

wwwroot/css/
└── site.css          ← CSS variables + component styles
```

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

### Roles Defined
| Role | Czech | Description |
|------|-------|-------------|
| Admin | Správce | School administrator (one per school) |
| Teacher | Pedagog | Teachers managing students |
| Parent | Rodič | View-only access to their child |
| SPP | ŠPP | School counseling center |
| PPP | PPP | Pedagogical-psychological counseling |
| SPC | SPC | Special pedagogical center |
| Assistant | Asistent | Teaching assistants |

### Infrastructure Structure
```
src/SpecEdu.Infrastructure/
└── Identity/
    ├── ApplicationUser.cs     ← Custom user entity
    ├── JwtSettings.cs         ← JWT configuration
    └── JwtTokenService.cs     ← Token generation

src/SpecEdu.Domain/
└── Constants/
    └── Roles.cs               ← Role constants
```

### NuGet Packages Added
| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 9.0.* | Identity with EF Core |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.* | JWT authentication |

### Password Policy
- Minimum 8 characters
- Requires digit, lowercase, uppercase, special character
- Account lockout after 5 failed attempts (5 min)

### JWT Configuration
- Token expiration: 60 minutes
- Refresh token expiration: 7 days
- HMAC-SHA256 signing

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

### Infrastructure Structure
```
src/SpecEdu.Infrastructure/
├── Data/
│   └── ApplicationDbContext.cs    ← Main DbContext with audit tracking
└── DependencyInjection.cs         ← Service registration extension

src/SpecEdu.Application/
└── Common/
    └── Interfaces/
        └── ICurrentUserService.cs ← Current user abstraction

src/SpecEdu.Web/
└── Services/
    └── CurrentUserService.cs      ← HTTP context user provider
```

### NuGet Packages Added (Infrastructure)
| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.* | SQL Server database provider |
| Microsoft.EntityFrameworkCore.Tools | 9.0.* | EF Core CLI tools (migrations) |

### How Audit Fields Work
```
On SaveChanges():
├── New entity (Added)
│   ├── CreatedAt = DateTime.UtcNow
│   └── CreatedBy = CurrentUser.UserId
└── Modified entity (Modified)
    ├── ModifiedAt = DateTime.UtcNow
    └── ModifiedBy = CurrentUser.UserId
```

### Connection String
```
Server=(localdb)\\mssqllocaldb;Database=SpecEduDb;Trusted_Connection=True
```

---

## [0.0.3] - 2025-01-17

### Added
- `Common/` folder in Domain layer for shared base classes
- `Entities/` folder in Domain layer for future entity classes
- `BaseEntity` abstract class with `Id` (Guid) property
- `IAuditableEntity` interface defining audit fields contract
- `AuditableEntity` abstract class combining BaseEntity + IAuditableEntity

### Domain Structure
```
src/SpecEdu.Domain/
├── Common/
│   ├── BaseEntity.cs         ← Id property (Guid)
│   ├── IAuditableEntity.cs   ← Audit fields interface
│   └── AuditableEntity.cs    ← BaseEntity + audit fields
└── Entities/
    └── (future entities inherit from AuditableEntity)
```

### Audit Fields (GDPR Compliance)
| Field | Type | Purpose |
|-------|------|---------|
| `Id` | `Guid` | Unique identifier |
| `CreatedAt` | `DateTime` | UTC creation timestamp |
| `CreatedBy` | `string?` | User who created |
| `ModifiedAt` | `DateTime?` | UTC modification timestamp |
| `ModifiedBy` | `string?` | User who modified |

### Design Decision
- Separated `BaseEntity` (just Id) from `IAuditableEntity` (audit fields)
- Reason: Not all entities need audit tracking (e.g., lookup tables)
- Most entities will inherit from `AuditableEntity` (combines both)

---

## [0.0.2] - 2025-01-17

### Added
- Clean Architecture solution structure implemented
- `src/SpecEdu.Domain` - Domain layer (entities, interfaces, business rules)
- `src/SpecEdu.Application` - Application layer (use cases, DTOs, services)
- `src/SpecEdu.Infrastructure` - Infrastructure layer (EF Core, external services)
- `src/SpecEdu.Web` - Web layer (Razor Pages, controllers, views)
- `tests/SpecEdu.Domain.Tests` - Domain unit tests (xUnit)
- `tests/SpecEdu.Application.Tests` - Application unit tests (xUnit)
- `tests/SpecEdu.Infrastructure.Tests` - Infrastructure integration tests (xUnit)

### Changed
- Moved existing Razor Pages project to `src/SpecEdu.Web/`
- Renamed project from `SpecEdu.csproj` to `SpecEdu.Web.csproj`
- Updated solution file with all 7 projects

### Project References (Dependency Flow)
```
Domain (no dependencies)
   ↑
Application (depends on Domain)
   ↑
Infrastructure (depends on Domain, Application)
   ↑
Web (depends on Application, Infrastructure)
```

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

### Technical Decisions

| Decision | Choice | Reason |
|----------|--------|--------|
| Architecture | Clean Architecture | Complex domain, 6+ roles, GDPR compliance, multiple integrations |
| Database | SQL Server (MSSQL) | Client requirement, developer has local instance |
| Authentication | ASP.NET Identity + JWT | Industry standard, stateless auth, role-based access |
| Font | Google Sans (self-hosted) | GDPR compliant, no external requests |
| Primary Language | Czech | Client requirement, localization planned for future |

### Project Context
- **Project**: SpecEdu - Platform for schools and educational counseling facilities
- **Purpose**: Manage students with special educational needs (SVP, SPU, LMP)
- **Target Users**: Schools, PPP, SPC, parents, teachers, assistants
- **Development Approach**: Agile, 2-week sprints, methodical progression

---

## Version History

| Version | Date | Description |
|---------|------|-------------|
| 0.0.8 | 2025-01-17 | Fixed layout shift, flexbox sticky footer, Test page |
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
