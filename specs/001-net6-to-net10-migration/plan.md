# Implementation Plan: .NET 6 to .NET 10 LTS Migration

**Branch**: `001-net6-to-net10-migration-spec` | **Date**: 2026-04-27 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-net6-to-net10-migration/spec.md`

## Summary

Migrate the Contoso University solution (4 projects) from .NET 6 to .NET 10 LTS
in dependency order: API first (owns data layer + EF Core), then WebApplication
(Razor Pages frontend), then Test and CodedUITest projects. Upgrade all NuGet
packages, modernize the API to use top-level endpoint routing (`app.MapControllers()`)
instead of `UseEndpoints()`, update Dockerfiles to .NET 10 official images, align
Bicep `netFrameworkVersion` to `v10.0`, and update the CI/CD pipeline SDK version.

## Technical Context

**Language/Version**: C# / .NET 6.0 (source) -> .NET 10.0 (target)
**Primary Dependencies**: ASP.NET Core 10.0, EF Core 10.0, MSTest, Selenium 4.x, Microsoft.AspNetCore.OpenApi, Swashbuckle.AspNetCore.SwaggerUI, Azure.Identity, Application Insights
**Storage**: Azure SQL Database via EF Core (connection string from Key Vault)
**Testing**: MSTest (`dotnet test`), Selenium WebDriver (CodedUITest)
**Target Platform**: Azure App Service (Windows), Docker containers (Linux)
**Project Type**: Web application (Razor Pages frontend + Web API backend)
**Performance Goals**: Page load < 2s, API response < 500ms p95, no regression > 10%
**Constraints**: Zero-downtime deployment, backward-compatible DB schema, all existing tests must pass
**Scale/Scope**: 4 projects, ~15 NuGet packages to update, 2 Dockerfiles, 1 Bicep file, 1 CI/CD pipeline

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|---|---|---|
| I. Database Schema Compatibility | PASS | No schema changes planned; EF Core conventions will be pinned |
| II. Zero-Downtime Migration | PASS | Slot-swap deployment; no breaking DB changes |
| III. Microsoft Modernization Patterns | PASS | Following official .NET 6->10 migration path |
| IV. Test Gate (NON-NEGOTIABLE) | PASS | All existing tests must pass before merge |
| V. DI & Async I/O | PASS | Already uses DI + async; will maintain |
| VI. Built-in First | PASS | Newtonsoft replaced by System.Text.Json; NSwag replaced by built-in OpenAPI + SwaggerUI |
| VII. Official Container Images | PASS | Will use mcr.microsoft.com .NET 10 images |
| VIII. Infrastructure Alignment | PASS | Bicep netFrameworkVersion updated to v10.0 |
| IX. CI/CD Quality Gates | PASS | Pipeline SDK updated to 10.0.x |

**Gate Result**: ALL PASS. Proceeding to Phase 0.

## Project Structure

### Documentation (this feature)

```text
specs/001-net6-to-net10-migration/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output (migration dependency map)
├── quickstart.md        # Phase 1 output (validation guide)
├── contracts/           # Phase 1 output (package version contracts)
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
src/
├── ContosoUniversity.API/              # Web API + EF Core (migrated FIRST)
│   ├── Program.cs                      # Modernize endpoint routing
│   ├── ContosoUniversity.API.csproj    # TFM + NuGet updates
│   ├── Dockerfile                      # .NET 10 base images
│   └── global.json                     # SDK version
├── ContosoUniversity.WebApplication/   # Razor Pages frontend (migrated SECOND)
│   ├── Program.cs                      # Already uses modern routing
│   ├── ContosoUniversity.WebApplication.csproj
│   ├── Dockerfile                      # .NET 10 base images
│   └── global.json                     # SDK version
├── ContosoUniversity.Test/             # Unit tests (migrated THIRD)
│   └── ContosoUniversity.Test.csproj   # TFM + MSTest updates
├── ContosoUniversity.CodedUITest/      # Selenium tests (migrated FOURTH)
│   └── ContosoUniversity.CodedUITest.csproj
└── ContosoUniversity.sln

infra/
├── resources.bicep                     # netFrameworkVersion: 'v10.0'
└── main.bicep

.github/workflows/
└── resilience-pipeline.yml             # DOTNET_VERSION: '10.0.x'
```

**Structure Decision**: Existing multi-project solution structure is preserved.
No new projects or architectural changes. Migration is in-place within each project.

## Complexity Tracking

No constitution violations detected. No complexity justifications required.
