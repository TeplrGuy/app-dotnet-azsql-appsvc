---
title: ".NET Modernization Demo Guide"
description: "30-minute live demo guide for modernizing Contoso University from .NET 6 to .NET 10 LTS using Spec Kit and modernize-dotnet"
author: Microsoft
ms.date: 2026-04-27
ms.topic: tutorial
---

## Session: GitHub Copilot for Application Modernization

**Title**: GitHub Copilot for Application Modernization: Migrating with Confidence

**Abstract**: Accelerate legacy modernization with AI-assisted refactoring. This session explores how GitHub Copilot supports .NET migrations, framework upgrades, dependency updates, and architectural transformation. Walk through modernization patterns including containerization and cloud readiness aligned to Microsoft guidance.

**Duration**: 30 minutes

### Time Budget

| Phase                                        | Duration    | Priority     |
|----------------------------------------------|-------------|--------------|
| Pre-Demo Setup                               | Before demo | Required     |
| Phase 1: Constitution, Discovery, and Assessment | ~5 min      | Must-show    |
| Phase 2: Planning                            | ~5 min      | Must-show    |
| Phase 3: Code Migration                      | ~10 min     | Must-show    |
| Phase 4: Containerization and Cloud Readiness | ~5 min      | Nice-to-have |
| Phase 5: Validation and Deployment           | ~5 min      | Nice-to-have |
| **Total**                                    | **~30 min** |              |

> [!TIP]
> Phases 1-3 are the core demo (20 min). If running short on time, talk through Phases 4-5 at a high level or skip them entirely.

---

## Pre-Demo Setup

Complete these steps before the audience joins.

### Environment Verification

```powershell
# Verify .NET 10 SDK is installed (must show 10.0.x)
dotnet --list-sdks

# Verify Spec Kit CLI
specify --version
# Expected: 0.8.1

# Verify modernize-dotnet plugin is loaded in VS Code
# Check: Extensions sidebar -> search "modernize" or check agent-plugins directory
```

### Repository Setup

```powershell
# Clone the fresh repo (or reset to the starting state)
git clone https://github.com/TeplrGuy/app-dotnet-azsql-appsvc.git
cd app-dotnet-azsql-appsvc

# Verify it builds on .NET 6
dotnet build src/ContosoUniversity.sln
```

### VS Code Configuration

* Open the project in VS Code
* Confirm GitHub Copilot is active (check status bar icon)
* Open the Copilot Chat panel
* Open the terminal panel
* Optionally open the Azure portal in a browser tab (App Service resource)

### Branch Creation

```powershell
git checkout -b modernize/net6-to-net10
```

---

## Phase 1: Discovery and Assessment (~5 min)

**Goal**: Show the audience the current state of the app and what needs to change.

> [!NOTE]
> Three tools work in tandem here: Spec Kit provides the structured workflow (including project governance via constitution), and the modernize-dotnet plugin delivers deep migration analysis.

### Presenter Talking Points

* "This is a real .NET 6 Contoso University app running on an UNSUPPORTED runtime since Nov 2024."
* "Before we touch any code, we establish a project constitution: the guardrails that govern this migration."
* "Let's use AI to assess what needs to change, with no manual file hunting."
* "In under a minute, Copilot identified every file, every package, and every breaking change."

### Step 1A: Spec Kit /constitute

Establish the project constitution that governs the entire migration. This sets the non-negotiable constraints and architectural principles before any specification or planning begins.

Open Copilot Chat and run:

```text
/constitute This is a .NET 6 to .NET 10 LTS migration for Contoso University.
Governance constraints:
- Maintain backward compatibility with the existing Azure SQL database schema
- No data loss or migration downtime
- Follow Microsoft's recommended modernization patterns
- All changes must pass existing unit tests before merging
- Use dependency injection and async/await for all I/O
- Prefer built-in .NET 10 features over third-party alternatives where feasible
- Dockerfiles must use official Microsoft container images
- Infrastructure as Code (Bicep) must stay aligned with application target framework
- CI/CD pipeline must gate on build success and test pass before deployment
```

> [!TIP]
> The constitution creates a `.specify/constitution.md` artifact. Show the audience how it captures the rules of engagement for the migration. This is especially valuable in team settings where multiple developers contribute to the upgrade.

**Why this matters for migration**: Unlike greenfield projects, migrations carry risk of regressions and data loss. The constitution makes these constraints explicit so that every subsequent `/specify`, `/plan`, and `/implement` step respects them automatically.

### Step 1B: Spec Kit /specify

Open Copilot Chat and run:

```text
/specify Modernize the Contoso University .NET 6 application to .NET 10 LTS.
The app has 4 projects: a Razor Pages frontend, a Web API with EF Core and SQL Server,
unit tests (MSTest), and Selenium UI tests. Currently deployed to Azure App Service (Windows).
Need to update all target frameworks, NuGet packages, Dockerfiles, Bicep infrastructure,
and CI/CD pipeline. Must maintain backward compatibility with existing Azure SQL database.
```

> [!TIP]
> This creates the specification artifact in `.specify/`. Show the audience the generated `spec.md` file briefly.

### Step 1C: modernize-dotnet Assessment

Switch to the modernize-dotnet agent in Copilot Chat (type `@modernize-dotnet` or use the agent picker):

```text
Assess this solution for upgrading from .NET 6 to .NET 10.
Show me all the changes that would be needed.
```

**What happens behind the scenes**:

1. Plugin calls `get_state()` to check existing scenarios
2. Calls `get_scenarios()` to list available upgrade scenarios
3. Loads scenario instructions via `get_instructions(kind='scenario', query='.NET Version Upgrade')`
4. Calls `initialize_scenario` for the .NET Version Upgrade scenario
5. Runs `generate_dotnet_upgrade_assessment` to analyze all 4 projects
6. Creates assessment artifacts in `.github/upgrades/{scenarioId}/`

**What to show the audience**:

* The assessment report listing all projects and their upgrade requirements
* NuGet package compatibility analysis
* Breaking changes detected (EF Core version jump, NSwag, port changes)
* **Key stat**: "16+ version references across 12 files need updating"

---

## Phase 2: Planning (~5 min)

**Goal**: Generate an actionable migration plan with both tools.

### Presenter Talking Points

* "Now we know WHAT needs to change. Let's plan HOW to do it safely."
* "The modernize-dotnet plugin creates a task-by-task execution plan in dependency order."
* "We have two complementary plans: Spec Kit gives the architecture view, modernize-dotnet gives the file-level execution plan."

### Step 2A: Spec Kit /plan

```text
/plan Upgrade all 4 projects from net6.0 to net10.0 in dependency order.
Start with the API project (has the data layer), then WebApplication, then tests.
Use the modernize-dotnet plugin assessment results. Update NuGet packages to .NET 10 compatible versions.
Modernize the API to use endpoint routing instead of UseEndpoints().
Update Dockerfiles to .NET 10 images.
Update Bicep infrastructure to netFrameworkVersion v10.0.
Update CI/CD pipeline to .NET 10 SDK.
```

> [!TIP]
> Show the generated `plan.md` to the audience. Highlight the structured phases and task breakdown.

### Step 2B: Spec Kit /tasks

```text
/tasks
```

Show the generated `tasks.md` with the ordered task list. This gives the audience the "flight plan" for the migration.

### Step 2C: modernize-dotnet Plan Review

The modernize-dotnet plugin generates its own plan. Show:

* The `tasks.md` file in `.github/upgrades/{scenarioId}/`
* The ordered list of tasks (project upgrades in topological order)
* How each task identifies specific files and changes

---

## Phase 3: Code Migration (~10 min)

**Goal**: Execute the migration. This is the main event.

### Presenter Talking Points

* "This is where AI really shines: executing repetitive but error-prone changes across dozens of files."
* "The modernize-dotnet plugin handles each project in dependency order."
* "Watch how it updates target frameworks, packages, and code patterns simultaneously."

### Step 3A: Start Implementation

**Option A: modernize-dotnet (recommended for demo impact)**

```text
@modernize-dotnet Start upgrading the projects. Use automatic mode.
```

The plugin follows this sequence:

1. `start_task()` for the first project (API, which owns the data layer)
2. Updates `TargetFramework` from `net6.0` to `net10.0`
3. Updates NuGet packages to .NET 10 compatible versions
4. Applies code fixes for breaking changes
5. `complete_task()` and moves to the next project
6. Repeats for WebApplication, Test, and CodedUITest projects

**Option B: Spec Kit /implement (alternative)**

```text
/implement Start with the API project. Update TargetFramework to net10.0 and all NuGet packages.
```

### Step 3B: Key Changes to Highlight

As the migration runs, point out these changes to the audience.

#### 1. Target Framework Update (all .csproj files)

```xml
<!-- Before -->
<TargetFramework>net6.0</TargetFramework>

<!-- After -->
<TargetFramework>net10.0</TargetFramework>
```

#### 2. EF Core Version Jump (API .csproj, most impactful change)

This spans 3 major versions (7 to 10) with compounding breaking changes:

```xml
<!-- Before: EF Core 7 on a net6.0 project (version mismatch) -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.4" />

<!-- After: Aligned to .NET 10 -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.x" />
```

#### 3. API Endpoint Routing Modernization (API Program.cs)

```csharp
// Before (.NET 6 legacy pattern):
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// After (.NET 10 modern pattern):
app.MapControllers();
```

#### 4. NSwag to Built-in OpenAPI (API Program.cs, optional)

```csharp
// Before (NSwag):
app.UseOpenApi();
app.UseSwaggerUi3();

// After (.NET 10 built-in OpenAPI):
app.MapOpenApi();
```

> [!NOTE]
> This change is optional. Upgrading NSwag to v14+ is also a valid approach. Choose based on the audience's interest level.

#### 5. global.json SDK Update

```json
// Before:
{ "sdk": { "version": "6.0.300" } }

// After:
{ "sdk": { "version": "10.0.100" } }
```

### Step 3C: Build Verification

```powershell
# Build the solution to verify
dotnet build src/ContosoUniversity.sln --configuration Release

# Run tests
dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj
```

**Success talking point**: "Clean build on the first try. The AI handled 16+ version references, 30+ NuGet package updates, and API pattern modernization, all coordinated across 4 projects."

> [!TIP]
> If the build fails, this is a GREAT demo moment. Show how Copilot can diagnose and fix the error. Use `@modernize-dotnet` or a `/analyze-and-fix-error` prompt.

---

## Phase 4: Containerization and Cloud Readiness (~5 min)

> [!IMPORTANT]
> This phase is **nice-to-have**. If running short on time, talk through the changes at a high level.

**Goal**: Update all infrastructure-as-code to align with .NET 10.

### Presenter Talking Points

* "Code is migrated, but the deployment pipeline still thinks we're on .NET 6."
* "Three layers of infrastructure need updating: containers, cloud resources, and CI/CD."

### Step 4A: Dockerfile Updates

Show the Dockerfile changes (may already be applied by modernize-dotnet):

```dockerfile
# Before:
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
EXPOSE 80

# After:
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
EXPOSE 8080
```

> [!WARNING]
> The port change from 80 to 8080 is critical. Starting with .NET 8, the default port changed to 8080. Missing this is a common deployment failure.

### Step 4B: Bicep Infrastructure

Ask Copilot to update the infrastructure:

```text
Update infra/resources.bicep to change all netFrameworkVersion values from 'v6.0' to 'v10.0'.
There are 5 occurrences across the webApp, apiApp, and slot resources.
```

Show the diff:

```bicep
// Before (5 occurrences):
netFrameworkVersion: 'v6.0'

// After:
netFrameworkVersion: 'v10.0'
```

### Step 4C: CI/CD Pipeline

```text
Update .github/workflows/resilience-pipeline.yml to use .NET 10 SDK.
Change DOTNET_VERSION from '6.0.x' to '10.0.x'.
```

---

## Phase 5: Validation and Deployment (~5 min)

> [!IMPORTANT]
> This phase is **nice-to-have**. Prioritize Step 5A (build and test) if time is limited.

**Goal**: Prove the migration works end-to-end.

### Step 5A: Full Build and Test

```powershell
# Clean Release build
dotnet build src/ContosoUniversity.sln --configuration Release

# Run all tests
dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj --configuration Release --verbosity normal
```

### Step 5B: Docker Build Test (if time permits)

```powershell
docker compose build
```

### Step 5C: Commit and Push

```powershell
git add -A
git commit -m "feat: modernize .NET 6 to .NET 10 LTS

- Upgraded all 4 projects from net6.0 to net10.0
- Updated 30+ NuGet packages to .NET 10 compatible versions
- Modernized API endpoint routing (UseEndpoints -> MapControllers)
- Updated Dockerfiles to .NET 10 images (port 80 -> 8080)
- Updated Bicep infrastructure (netFrameworkVersion v10.0)
- Updated CI/CD pipeline (.NET 10 SDK)

Fixes: .NET 6 end-of-support (Nov 2024)"

git push origin modernize/net6-to-net10
```

### Step 5D: Deploy to Azure App Service (if time permits)

**Option A: Via GitHub Actions (recommended)**

Create a pull request to `main`, which triggers the resilience pipeline for deployment.

**Option B: Direct deployment**

```powershell
# Publish and deploy directly
dotnet publish src/ContosoUniversity.WebApplication -c Release -o ./publish/web
dotnet publish src/ContosoUniversity.API -c Release -o ./publish/api

# Deploy via Azure CLI
az webapp deploy --resource-group <rg> --name <web-app-name> --src-path ./publish/web
az webapp deploy --resource-group <rg> --name <api-app-name> --src-path ./publish/api
```

### Closing Talking Point

"In 30 minutes, we went from an unsupported .NET 6 app to a fully modernized .NET 10 application, ready for production on Azure App Service. The AI handled the tedious parts while we focused on the architecture decisions."

---

## Closing

### Five Key Takeaways

1. **Speed**: What would take days of manual work was completed in minutes with AI assistance.
2. **Confidence**: The structured assess, plan, execute, validate approach reduces risk at every step.
3. **Completeness**: AI caught the EF Core version mismatch, the port change, and 16+ version references that a manual review might miss.
4. **Tooling Integration**: Spec Kit provides the methodology, modernize-dotnet provides the migration engine, and GitHub Copilot ties it all together.
5. **Microsoft Alignment**: This follows Microsoft's recommended modernization patterns for assessment, planning, migration, and validation.

### Key Audience Takeaways

| What                          | Tool                      | Value                                       |
|-------------------------------|---------------------------|---------------------------------------------|
| Project governance            | Spec Kit /constitute      | Migration guardrails enforced automatically  |
| Structured specification      | Spec Kit /specify         | Clear requirements before coding            |
| AI-powered assessment         | modernize-dotnet          | Complete impact analysis in seconds         |
| Dependency-ordered planning   | modernize-dotnet tasks    | Safe migration sequence                     |
| Code migration execution      | Copilot + modernize-dotnet | 30+ package updates, pattern modernization |
| Infrastructure updates        | Copilot                   | Bicep, Docker, CI/CD alignment              |
| End-to-end validation         | dotnet build/test         | Confidence before deployment                |

### Suggested Q&A Topics

* **"What about .NET Framework (4.x) apps?"**: The modernize-dotnet plugin supports .NET Framework to modern .NET migration as a separate scenario. The same assess-plan-execute pattern applies, though the migration scope is significantly larger.

* **"Can this handle microservices or larger solutions?"**: Yes. The dependency-ordered task execution scales to multi-project solutions. For microservices, run the assessment per service boundary and plan migrations in waves.

* **"What if the app uses third-party libraries that don't support .NET 10?"**: The assessment report flags incompatible packages. Options include finding alternatives, wrapping legacy code, or targeting multi-framework builds as a bridge.

* **"How does this compare to manual migration?"**: Manual migration requires reading every breaking change doc, updating each file individually, and hoping nothing was missed. AI-assisted migration automates the discovery and execution while keeping you in control of the decisions.

* **"Is the modernize-dotnet plugin generally available?"**: Check the [.NET modernization tooling page](https://learn.microsoft.com/en-us/dotnet/core/porting/) for the latest availability status.

---

## Appendix: Version Reference Inventory

All locations in the codebase that reference .NET 6 and require updating during migration:

| File                                                        | Setting                       | Current Value | Target Value | Count |
|-------------------------------------------------------------|-------------------------------|---------------|--------------|-------|
| src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj | TargetFramework               | net6.0        | net10.0      | 1     |
| src/ContosoUniversity.API/ContosoUniversity.API.csproj      | TargetFramework               | net6.0        | net10.0      | 1     |
| src/ContosoUniversity.Test/ContosoUniversity.Test.csproj    | TargetFramework               | net6.0        | net10.0      | 1     |
| src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj | TargetFramework               | net6.0        | net10.0      | 1     |
| infra/resources.bicep                                       | netFrameworkVersion            | v6.0          | v10.0        | 5     |
| .github/workflows/resilience-pipeline.yml                   | DOTNET_VERSION                | 6.0.x         | 10.0.x       | 1     |
| src/ContosoUniversity.WebApplication/global.json            | sdk.version                   | 6.0.300       | 10.0.100     | 1     |
| src/ContosoUniversity.API/global.json                       | sdk.version                   | 6.0.300       | 10.0.100     | 1     |
| src/ContosoUniversity.WebApplication/Dockerfile             | aspnet base image             | 6.0           | 10.0         | 1     |
| src/ContosoUniversity.WebApplication/Dockerfile             | sdk build image               | 6.0           | 10.0         | 1     |
| src/ContosoUniversity.API/Dockerfile                        | aspnet base image             | 6.0           | 10.0         | 1     |
| src/ContosoUniversity.API/Dockerfile                        | sdk build image               | 6.0           | 10.0         | 1     |
| **Total**                                                   |                               |               |              | **16** |
