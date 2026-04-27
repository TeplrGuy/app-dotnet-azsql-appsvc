# Implementation Plan: .NET 6 to .NET 10 LTS Migration

**Branch**: `modernize/net6-to-net10` | **Date**: 2026-04-27 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-net6-to-net10-migration/spec.md`
**Assessment**: [assessment.md](../../.github/upgrades/scenarios/dotnet-version-upgrade/assessment.md)

## Summary

Upgrade all 4 Contoso University projects from .NET 6 to .NET 10 LTS in dependency order (API first, then WebApplication, then test projects). Update NuGet packages to .NET 10-compatible versions, modernize API endpoint routing, update Dockerfiles to .NET 10 MCR images, align Bicep infrastructure to `netFrameworkVersion: 'v10.0'`, and update CI/CD pipeline to use .NET 10 SDK. The assessment confirms all projects are 🟢 Low difficulty with ~17 LOC to modify.

## Technical Context

**Language/Version**: C# / .NET 6 → .NET 10 LTS (SDK 10.0.x)
**Primary Dependencies**: ASP.NET Core 10, EF Core 10.0.7, Microsoft.Data.SqlClient 7.0.1, Azure.Identity 1.21.0, MSTest 2.2.10, Selenium WebDriver 4.1.1
**Storage**: Azure SQL Database (existing schema preserved, EF Core code-first with no pending migrations)
**Testing**: MSTest (`dotnet test`), Selenium WebDriver (CodedUITest)
**Target Platform**: Linux containers on Azure App Service
**Project Type**: Web application (Razor Pages frontend + REST API backend)
**Performance Goals**: Page loads < 2s p95; API responses < 500ms p95; 50-500 concurrent users
**Constraints**: Zero database schema changes; zero downtime deployment via slot swap; all existing tests must pass
**Scale/Scope**: 4 projects, 95 code files, 4,290 LOC, 23 NuGet packages

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Database Schema Preservation | ✅ PASS | EF Core upgraded to 10.0.7; no new migrations; schema unchanged |
| II. Microsoft Modernization Patterns | ✅ PASS | Following official .NET upgrade path; incremental project-by-project approach |
| III. Test-Gated Changes | ✅ PASS | All existing unit tests must pass after each project upgrade |
| IV. Async-First with DI | ✅ PASS | Existing async/DI patterns preserved; endpoint routing modernization maintains DI |
| V. Built-in Over Third-Party | ✅ PASS | Removing Microsoft.AspNetCore.Razor.Language (now in framework), System.Runtime.Extensions; replacing deprecated packages |
| VI. Official Container Images | ✅ PASS | Updating to mcr.microsoft.com/dotnet/aspnet:10.0 and sdk:10.0 |
| VII. Infrastructure-Application Alignment | ✅ PASS | Bicep netFrameworkVersion updated to v10.0 in same PR |

**Gate Result**: ALL PASS — proceeding to Phase 0.

## Project Structure

### Documentation (this feature)

```text
specs/001-net6-to-net10-migration/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (API endpoints)
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
src/
├── ContosoUniversity.sln
├── ContosoUniversity.API/              # REST API + EF Core data layer
│   ├── ContosoUniversity.API.csproj    # net6.0 → net10.0
│   ├── Program.cs                      # Modernize endpoint routing
│   ├── Dockerfile                      # aspnet:6.0 → aspnet:10.0
│   ├── global.json                     # SDK 6.0.300 → 10.0.x
│   ├── Controllers/
│   ├── Data/
│   └── Models/
├── ContosoUniversity.WebApplication/   # Razor Pages frontend
│   ├── ContosoUniversity.WebApplication.csproj  # net6.0 → net10.0
│   ├── Program.cs
│   ├── Dockerfile                      # aspnet:6.0 → aspnet:10.0
│   ├── global.json                     # SDK 6.0.300 → 10.0.x
│   └── Pages/
├── ContosoUniversity.Test/             # Unit tests (MSTest)
│   └── ContosoUniversity.Test.csproj   # net6.0 → net10.0
└── ContosoUniversity.CodedUITest/      # Selenium UI tests
    └── ContosoUniversity.CodedUITest.csproj  # net6.0 → net10.0

infra/
├── resources.bicep                     # netFrameworkVersion: v6.0 → v10.0

.github/workflows/
└── resilience-pipeline.yml             # DOTNET_VERSION: '6.0.x' → '10.0.x'

docker-compose.yml                      # Image tags: 6.0 → 10.0
```

**Structure Decision**: Existing multi-project solution structure is preserved. No new projects or directories are created. Changes are in-place upgrades to existing files.

## Complexity Tracking

> No violations. All constitution gates pass.

*No complexity violations needed for this migration.*

## Post-Design Constitution Re-Check

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Database Schema Preservation | ✅ PASS | data-model.md confirms all entities unchanged; EF schema validation step in quickstart |
| II. Microsoft Modernization Patterns | ✅ PASS | research.md decisions follow official upgrade guidance |
| III. Test-Gated Changes | ✅ PASS | quickstart.md documents test execution; upgrade order ensures testability at each step |
| IV. Async-First with DI | ✅ PASS | No changes to async patterns; DI registration preserved |
| V. Built-in Over Third-Party | ✅ PASS | Removing framework-included packages; replacing deprecated telemetry SDK |
| VI. Official Container Images | ✅ PASS | Dockerfile targets use official MCR .NET 10 images |
| VII. Infrastructure-Application Alignment | ✅ PASS | Bicep, CI/CD, and Dockerfiles updated in same change set |

**Gate Result**: ALL PASS — plan ready for task generation.

## Implementation Phases

### Phase 1: API Project Upgrade

**Scope**: `src/ContosoUniversity.API/`

1. Update `global.json` SDK version to 10.0.203 with `latestPatch` rollForward
2. Change TFM in `.csproj` from `net6.0` to `net10.0`
3. Upgrade NuGet packages:
   - Microsoft.EntityFrameworkCore.* → 10.0.7
   - Microsoft.Data.SqlClient → 7.0.1
   - Azure.Identity → 1.21.0
   - Microsoft.AspNetCore.Mvc.NewtonsoftJson → 10.0.7
   - NSwag.AspNetCore → 14.x (latest compatible)
   - Microsoft.VisualStudio.Web.CodeGeneration.Design → 10.0.2
4. Remove: Microsoft.AspNetCore.Razor.Language, System.Runtime.Extensions
5. Modernize `Program.cs`: Replace `app.UseEndpoints(...)` with `app.MapControllers()`
6. Fix TimeSpan.FromSeconds ambiguity (if present in this project)
7. Update `Dockerfile` base images to `aspnet:10.0` / `sdk:10.0`
8. Build and verify: `dotnet build --configuration Release`

### Phase 2: WebApplication Project Upgrade

**Scope**: `src/ContosoUniversity.WebApplication/`

1. Update `global.json` SDK version to 10.0.203 with `latestPatch` rollForward
2. Change TFM in `.csproj` from `net6.0` to `net10.0`
3. Upgrade NuGet packages:
   - Microsoft.Extensions.DependencyInjection → 10.0.7
   - Microsoft.Extensions.DependencyInjection.Abstractions → 10.0.7
   - Newtonsoft.Json → 13.0.4 (patch only)
   - Microsoft.VisualStudio.Web.CodeGeneration.Design → 10.0.2
4. Remove: Microsoft.AspNetCore.Razor.Language
5. Replace deprecated `Microsoft.ApplicationInsights.AspNetCore` with `Azure.Monitor.OpenTelemetry.AspNetCore`
6. Update `Program.cs` telemetry registration
7. Fix TimeSpan.FromSeconds ambiguity (1 occurrence)
8. Update `Dockerfile` base images to `aspnet:10.0` / `sdk:10.0`
9. Build and verify: `dotnet build --configuration Release`

### Phase 3: Test Projects Upgrade

**Scope**: `src/ContosoUniversity.Test/` and `src/ContosoUniversity.CodedUITest/`

1. Change TFM in both `.csproj` from `net6.0` to `net10.0`
2. Upgrade MSTest packages to latest .NET 10-compatible versions
3. Fix TimeSpan.FromSeconds ambiguity (5 occurrences in CodedUITest)
4. Build and run tests: `dotnet test --configuration Release`

### Phase 4: Infrastructure and CI/CD

**Scope**: `infra/`, `.github/workflows/`, `docker-compose.yml`

1. Update `infra/resources.bicep`: Change all `netFrameworkVersion: 'v6.0'` to `'v10.0'` (4 occurrences)
2. Update `.github/workflows/resilience-pipeline.yml`: Change `DOTNET_VERSION: '6.0.x'` to `'10.0.x'`
3. Verify docker-compose.yml build contexts work with new Dockerfiles
4. Full solution build + test pass: `dotnet build && dotnet test`

### Phase 5: Validation

1. Run full solution build in Release mode
2. Run all unit tests
3. Docker compose build and verify all containers start
4. EF Core schema parity check
5. Smoke test health endpoints

## Generated Artifacts

| Artifact | Path | Purpose |
|----------|------|---------|
| Research | [research.md](research.md) | Technical decisions and rationale |
| Data Model | [data-model.md](data-model.md) | Entity schema preservation proof |
| API Contracts | [contracts/api-endpoints.md](contracts/api-endpoints.md) | Endpoint contract freeze |
| Quickstart | [quickstart.md](quickstart.md) | Post-migration build/run/test guide |
