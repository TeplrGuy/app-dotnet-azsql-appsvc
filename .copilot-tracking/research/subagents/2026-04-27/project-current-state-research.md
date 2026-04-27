# Project Current State Research: Contoso University .NET Application

## Research Topics

1. All .csproj files -- current .NET version, target frameworks, and NuGet packages
2. Program.cs files for WebApplication and API -- hosting model
3. Dockerfiles for both projects
4. Solution file structure
5. global.json files
6. copilot-instructions.md project context
7. AGENTS.md file
8. Spec Kit files in .specify/ directory
9. Migration considerations for .NET 6 to .NET 9

---

## 1. Project Files (.csproj) Analysis

### ContosoUniversity.WebApplication.csproj

- **File**: `src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj`
- **SDK**: `Microsoft.NET.Sdk.Web`
- **Target Framework**: `net6.0` (line 4)
- **UserSecretsId**: `ba41875f-c956-4fad-87e5-4b75230e53bc` (line 5)

**NuGet Packages** (lines 9-14):

| Package | Version | Migration Notes |
|---------|---------|-----------------|
| Microsoft.ApplicationInsights.AspNetCore | 2.21.0 | Update to latest (2.22.x+) |
| Microsoft.AspNetCore.Razor.Language | 6.0.15 | Remove -- bundled in SDK for .NET 9 |
| Microsoft.Extensions.DependencyInjection | 6.0.0 | Update to 9.0.x |
| Microsoft.Extensions.DependencyInjection.Abstractions | 6.0.0 | Update to 9.0.x |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 6.0.4 | Update to 9.0.x |
| Newtonsoft.Json | 13.0.3 | Consider migrating to System.Text.Json |

### ContosoUniversity.API.csproj

- **File**: `src/ContosoUniversity.API/ContosoUniversity.API.csproj`
- **SDK**: `Microsoft.NET.Sdk.Web`
- **Target Framework**: `net6.0` (line 4)
- **UserSecretsId**: `008cb285-7653-4d63-afcc-02830f4a6eb2` (line 5)

**NuGet Packages** (lines 9-24):

| Package | Version | Migration Notes |
|---------|---------|-----------------|
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.2.2 | Update to latest stable |
| Azure.Identity | 1.8.2 | Update to latest (1.12.x+) |
| Bogus | 34.0.2 | Compatible -- check latest |
| Microsoft.ApplicationInsights.AspNetCore | 2.21.0 | Update to latest |
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 6.0.5 | Update to 9.0.x or migrate to System.Text.Json |
| Microsoft.AspNetCore.Razor.Language | 6.0.15 | Remove -- bundled in SDK |
| Microsoft.Data.SqlClient | 5.0.1 | Update to 5.2.x+ |
| Microsoft.EntityFrameworkCore | 7.0.4 | **CRITICAL**: Update to 9.0.x -- major version jump |
| Microsoft.EntityFrameworkCore.SqlServer | 7.0.4 | **CRITICAL**: Update to 9.0.x |
| Microsoft.EntityFrameworkCore.Tools | 7.0.4 | **CRITICAL**: Update to 9.0.x |
| Microsoft.Extensions.Azure | 1.6.3 | Update to latest |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 6.0.4 | Update to 9.0.x |
| NSwag.AspNetCore | 13.18.2 | Update to 14.x+ for .NET 9 compat |
| System.Runtime.Extensions | 4.3.1 | Remove -- included in .NET 9 runtime |

**NOTE**: Mixed versioning detected -- project targets `net6.0` but uses EF Core 7.0.4 packages. This is a pre-existing version mismatch.

### ContosoUniversity.Test.csproj

- **File**: `src/ContosoUniversity.Test/ContosoUniversity.Test.csproj`
- **SDK**: `Microsoft.NET.Sdk`
- **Target Framework**: `net6.0` (line 4)
- **IsPackable**: false (line 6)

**NuGet Packages** (lines 9-15):

| Package | Version | Migration Notes |
|---------|---------|-----------------|
| Microsoft.NET.Test.Sdk | 17.2.0 | Update to 17.11.x+ |
| MSTest.TestAdapter | 2.2.10 | Update to 3.x |
| MSTest.TestFramework | 2.2.10 | Update to 3.x |
| coverlet.collector | 3.1.2 | Update to 6.x+ |

### ContosoUniversity.CodedUITest.csproj

- **File**: `src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj`
- **SDK**: `Microsoft.NET.Sdk`
- **Target Framework**: `net6.0` (line 4)
- **IsPackable**: false (line 6)

**NuGet Packages** (lines 9-13):

| Package | Version | Migration Notes |
|---------|---------|-----------------|
| Microsoft.NET.Test.Sdk | 17.2.0 | Update to 17.11.x+ |
| MSTest.TestAdapter | 2.2.10 | Update to 3.x |
| MSTest.TestFramework | 2.2.10 | Update to 3.x |
| Selenium.WebDriver | 4.1.1 | Update to 4.25.x+ |
| Selenium.WebDriver.ChromeDriver | 101.0.4951.4100 | Update to latest matching Chrome version |

---

## 2. Program.cs Hosting Model Analysis

### WebApplication Program.cs

- **File**: `src/ContosoUniversity.WebApplication/Program.cs`
- **Hosting Model**: Minimal hosting (top-level statements) -- .NET 6 style
- **Pattern**: `WebApplication.CreateBuilder(args)` (line 7)
- **Key Services**:
  - CookiePolicy configuration (lines 9-13)
  - HttpClient factory with named client "client" for API communication (lines 15-23)
  - Razor Pages (line 25)
  - Health checks (line 28)
  - Application Insights telemetry with conditional connection string (lines 30-36)
- **Middleware Pipeline**:
  - ExceptionHandler, HSTS, HTTPS Redirect, StaticFiles, Routing, Authorization
  - Health check endpoint at `/health` (line 49)
  - Root redirect `/` to `/Index` (line 52)
  - MapRazorPages (line 55)
- **Migration Impact**: Already uses minimal hosting -- compatible with .NET 9. No `Startup.cs` to migrate.

### API Program.cs

- **File**: `src/ContosoUniversity.API/Program.cs`
- **Hosting Model**: Minimal hosting (top-level statements) -- .NET 6 style
- **Pattern**: `WebApplication.CreateBuilder(args)` (line 12)
- **Key Services**:
  - Azure.Identity imported (line 1) but not directly used for managed identity in code
  - EF Core DbContext with SQL Server and retry-on-failure (lines 18-21)
  - Controllers (line 23)
  - Application Insights telemetry (line 24)
- **Database Initialization**: Automatic via `DbInitializer.Initialize(db)` at startup (lines 28-31)
- **Middleware Pipeline**:
  - HTTPS Redirect, Routing, Authorization
  - **LEGACY pattern**: Uses `app.UseEndpoints(endpoints => ...)` (lines 42-45) instead of `app.MapControllers()` directly
  - NSwag/Swagger: `app.UseOpenApi()` and `app.UseSwaggerUi3()` (lines 48-49)
- **Migration Impact**:
  - Replace `UseEndpoints` with direct `MapControllers()` call
  - NSwag 13.x must be updated for .NET 9 compatibility
  - Consider switching to built-in OpenAPI support in .NET 9

---

## 3. Dockerfiles

### WebApplication Dockerfile

- **File**: `src/ContosoUniversity.WebApplication/Dockerfile`
- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:6.0` (line 1)
- **Build Image**: `mcr.microsoft.com/dotnet/sdk:6.0` (line 4)
- **Exposed Port**: 80
- **Build Pattern**: Multi-stage (restore, build, publish, final)
- **Entry Point**: `dotnet ContosoUniversity.WebApplication.dll`
- **Migration Required**: Update both base images from `6.0` to `9.0`

### API Dockerfile

- **File**: `src/ContosoUniversity.API/Dockerfile`
- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:6.0` (line 1)
- **Build Image**: `mcr.microsoft.com/dotnet/sdk:6.0` (line 5)
- **Exposed Port**: 80
- **Build Pattern**: Multi-stage (restore, build, publish, final)
- **Entry Point**: `dotnet ContosoUniversity.API.dll`
- **Migration Required**: Update both base images from `6.0` to `9.0`. Consider adding `EXPOSE 8080` since .NET 8+ defaults to port 8080.

---

## 4. Solution File Structure

- **File**: `src/ContosoUniversity.sln`
- **Visual Studio Version**: 17 (VS 2022), format 12.00
- **Projects** (4 application projects + 1 solution folder):

| Project | GUID | Type |
|---------|------|------|
| ContosoUniversity.WebApplication | {76DDF82E-58A3-4122-844D-151CA2C81F9E} | Web (SDK-style) |
| ContosoUniversity.API | {DF42193F-5EFE-4FD1-9E14-3A4D4F7BB4AB} | Web (SDK-style) |
| ContosoUniversity.CodedUITest | {0FC969C1-3DC0-4A33-BF27-7196E51BFD03} | Test (SDK-style) |
| ContosoUniversity.Test | {36B19D13-F27D-461F-A21F-4EF417C05CFC} | Test (SDK-style) |
| infra (solution folder) | {66743DFB-9BD7-482D-B066-21DC764ED914} | Solution Folder |

- **Configurations**: Debug|Any CPU, Release|Any CPU
- **Solution Items**: References Bicep files (ContainerRegistry.bicep, KeyVault.bicep, Kubernetes.bicep, main.bicep, SQLServer.bicep) and parameters
- **Note**: All projects use the C# SDK-style project format (GUID `9A19103F-...`). No legacy .csproj formats.
- **No shared Data project**: The API project contains the Data layer (`Data/ContosoUniversityAPIContext.cs`); WebApplication communicates via HTTP client.

---

## 5. global.json Files

### WebApplication global.json

- **File**: `src/ContosoUniversity.WebApplication/global.json`
- **SDK Version**: `6.0.300`
- **Roll Forward**: `latestFeature`
- **Migration Required**: Update to `9.0.100` or later

### API global.json

- **File**: `src/ContosoUniversity.API/global.json`
- **SDK Version**: `6.0.300`
- **Roll Forward**: `latestFeature`
- **Migration Required**: Update to `9.0.100` or later

**NOTE**: Having global.json in individual project folders (not solution root) is unusual. Consider consolidating to a single global.json at the solution or repository root.

---

## 6. copilot-instructions.md Context

- **File**: `.github/copilot-instructions.md`
- **Project Description**: .NET 6 MVC web application (Contoso University)
- **Architecture**: Azure App Service + Azure SQL Database + Application Insights + Azure Load Testing + Azure Chaos Studio
- **Key Endpoints**: `/`, `/Students`, `/Students/Create`, `/Courses`, `/Enrollments`
- **CI/CD**: GitHub Actions with quality gates
- **Build Commands**: `dotnet build src/ContosoUniversity.sln`, `dotnet test`, `dotnet run --project src/ContosoUniversity.WebApplication`
- **Resilience Patterns**: Polly retry, circuit breaker, timeout policies, health checks, graceful degradation
- **Migration Impact**: References ".NET 6" throughout -- needs updating after migration

---

## 7. AGENTS.md Context

- **File**: `AGENTS.md`
- **Description**: Agent instructions for Contoso University application
- **Key Information**:
  - Explicitly states ".NET 6 MVC Web Application with Azure SQL backend"
  - Lists build/test/run commands
  - References EF Core patterns (`.AsNoTracking()`, N+1 query prevention)
  - Uses xUnit with FluentAssertions (but test projects actually use MSTest -- discrepancy)
  - Deployment uses GitHub Actions with QA/Staging/Production environments
- **Migration Impact**: References ".NET 6" -- needs updating after migration. Test framework reference is incorrect (says xUnit, projects use MSTest).

---

## 8. Spec Kit (.specify/) Files

### init-options.json

- **File**: `.specify/init-options.json`
- **AI Integration**: `copilot`
- **Branch Numbering**: `sequential`
- **Context File**: `.github/copilot-instructions.md`
- **Script Type**: `ps` (PowerShell)
- **Speckit Version**: `0.8.1`

### integration.json

- **File**: `.specify/integration.json`
- **Integration**: `copilot`
- **Version**: `0.8.1`

### copilot.manifest.json

- **File**: `.specify/integrations/copilot.manifest.json`
- **Integration**: `copilot`
- **Version**: `0.8.1`
- **Installed At**: `2026-04-27T16:37:05.476326+00:00`
- **Managed Files**: 18 agent and prompt files under `.github/agents/` and `.github/prompts/`
  - Agents: speckit.analyze, checklist, clarify, constitution, implement, plan, specify, tasks, taskstoissues
  - Prompts: Corresponding prompt files for each agent

### speckit.manifest.json

- **File**: `.specify/integrations/speckit.manifest.json`
- **Version**: `0.8.1`
- **Managed Files**: 9 script and template files under `.specify/`

### extensions.yml

- **File**: `.specify/extensions.yml`
- **Git Extension**: Configured with hooks for auto-commit before various phases (constitution, specify, clarify, plan, tasks)
- **Auto Execute Hooks**: `true`

### constitution.md (Memory)

- **File**: `.specify/memory/constitution.md`
- **Status**: Template only -- not yet populated with project-specific principles

---

## 9. Additional Infrastructure Context

### docker-compose.yml

- **File**: `docker-compose.yml`
- **Services**:
  - `sqlserver`: Azure SQL Edge (latest)
  - `api`: Built from `src/ContosoUniversity.API/Dockerfile`, port 5001:80
  - `webapp`: Built from `src/ContosoUniversity.WebApplication/Dockerfile`, port 5000:80
- **Migration Impact**: Docker Compose environment variables and port mappings may need update for .NET 9 default port (8080).

### Bicep Infrastructure (infra/main.bicep)

- **File**: `infra/main.bicep`
- **Resources**: Key Vault, App Service Plan, Web App, API App, SQL Server, SQL Database, Application Insights, Log Analytics, Azure Load Testing, SRE Agent
- **Auth Modes**: SQL auth or Azure AD-only
- **Scope**: Subscription-level deployment
- **Migration Impact**: App Service may need `netFrameworkVersion` property updated in Bicep if configured.

### DbContext

- **File**: `src/ContosoUniversity.API/Data/ContosoUniversityAPIContext.cs`
- **EF Core Version**: 7.0.4 (from csproj)
- **Tables**: tbl_Course, tbl_Student, tbl_Department, tbl_Instructor, tbl_StudentCourse
- **Composite Key**: StudentCourse has composite key (CourseID, StudentID)
- **Migration Impact**: EF Core 7 to 9 has breaking changes (date-only types, query behavior changes, etc.)

### appsettings.json (API)

- **File**: `src/ContosoUniversity.API/appsettings.json`
- **WARNING**: Contains hardcoded Application Insights InstrumentationKey and local SQL Server connection string. These should be environment-specific only.

---

## 10. Migration Impact Summary: .NET 6 to .NET 9

### Files Requiring Changes

| Category | File | Change Required |
|----------|------|----------------|
| **Target Framework** | `src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj` (line 4) | `net6.0` to `net9.0` |
| **Target Framework** | `src/ContosoUniversity.API/ContosoUniversity.API.csproj` (line 4) | `net6.0` to `net9.0` |
| **Target Framework** | `src/ContosoUniversity.Test/ContosoUniversity.Test.csproj` (line 4) | `net6.0` to `net9.0` |
| **Target Framework** | `src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj` (line 4) | `net6.0` to `net9.0` |
| **SDK Version** | `src/ContosoUniversity.WebApplication/global.json` (line 3) | `6.0.300` to `9.0.100`+ |
| **SDK Version** | `src/ContosoUniversity.API/global.json` (line 3) | `6.0.300` to `9.0.100`+ |
| **Docker Images** | `src/ContosoUniversity.WebApplication/Dockerfile` (lines 1, 6) | `aspnet:6.0` / `sdk:6.0` to `9.0` |
| **Docker Images** | `src/ContosoUniversity.API/Dockerfile` (lines 1, 5) | `aspnet:6.0` / `sdk:6.0` to `9.0` |
| **NuGet Packages** | All .csproj files | Update all packages to .NET 9 compatible versions |
| **API Code** | `src/ContosoUniversity.API/Program.cs` (lines 42-45) | Replace `UseEndpoints` with `MapControllers()` |
| **Documentation** | `.github/copilot-instructions.md` | Update ".NET 6" references to ".NET 9" |
| **Documentation** | `AGENTS.md` | Update ".NET 6" references to ".NET 9" |

### Critical Package Upgrades

| Package | Current | Target | Risk |
|---------|---------|--------|------|
| Microsoft.EntityFrameworkCore* | 7.0.4 | 9.0.x | HIGH -- breaking changes in EF 8 and 9 |
| NSwag.AspNetCore | 13.18.2 | 14.x+ | MEDIUM -- API generation changes |
| Microsoft.AspNetCore.Razor.Language | 6.0.15 | REMOVE | LOW -- bundled in SDK |
| System.Runtime.Extensions | 4.3.1 | REMOVE | LOW -- included in runtime |
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 6.0.5 | 9.0.x | LOW -- or migrate to System.Text.Json |

### Breaking Change Risks

1. **EF Core 7 to 9**: Multiple breaking changes across two major versions (7 to 8, 8 to 9). Key areas: query translation changes, nullable reference type defaults, date/time handling.
2. **.NET 8+ Default Port Change**: ASP.NET Core 8+ containers default to port 8080 instead of 80. Docker files and docker-compose need updating.
3. **NSwag Compatibility**: NSwag 13.x may not support .NET 9. Built-in OpenAPI support in .NET 9 is an alternative (Microsoft.AspNetCore.OpenApi).
4. **Implicit Usings**: .NET 9 SDK projects have implicit usings enabled by default. May cause namespace conflicts.
5. **MSTest v2 to v3**: MSTest 2.2.x to 3.x has some behavior changes (test discovery, lifecycle).

### Non-Code Changes

- Bicep `resources.bicep` may need App Service `netFrameworkVersion` or `linuxFxVersion` updated if it specifies runtime version.
- GitHub Actions workflows need .NET 9 SDK installation step updated.
- Docker Compose port mappings (internal port 80 to 8080).
- Load test configurations may need endpoint URL adjustments if ports change.

---

## Follow-On Questions

- [ ] Does `resources.bicep` specify a runtime version for App Service that needs updating?
- [ ] Are there any GitHub Actions workflow files that pin the .NET SDK version?
- [ ] Does the WebApplication project have any direct EF Core dependencies, or does it only communicate with the API via HTTP?
- [ ] Is there a .editorconfig or Directory.Build.props that might affect the migration?

---

## Clarifying Questions

- None at this time. All requested information was found through workspace analysis.
