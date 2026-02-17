# SpecEdu — Developer Guide

> Two-project architecture: **SpecEdu.Web.Public** (specedu.cz) + **SpecEdu.Web.App** (app.specedu.cz)

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Getting Started](#getting-started)
3. [Running the Projects](#running-the-projects)
4. [Project Responsibilities](#project-responsibilities)
5. [Authentication Flow](#authentication-flow)
6. [Working in a Team](#working-in-a-team)
7. [Coding Conventions](#coding-conventions)
8. [Localization](#localization)
9. [Adding New Pages](#adding-new-pages)
10. [Database & Migrations](#database--migrations)
11. [Testing](#testing)
12. [Debugging](#debugging)
13. [Deployment](#deployment)
14. [Troubleshooting](#troubleshooting)

---

## Architecture Overview

```
SpecEdu.sln
├── src/
│   ├── SpecEdu.Domain/            # Entities, enums, constants (no dependencies)
│   ├── SpecEdu.Application/       # Interfaces, DTOs, business contracts
│   ├── SpecEdu.Infrastructure/    # EF Core, Identity, services, email, PDF
│   ├── SpecEdu.Web.Public/        # Marketing site (specedu.cz)
│   └── SpecEdu.Web.App/           # Management app (app.specedu.cz)
├── tests/
│   ├── SpecEdu.Domain.Tests/
│   ├── SpecEdu.Application.Tests/
│   ├── SpecEdu.Infrastructure.Tests/
│   ├── SpecEdu.Web.Public.Tests/
│   └── SpecEdu.Web.App.Tests/
```

**Dependency flow** (outer depends on inner):

```
Web.Public / Web.App
        │
   Infrastructure
        │
    Application
        │
      Domain
```

Both web projects reference `Application` + `Infrastructure`. They share the same database, Identity system, and authentication cookies. Changes to Domain/Application/Infrastructure affect both.

---

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB or full instance)
- IDE: Visual Studio 2022+ / Rider / VS Code with C# Dev Kit
- Git

### First-Time Setup

```bash
# Clone and restore
git clone <repo-url>
cd SpecEdu
dotnet restore

# Set up user secrets for both web projects (connection string, mail, etc.)
dotnet user-secrets set "ConnectionStrings:SpecEduConnectionString" "Server=(localdb)\\mssqllocaldb;Database=SpecEdu;Trusted_Connection=True;" --project src/SpecEdu.Web.Public
dotnet user-secrets set "ConnectionStrings:SpecEduConnectionString" "Server=(localdb)\\mssqllocaldb;Database=SpecEdu;Trusted_Connection=True;" --project src/SpecEdu.Web.App

# Build everything
dotnet build
```

> **Tip:** Both projects must share the same connection string — they use the same database.

### Initialize User Secrets

Each web project has its own `UserSecretsId`. Secrets you need:

| Key | Description |
|-----|-------------|
| `ConnectionStrings:SpecEduConnectionString` | SQL Server connection string |
| `JwtSettings:Secret` | JWT signing key (min 32 chars) |
| `Mail:SmtpHost`, `Mail:SmtpPort`, `Mail:SmtpUser`, `Mail:SmtpPass` | SMTP for emails |
| `AdminUser:Email`, `AdminUser:Password` | Seed admin (development only) |

Set the same secrets for **both** projects or create a shared `appsettings.Local.json` (gitignored).

---

## Running the Projects

### Option A: Two Terminals (Recommended for focused work)

```bash
# Terminal 1 — Public site
dotnet run --project src/SpecEdu.Web.Public
# → https://localhost:5000

# Terminal 2 — App
dotnet run --project src/SpecEdu.Web.App
# → https://localhost:5001
```

### Option B: Visual Studio — Multiple Startup Projects

1. Right-click the Solution → **Configure Startup Projects**
2. Select **Multiple startup projects**
3. Set both `SpecEdu.Web.Public` and `SpecEdu.Web.App` to **Start**
4. Press F5 — both launch together

### Option C: Run Only What You Need

Working on the App only? Just run the App project. Login will fail (no Public site to redirect to), so navigate directly to `https://localhost:5001/Dashboard` while already authenticated via a shared cookie, or temporarily adjust `LoginPath` in `Program.cs` during development.

**Practical shortcut for App-only development:** If you need to log in without the Public site, you can:
1. Run both projects briefly, log in via Public, then stop Public
2. The auth cookie persists — continue working on App alone

---

## Project Responsibilities

### SpecEdu.Web.Public (specedu.cz)

| Area | Pages |
|------|-------|
| Landing & Marketing | Index, HowItWorks, Pricing, Contact |
| System Info | System/Index, ForClients, Security, Documentation |
| Feature Descriptions | Functions/StudentFiles, Documents, Diary, Notifications |
| Legal | Legal/Terms, Privacy, Cookies |
| Authentication | Account/Login, Register, ForgotPassword, ResetPassword, Logout |
| Utility | Culture/Set, Error |

**Layout:** Traditional header + content + footer (`_Header.cshtml`, `_Footer.cshtml`).

### SpecEdu.Web.App (app.specedu.cz)

| Area | Pages |
|------|-------|
| Dashboard | Dashboard/Index, Calendar |
| Student Management | SchoolAdmin/Students/* (Index, Create, Edit, Details, Diary, PLPP, etc.) |
| Calendar Management | SchoolAdmin/Calendar/* (Index, Create, Edit, Details) |
| Parent View | MyChildren/* (Index, ChildDetails, ChildDiary, Calendar, PlppDownload) |
| Administration | Admin/Index, Admin/Schools/* |
| User Account | Account/Profile, Settings, AccessDenied, Logout |
| Utility | Culture/Set, Error |

**Layout:** Sidebar + top bar (`_Sidebar.cshtml`, `_TopBar.cshtml`). Dark sidebar with role-based navigation.

### Shared Layers (both projects use these)

| Layer | What It Does |
|-------|--------------|
| `Domain` | Entities (`Student`, `School`, `Plpp`, etc.), enums, constants |
| `Application` | Interfaces (`IStudentService`, `IPlppService`, etc.), DTOs, models |
| `Infrastructure` | EF Core context, Identity, services implementation, email, PDF, background jobs |

> **Rule:** If you're adding business logic or data access, it goes into Infrastructure. If you're defining contracts or DTOs, it goes into Application. UI/pages go into the respective web project.

---

## Authentication Flow

```
User visits app.specedu.cz
        │
        ▼
  Authenticated? ──No──► Redirect to specedu.cz/Account/Login?returnUrl=...
        │
       Yes
        │
        ▼
  Show App (sidebar layout)
```

```
User logs in at specedu.cz/Account/Login
        │
        ▼
  Success ──► Redirect to app.specedu.cz/Dashboard
  Failure ──► Show error on login page
```

```
User logs out from App
        │
        ▼
  Cookie cleared ──► Redirect to specedu.cz
```

### How It Works Technically

- Both apps share cookie name: `.SpecEdu.Auth`
- Both apps share DataProtection keys (stored in DB via `IDataProtectionKeyContext`)
- Production cookie domain: `.specedu.cz` (covers both subdomains)
- Development: cookies are on `localhost` (different ports, same domain — works automatically)

---

## Working in a Team

### Developer A: App (app.specedu.cz)

Your workspace:
- `src/SpecEdu.Web.App/` — pages, layouts, CSS, JS
- `src/SpecEdu.Infrastructure/` — services, data access (shared, coordinate changes)
- `src/SpecEdu.Application/` — interfaces, DTOs (shared, coordinate changes)
- `tests/SpecEdu.Web.App.Tests/`

### Developer B: Public Site (specedu.cz)

Your workspace:
- `src/SpecEdu.Web.Public/` — pages, layouts, CSS, JS
- `src/SpecEdu.Infrastructure/` — services (shared, coordinate changes)
- `src/SpecEdu.Application/` — interfaces, DTOs (shared, coordinate changes)
- `tests/SpecEdu.Web.Public.Tests/`

### Coordination Rules

1. **Always `dotnet build` the full solution** before pushing — ensures both projects compile
2. **Shared layer changes need communication** — if you modify `IStudentService` or add a migration, tell the other developer
3. **Use feature branches** — `feature/app-plpp-export`, `feature/public-pricing-redesign`
4. **Migrations are shared** — only one person creates a migration at a time; coordinate via PR
5. **Resources (`.resx`) are duplicated** — each project has its own copy; keep them in sync or extract to a shared project later

---

## Coding Conventions

### C# Style

**Primary constructors** (preferred for services and page models):

```csharp
// ✅ Good — primary constructor
public class IndexModel(
    IStudentAccessService studentAccessService,
    ICurrentUserService currentUserService) : PageModel
{
    public IList<StudentDto> Students { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = currentUserService.UserId;
        Students = await studentAccessService.GetAccessibleStudentsAsync(userId!);
        return Page();
    }
}

// ❌ Avoid — manual field assignment
public class IndexModel : PageModel
{
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(IStudentAccessService sas, ICurrentUserService cus)
    {
        _studentAccessService = sas;
        _currentUserService = cus;
    }
}
```

**File-scoped namespaces:**

```csharp
namespace SpecEdu.Web.App.Pages.Dashboard;  // ✅ file-scoped

namespace SpecEdu.Web.App.Pages.Dashboard   // ❌ block-scoped
{
}
```

**Collection expressions:**

```csharp
public IList<StudentDto> Students { get; set; } = [];  // ✅
public IList<StudentDto> Students { get; set; } = new List<StudentDto>();  // ❌
```

**Nullable reference types** are enabled — handle nulls explicitly:

```csharp
var userId = currentUserService.UserId;
if (string.IsNullOrEmpty(userId))
    return RedirectToPage("/Account/Login");
```

### Razor Pages Style

- Page model class in separate `.cshtml.cs` file (never inline `@functions`)
- Use `[BindProperty]` for form inputs
- Use `[TempData]` for flash messages
- Use `asp-page` tag helpers for links (not hardcoded URLs)
- Use `@L["Key"]` for all user-facing text (see [Localization](#localization))

### CSS Style

- Use existing CSS variables (`var(--spec-primary)`, `var(--spec-gray-200)`, etc.)
- App-specific styles go in `wwwroot/css/app.css`
- Public-specific styles go in `wwwroot/css/site.css` and `wwwroot/css/Layout/Layout.css`
- Use BEM-like naming: `.sidebar-link`, `.sidebar-link.active`, `.topbar-right`
- Bootstrap 5 utilities are available in both projects

---

## Localization

Both projects use resource files for localization.

### Resource Files

```
src/SpecEdu.Web.Public/Resources/
    SharedResource.resx          # Default (Czech)
    SharedResource.cs.resx       # Czech (explicit)
    SharedResource.en.resx       # English

src/SpecEdu.Web.App/Resources/
    SharedResource.resx          # Default (Czech)
    SharedResource.cs.resx       # Czech (explicit)
    SharedResource.en.resx       # English
```

### Usage in Razor Pages

`_ViewImports.cshtml` injects `L` globally:

```html
@inject IStringLocalizer<SharedResource> L
```

Use it in views:

```html
<h1>@L["Dashboard.Title"]</h1>
<p>@L["Dashboard.WelcomeMessage"]</p>
```

### Adding New Strings

1. Add the key to `SharedResource.resx` (Czech — this is the default/fallback)
2. Add the translation to `SharedResource.en.resx`
3. Use `@L["YourSection.YourKey"]` in the view

### Naming Convention for Keys

```
Section.SubSection.Element
```

Examples:
```
HeaderPage.System
HeaderPage.Login
Dashboard.Title
Sidebar.Students
Sidebar.Calendar
TopBar.Profile
TopBar.Logout
```

### Switching Language

Both projects have `Culture/Set` page. The language switch sets a cookie that persists for 1 year:

```html
<a asp-page="/Culture/Set" asp-route-culture="en" asp-route-returnUrl="@returnUrl">EN</a>
```

---

## Adding New Pages

### Adding a Page to the App

1. Create `Pages/YourSection/NewPage.cshtml`:

```html
@page
@model SpecEdu.Web.App.Pages.YourSection.NewPageModel
@{
    ViewData["Title"] = L["YourSection.NewPage.Title"];
}

<h1>@L["YourSection.NewPage.Title"]</h1>
<!-- Your content here -->
```

2. Create `Pages/YourSection/NewPage.cshtml.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpecEdu.Web.App.Pages.YourSection;

[Authorize(Policy = "YourPolicy")]  // or just [Authorize]
public class NewPageModel(
    IYourService yourService,
    ICurrentUserService currentUserService) : PageModel
{
    public YourDto Data { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Data = await yourService.GetDataAsync(currentUserService.UserId!);
    }
}
```

3. Add sidebar link in `_Sidebar.cshtml`:

```html
<li>
    <a class="sidebar-link @(currentPage.Contains("YourSection/NewPage") ? "active" : "")"
       asp-page="/YourSection/NewPage">
        <i class="bi bi-icon-name"></i>
        <span>@L["Sidebar.NewPage"]</span>
    </a>
</li>
```

4. Add localization keys to both `.resx` files.

### Adding a Page to the Public Site

Same pattern, but:
- Namespace: `SpecEdu.Web.Public.Pages.YourSection`
- Usually no `[Authorize]` attribute (public pages)
- Add navigation link in `_Header.cshtml` instead of sidebar

---

## Database & Migrations

### Creating a Migration

Migrations live in `Infrastructure`. Run from the repo root:

```bash
dotnet ef migrations add YourMigrationName \
    --project src/SpecEdu.Infrastructure \
    --startup-project src/SpecEdu.Web.App
```

> Use either web project as startup — both connect to the same DB.

### Applying Migrations

Migrations are applied automatically on startup via `DbSeeder.SeedAsync()` in the App's `Program.cs`. Alternatively:

```bash
dotnet ef database update \
    --project src/SpecEdu.Infrastructure \
    --startup-project src/SpecEdu.Web.App
```

### Important: DataProtection Keys Migration

After the project split, if this is a fresh database, a `DataProtectionKeys` table will be created automatically by EF Core's DataProtection. If you're running against an existing database, create a migration:

```bash
dotnet ef migrations add AddDataProtectionKeys \
    --project src/SpecEdu.Infrastructure \
    --startup-project src/SpecEdu.Web.App
```

---

## Testing

### Running Tests

```bash
# All tests
dotnet test

# Specific project
dotnet test tests/SpecEdu.Web.App.Tests
dotnet test tests/SpecEdu.Web.Public.Tests

# With output
dotnet test --logger "console;verbosity=detailed"
```

### Test Structure

```
tests/SpecEdu.Web.App.Tests/
├── Pages/
│   ├── Dashboard/
│   │   └── IndexModelTests.cs
│   └── SchoolAdmin/
│       └── Students/
│           └── CreateModelTests.cs
└── Integration/
    └── AppIntegrationTests.cs
```

### Writing a Page Model Test

```csharp
using Moq;
using FluentAssertions;

namespace SpecEdu.Web.App.Tests.Pages.Dashboard;

public class IndexModelTests
{
    [Fact]
    public async Task OnGetAsync_ReturnsStudents_ForAuthenticatedUser()
    {
        // Arrange
        var mockAccess = new Mock<IStudentAccessService>();
        var mockUser = new Mock<ICurrentUserService>();
        mockUser.Setup(u => u.UserId).Returns("user-123");
        mockAccess.Setup(s => s.GetAccessibleStudentsAsync("user-123"))
            .ReturnsAsync([new StudentDto { FirstName = "Jan" }]);

        var model = new IndexModel(mockAccess.Object, mockUser.Object);

        // Act
        await model.OnGetAsync();

        // Assert
        model.Students.Should().HaveCount(1);
        model.Students[0].FirstName.Should().Be("Jan");
    }
}
```

---

## Debugging

### Visual Studio

1. Set the desired project as startup (right-click → Set as Startup Project)
2. Or configure multiple startup projects (see [Running the Projects](#running-the-projects))
3. Set breakpoints in page models, services, or infrastructure
4. Press F5

### VS Code

`.vscode/launch.json`:

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Public Site",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/src/SpecEdu.Web.Public/bin/Debug/net9.0/SpecEdu.Web.Public.dll",
            "cwd": "${workspaceFolder}/src/SpecEdu.Web.Public",
            "env": { "ASPNETCORE_ENVIRONMENT": "Development" }
        },
        {
            "name": "App",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/src/SpecEdu.Web.App/bin/Debug/net9.0/SpecEdu.Web.App.dll",
            "cwd": "${workspaceFolder}/src/SpecEdu.Web.App",
            "env": { "ASPNETCORE_ENVIRONMENT": "Development" }
        }
    ],
    "compounds": [
        {
            "name": "Both (Public + App)",
            "configurations": ["Public Site", "App"]
        }
    ]
}
```

### Debugging the Auth Flow

1. Run both projects
2. Open `https://localhost:5000` (Public) — you should see the landing page
3. Click Login → enter credentials → you should be redirected to `https://localhost:5001/Dashboard`
4. If redirect fails, check:
   - `AppUrl` in Public's `appsettings.Development.json` points to `https://localhost:5001`
   - `PublicUrl` in App's `appsettings.Development.json` points to `https://localhost:5000`
   - Both projects share the same DB connection string

### Debugging Tips

- **Cookie issues:** Open DevTools → Application → Cookies. Look for `.SpecEdu.Auth`. Both sites should see it on `localhost`.
- **401/403 on App:** The App redirects unauthenticated users to `{PublicUrl}/Account/Login`. If Public isn't running, you'll get a connection error.
- **Razor compilation errors:** Run `dotnet build` — Razor views are compiled at build time and errors show clearly.
- **Hot reload:** `dotnet watch --project src/SpecEdu.Web.App` for live reloading during development.

---

## Deployment

### Build for Production

```bash
dotnet publish src/SpecEdu.Web.Public -c Release -o publish/public
dotnet publish src/SpecEdu.Web.App -c Release -o publish/app
```

### Configuration

Each project needs its own `appsettings.Production.json` or environment variables:

**Public Site:**
```json
{
  "AppUrl": "https://app.specedu.cz",
  "ConnectionStrings": {
    "SpecEduConnectionString": "<production-connection-string>"
  }
}
```

**App:**
```json
{
  "PublicUrl": "https://specedu.cz",
  "ConnectionStrings": {
    "SpecEduConnectionString": "<same-production-connection-string>"
  }
}
```

### Deployment Checklist

- [ ] Both apps point to the same database
- [ ] Both apps have the same `JwtSettings:Secret`
- [ ] Cookie domain is `.specedu.cz` (set automatically in production via `Program.cs`)
- [ ] DataProtection keys table exists in DB
- [ ] HTTPS is enforced on both subdomains
- [ ] DNS: `specedu.cz` → Public server, `app.specedu.cz` → App server (can be same server, different ports behind reverse proxy)

### Independent Deployment

You can deploy each project independently. Since they share the same Domain/Application/Infrastructure code at build time, there's no version mismatch risk as long as you build from the same commit. Recommended workflow:

1. Build both from the same `main` branch commit
2. Deploy Public first (lower risk — mostly static content)
3. Deploy App
4. Verify auth flow end-to-end

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Login redirects to wrong URL | Check `AppUrl` in Public's config, `PublicUrl` in App's config |
| Cookie not shared between ports | Both must use cookie name `.SpecEdu.Auth` — check `Program.cs` |
| DataProtection key error | Run migration to create `DataProtectionKeys` table |
| Build fails after pulling | `dotnet restore` then `dotnet build` |
| Razor view not found | Check namespace in `_ViewImports.cshtml` matches your page's namespace |
| Localization key not found | Add key to `Resources/SharedResource.resx` in the correct project |
| Migration conflict | Coordinate with team — only one person creates migrations at a time |
| App shows login page instead of sidebar | You're not authenticated — run Public site and log in first |

---

## Quick Reference

```bash
# Build everything
dotnet build

# Run Public site
dotnet run --project src/SpecEdu.Web.Public

# Run App
dotnet run --project src/SpecEdu.Web.App

# Run with hot reload
dotnet watch --project src/SpecEdu.Web.App

# Run all tests
dotnet test

# Create migration
dotnet ef migrations add MigrationName --project src/SpecEdu.Infrastructure --startup-project src/SpecEdu.Web.App

# Apply migrations
dotnet ef database update --project src/SpecEdu.Infrastructure --startup-project src/SpecEdu.Web.App
```

**URLs (Development):**
- Public: `https://localhost:5000`
- App: `https://localhost:5001`
- Login: `https://localhost:5000/Account/Login`
- Dashboard: `https://localhost:5001/Dashboard`
