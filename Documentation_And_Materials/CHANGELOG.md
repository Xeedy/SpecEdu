# SpecEdu Development Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Planned
- Step 0.3: BaseEntity with audit fields
- Step 0.4: EF Core + SQL Server setup
- Step 0.5: ASP.NET Identity + JWT authentication
- Step 0.6: Layout, header, footer, CSS variables
- Step 0.7: Serilog structured logging

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
