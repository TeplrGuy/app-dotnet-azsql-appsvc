<!-- markdownlint-disable-file -->
# Task Research: .NET 6 to .NET 10 Modernization Demo Guide

Comprehensive phased demo guide for modernizing the Contoso University application from .NET 6 to .NET 10 LTS, using Spec Kit for structured workflow and the modernize-dotnet plugin for AI-assisted migration. Targeting Azure App Service deployment. Designed as a 30-minute live demo.

## Task Implementation Requests

* Create a phased demo guide for modernizing Contoso University from .NET 6 to .NET 10 LTS
* Use Spec Kit (/specify, /plan, /tasks, /implement) for the structured step-by-step approach
* Leverage the modernize-dotnet Copilot plugin (already installed) for AI-assisted assessment and migration
* Cover containerization and cloud readiness patterns aligned to Microsoft guidance
* Deploy the modernized application to Azure App Service
* Keep the guide executable within a 30-minute demo window

## Scope and Success Criteria

* Scope: Full .NET 6 to .NET 10 migration of 4 projects (WebApp, API, Test, CodedUITest), infrastructure updates (Bicep, Dockerfiles, CI/CD), and Azure App Service deployment
* Assumptions:
  * Presenter has the new repo (TeplrGuy/app-dotnet-azsql-appsvc) ready on main branch
  * Spec Kit 0.8.1 is installed and initialized (`specify init . --ai copilot` completed)
  * modernize-dotnet plugin v1.0.1047-preview1 is installed in VS Code
  * .NET 10 SDK is installed locally
  * Azure subscription with existing resource group is available
  * Audience: .NET developers and architects interested in AI-assisted modernization
* Success Criteria:
  * Guide covers all phases from assessment through deployment
  * Each phase has clear demo talking points and expected outcomes
  * Guide is realistic for 30 minutes (with notes on what to skip if short on time)
  * Demonstrates both Spec Kit and modernize-dotnet plugin capabilities

## Outline

1. Pre-Demo Setup (before the demo starts)
2. Phase 1: Discovery and Assessment (5 min) -- Spec Kit /specify + modernize-dotnet assessment
3. Phase 2: Planning (5 min) -- Spec Kit /plan + modernize-dotnet upgrade plan
4. Phase 3: Code Migration (10 min) -- Spec Kit /implement + modernize-dotnet task execution
5. Phase 4: Containerization and Cloud Readiness (5 min) -- Dockerfile + Bicep updates
6. Phase 5: Validation and Deployment (5 min) -- Build, test, deploy to Azure App Service

## Research Executed

### File Analysis

* src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj
  * Targets net6.0, 6 NuGet packages including Razor.Language (remove), DI 6.0.0, Newtonsoft.Json 13.0.3
* src/ContosoUniversity.API/ContosoUniversity.API.csproj
  * Targets net6.0, 14 NuGet packages including EF Core 7.0.4 (version mismatch), NSwag 13.18.2
* src/ContosoUniversity.Test/ContosoUniversity.Test.csproj
  * Targets net6.0, MSTest 2.2.10, Test.Sdk 17.2.0
* src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj
  * Targets net6.0, Selenium 4.1.1, MSTest 2.2.10
* src/ContosoUniversity.WebApplication/Program.cs
  * Minimal hosting model, Razor Pages, HttpClient factory, health checks, App Insights
* src/ContosoUniversity.API/Program.cs
  * Minimal hosting, EF Core with SQL Server, legacy UseEndpoints() pattern, NSwag/Swagger
* infra/resources.bicep
  * 5 occurrences of netFrameworkVersion: 'v6.0', Windows App Service, S1 tier
* .github/workflows/resilience-pipeline.yml
  * DOTNET_VERSION: '6.0.x' environment variable
* Both Dockerfiles: aspnet:6.0 and sdk:6.0 base images, port 80
* Both global.json: SDK 6.0.300 with latestFeature rollForward

### External Research

* https://dotnet.microsoft.com/en-us/download/dotnet
  * .NET 10.0 LTS (10.0.7) is the latest stable release, supported until Nov 2028
  * .NET 6.0 is end-of-support since Nov 2024
* https://speckit.org/
  * Spec Kit 0.8.1 installed via `uv tool install specify-cli`
  * Workflow: /constitution -> /specify -> /plan -> /tasks -> /implement
* modernize-dotnet plugin (local)
  * Version 1.0.1047-preview1 at c:\Users\gappiah\.vscode\agent-plugins\github.com\dotnet\modernize-dotnet\
  * MCP tools: get_state, initialize_scenario, start_task, complete_task, etc.
  * 9 scenarios including .NET Version Upgrade
  * 30+ built-in skills for EF, ASP.NET, MVC, etc.

### Project Conventions

* Standards referenced: .github/copilot-instructions.md, AGENTS.md
* Instructions followed: async/await for DB ops, DI for all services, retry logic with Polly

## Key Discoveries

### Project Structure

The Contoso University application is a 4-project .NET 6 solution:
- **ContosoUniversity.WebApplication** -- Razor Pages MVC frontend, communicates with API via HttpClient
- **ContosoUniversity.API** -- RESTful Web API with EF Core, Swagger/NSwag, SQL Server backend
- **ContosoUniversity.Test** -- MSTest unit tests
- **ContosoUniversity.CodedUITest** -- Selenium-based UI tests (legacy, Coded UI is deprecated)

No shared Data project exists; the API project owns the data layer. The WebApplication talks to the API over HTTP.

### Implementation Patterns

All projects already use SDK-style .csproj and minimal hosting (no Startup.cs). The API uses a legacy `UseEndpoints()` pattern that should be modernized to direct `MapControllers()`.

### Version Reference Inventory (12+ locations)

| File | Setting | Count |
|------|---------|-------|
| 4x .csproj files | TargetFramework: net6.0 | 4 |
| infra/resources.bicep | netFrameworkVersion: 'v6.0' | 5 |
| .github/workflows/resilience-pipeline.yml | DOTNET_VERSION: '6.0.x' | 1 |
| 2x global.json | SDK version: 6.0.300 | 2 |
| 2x Dockerfiles | aspnet:6.0, sdk:6.0 | 4 |
| **Total** | | **16+** |

### Migration Risk Areas

1. **EF Core 7 to 10**: Spans 3 major versions with compounding breaking changes
2. **NSwag 13 to 14+**: Major version bump, or switch to .NET built-in OpenAPI
3. **Dockerfile port change**: .NET 8+ defaults to port 8080 (not 80)
4. **CodedUITest**: Coded UI Tests are deprecated; consider dropping or converting

## Technical Scenarios

### Scenario: Phased 30-Minute Demo

The demo follows a 5-phase approach designed to fit in 30 minutes. Each phase has a primary tool (Spec Kit or modernize-dotnet) and clear deliverables.

**Requirements:**

* Audience sees the full modernization lifecycle: assess -> plan -> migrate -> validate -> deploy
* Both Spec Kit and modernize-dotnet plugin are showcased
* Each phase produces visible artifacts (specs, plans, code changes, build output)
* Presenters can cut Phase 4/5 short if running low on time

**Preferred Approach:**

Use Spec Kit for the high-level structured workflow (/specify -> /plan -> /tasks) and modernize-dotnet for the actual code migration execution. This combines the spec-driven methodology with the AI-powered migration engine.

---

## DEMO GUIDE: GitHub Copilot for Application Modernization

### Session Title
**GitHub Copilot for Application Modernization: Migrating with Confidence**

### Session Abstract
Accelerate legacy modernization with AI-assisted refactoring. This session explores how GitHub Copilot supports .NET migrations, framework upgrades, dependency updates, and architectural transformation. We'll walk through modernization patterns including containerization and cloud readiness aligned to Microsoft guidance.

### Time Budget

| Phase | Duration | Priority |
|-------|----------|----------|
| Pre-Demo Setup | Before demo | Required |
| Phase 1: Discovery & Assessment | ~5 min | Must-show |
| Phase 2: Planning | ~5 min | Must-show |
| Phase 3: Code Migration | ~10 min | Must-show |
| Phase 4: Containerization & Cloud Readiness | ~5 min | Nice-to-have |
| Phase 5: Validation & Deployment | ~5 min | Nice-to-have |
| **Total** | **~30 min** | |

> **Time pressure tip**: Phases 1-3 are the core demo (20 min). Phases 4-5 can be talked through quickly or skipped if running short.

---

### PRE-DEMO SETUP (Before the demo starts)

Complete these steps before the audience joins:

#### 1. Environment Verification

```powershell
# Verify .NET 10 SDK is installed
dotnet --list-sdks

# Verify Spec Kit CLI
specify --version
# Expected: 0.8.1

# Verify modernize-dotnet plugin is loaded in VS Code
# Check: Extensions sidebar -> search "modernize" or check agent-plugins
```

#### 2. Repository Setup

```powershell
# Clone the fresh repo (or reset to the starting state)
git clone https://github.com/TeplrGuy/app-dotnet-azsql-appsvc.git
cd app-dotnet-azsql-appsvc

# Verify it builds on .NET 6
dotnet build src/ContosoUniversity.sln
```

#### 3. VS Code Configuration

- Open the project in VS Code
- Ensure GitHub Copilot is active (check status bar)
- Have the Copilot Chat panel open
- Have the terminal panel visible
- Optionally: have the Azure portal open in a browser tab (App Service resource)

#### 4. Pre-create a Branch (optional, for clean demo)

```powershell
git checkout -b modernize/net6-to-net10
```

---

### PHASE 1: DISCOVERY AND ASSESSMENT (~5 minutes)

**Goal**: Show the audience the current state of the app and what needs to change.

**Talking Points**:
- "This is a real .NET 6 Contoso University app running on an UNSUPPORTED runtime since Nov 2024"
- "Let's use AI to assess what needs to change -- no manual file hunting"
- "We'll use two tools: Spec Kit for structured workflow and the modernize-dotnet plugin for deep analysis"

#### Step 1A: Spec Kit -- Define What We're Building

Open Copilot Chat and run:

```
/specify Modernize the Contoso University .NET 6 application to .NET 10 LTS. 
The app has 4 projects: a Razor Pages frontend, a Web API with EF Core and SQL Server, 
unit tests (MSTest), and Selenium UI tests. Currently deployed to Azure App Service (Windows). 
Need to update all target frameworks, NuGet packages, Dockerfiles, Bicep infrastructure, 
and CI/CD pipeline. Must maintain backward compatibility with existing Azure SQL database.
```

> **Demo Note**: This creates the specification artifact in `.specify/`. Show the audience the generated `spec.md` file briefly.

#### Step 1B: modernize-dotnet -- Run Assessment

Switch to the modernize-dotnet agent in Copilot Chat (type `@modernize-dotnet` or use the agent picker):

```
Assess this solution for upgrading from .NET 6 to .NET 10. 
Show me all the changes that would be needed.
```

> **What happens behind the scenes**:
> 1. Plugin calls `get_state()` to check existing scenarios
> 2. Calls `get_scenarios()` to list available scenarios
> 3. Loads scenario instructions via `get_instructions(kind='scenario', query='.NET Version Upgrade')`
> 4. Calls `initialize_scenario` for .NET Version Upgrade
> 5. Runs `generate_dotnet_upgrade_assessment` to analyze all 4 projects
> 6. Creates assessment artifacts in `.github/upgrades/{scenarioId}/`

**What to show the audience**:
- The assessment report listing all projects and their upgrade requirements
- The NuGet package compatibility analysis
- The breaking changes detected (EF Core version jump, NSwag, port changes)
- **Key stat**: "16+ version references across 12 files need updating"

**Talking Point**: "In under a minute, Copilot identified every file, every package, and every breaking change. This would take a developer hours to compile manually."

---

### PHASE 2: PLANNING (~5 minutes)

**Goal**: Show structured planning with both tools generating an actionable migration plan.

**Talking Points**:
- "Now we know WHAT needs to change. Let's plan HOW to do it safely."
- "The modernize-dotnet plugin creates a task-by-task execution plan in dependency order"
- "Spec Kit gives us the architectural view"

#### Step 2A: Spec Kit -- Create the Plan

```
/plan Upgrade all 4 projects from net6.0 to net10.0 in dependency order.
Start with the API project (has the data layer), then WebApplication, then tests.
Use the modernize-dotnet plugin assessment results. Update NuGet packages to .NET 10 compatible versions.
Modernize the API to use endpoint routing instead of UseEndpoints().
Update Dockerfiles to .NET 10 images.
Update Bicep infrastructure to netFrameworkVersion v10.0.
Update CI/CD pipeline to .NET 10 SDK.
```

> **Demo Note**: Show the generated `plan.md` -- the structured phases and task breakdown.

#### Step 2B: Generate Tasks

```
/tasks
```

> **Demo Note**: Show the generated `tasks.md` with the ordered task list. This gives the audience the "flight plan" for the migration.

#### Step 2C: modernize-dotnet -- View Upgrade Plan

In the modernize-dotnet chat, the plugin should have already generated a plan. Show:
- The `tasks.md` file in `.github/upgrades/{scenarioId}/`
- The ordered list of tasks (project upgrades in topological order)
- How each task has specific files and changes identified

**Talking Point**: "We now have two complementary plans -- Spec Kit gives us the architecture view, modernize-dotnet gives us the file-level execution plan. Let's execute."

---

### PHASE 3: CODE MIGRATION (~10 minutes)

**Goal**: This is the main event. Show Copilot actually upgrading the code.

**Talking Points**:
- "This is where AI really shines -- executing repetitive but error-prone changes across dozens of files"
- "The modernize-dotnet plugin handles each project in dependency order"
- "Watch how it updates target frameworks, packages, and code patterns simultaneously"

#### Step 3A: Start Implementation

Option A -- Use modernize-dotnet (recommended for demo impact):

```
@modernize-dotnet Start upgrading the projects. Use automatic mode.
```

> The plugin will:
> 1. `start_task()` for the first task (likely API project -- has dependencies)
> 2. Update `TargetFramework` from `net6.0` to `net10.0`
> 3. Update NuGet packages to .NET 10 compatible versions
> 4. Apply code fixes for breaking changes
> 5. `complete_task()` and move to next project

Option B -- Use Spec Kit /implement (alternative):

```
/implement Start with the API project. Update TargetFramework to net10.0 and all NuGet packages.
```

#### Step 3B: Key Changes to Highlight During Migration

As the migration runs, point out these changes to the audience:

**1. Target Framework Update** (all .csproj files):
```xml
<!-- Before -->
<TargetFramework>net6.0</TargetFramework>

<!-- After -->
<TargetFramework>net10.0</TargetFramework>
```

**2. EF Core Jump** (API .csproj -- most impactful change):
```xml
<!-- Before: Mixed versions! -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.4" />

<!-- After: Aligned to .NET 10 -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.x" />
```

**3. API Endpoint Routing Modernization** (API Program.cs):
```csharp
// Before (.NET 6 legacy pattern):
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// After (.NET 10 modern pattern):
app.MapControllers();
```

**4. NSwag to Built-in OpenAPI** (API Program.cs -- optional):
```csharp
// Before:
app.UseOpenApi();
app.UseSwaggerUi3();

// After (.NET 10 built-in):
app.MapOpenApi();
```

**5. global.json Update**:
```json
// Before:
{ "sdk": { "version": "6.0.300" } }

// After:
{ "sdk": { "version": "10.0.100" } }
```

#### Step 3C: Build Verification

After migration completes:

```powershell
# Build the solution to verify
dotnet build src/ContosoUniversity.sln --configuration Release

# Run tests
dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj
```

**Talking Point**: "Clean build on the first try. The AI handled 16+ version references, 30+ NuGet package updates, and API pattern modernization -- all coordinated across 4 projects."

> **If build fails**: This is actually a GREAT demo moment. Show how Copilot can diagnose and fix the error. Use `@modernize-dotnet` or `/analyze-and-fix-error` prompt.

---

### PHASE 4: CONTAINERIZATION AND CLOUD READINESS (~5 minutes)

**Goal**: Show the infrastructure-as-code updates needed for Azure deployment.

**Talking Points**:
- "Code is migrated, but the deployment pipeline still thinks we're on .NET 6"
- "We need to update Dockerfiles, Bicep infrastructure, and CI/CD"

#### Step 4A: Dockerfile Updates

Show the Dockerfile changes (may already be done by modernize-dotnet):

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

**Talking Point**: "Note the port change -- .NET 8+ defaults to 8080 instead of 80. Missing this is a common deployment failure."

#### Step 4B: Bicep Infrastructure

Ask Copilot to update the infrastructure:

```
Update infra/resources.bicep to change all netFrameworkVersion values from 'v6.0' to 'v10.0'. 
There are 5 occurrences across the webApp, apiApp, and slot resources.
```

Show the diff:
```bicep
// Before (5 places):
netFrameworkVersion: 'v6.0'

// After:
netFrameworkVersion: 'v10.0'
```

#### Step 4C: CI/CD Pipeline

```
Update .github/workflows/resilience-pipeline.yml to use .NET 10 SDK.
Change DOTNET_VERSION from '6.0.x' to '10.0.x'.
```

**Talking Point**: "Three layers of infrastructure updated: containers, cloud resources, and CI/CD. All aligned to .NET 10."

---

### PHASE 5: VALIDATION AND DEPLOYMENT (~5 minutes)

**Goal**: Prove the migration works end-to-end.

**Talking Points**:
- "Let's validate everything builds, tests pass, and we can deploy"

#### Step 5A: Full Build and Test

```powershell
# Clean build
dotnet build src/ContosoUniversity.sln --configuration Release

# Run all tests
dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj --configuration Release --verbosity normal
```

#### Step 5B: Docker Build Test (if time permits)

```powershell
docker compose build
```

#### Step 5C: Commit and Push

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

#### Step 5D: Deploy to Azure App Service (if time permits)

Option A -- Via GitHub Actions:
```
Create a pull request to main, which triggers the resilience pipeline for deployment.
```

Option B -- Direct deployment:
```powershell
# Publish and deploy directly
dotnet publish src/ContosoUniversity.WebApplication -c Release -o ./publish/web
dotnet publish src/ContosoUniversity.API -c Release -o ./publish/api

# Deploy via Azure CLI
az webapp deploy --resource-group <rg> --name <web-app-name> --src-path ./publish/web
az webapp deploy --resource-group <rg> --name <api-app-name> --src-path ./publish/api
```

**Talking Point**: "In 30 minutes, we went from an unsupported .NET 6 app to a fully modernized .NET 10 application, ready for production on Azure App Service. The AI handled the tedious parts -- we focused on the architecture decisions."

---

### CLOSING TALKING POINTS

1. **Speed**: "What would take days of manual work was done in minutes with AI assistance"
2. **Confidence**: "The structured assessment -> plan -> execute -> validate approach reduces risk"
3. **Completeness**: "AI caught the EF Core version mismatch, the port change, and 16+ version references that a manual review might miss"
4. **Tooling Integration**: "Spec Kit provides the methodology, modernize-dotnet provides the migration engine, GitHub Copilot ties it all together"
5. **Microsoft Alignment**: "This follows Microsoft's recommended modernization patterns -- assess, plan, migrate, validate"

### KEY AUDIENCE TAKEAWAYS

| What | Tool | Value |
|------|------|-------|
| Structured specification | Spec Kit /specify | Clear requirements before coding |
| AI-powered assessment | modernize-dotnet | Complete impact analysis in seconds |
| Dependency-ordered planning | modernize-dotnet tasks | Safe migration sequence |
| Code migration execution | Copilot + modernize-dotnet | 30+ package updates, pattern modernization |
| Infrastructure updates | Copilot | Bicep, Docker, CI/CD alignment |
| Validation | dotnet build/test | Confidence before deployment |

---

## Potential Next Research

* Detailed .NET 10 breaking changes list for EF Core, ASP.NET Core, and MVC
  * Reasoning: Needed if specific errors arise during live demo
  * Reference: https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0
* Azure App Service .NET 10 runtime availability by region
  * Reasoning: Ensure the target Azure region supports .NET 10
  * Reference: Azure portal App Service runtime settings
* NSwag 14 vs. .NET built-in OpenAPI comparison
  * Reasoning: Demo may need to choose between upgrading NSwag or switching to built-in
  * Reference: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview

## Considered Alternatives

### Alternative 1: Incremental Migration (.NET 6 -> 8 -> 10)

Step through each major version individually. **Rejected** because:
- Takes 3x longer in a 30-minute demo
- modernize-dotnet supports direct jumps
- No business value in intermediate versions with shorter support windows

### Alternative 2: Use Only modernize-dotnet (Skip Spec Kit)

Run the entire migration through the modernize-dotnet agent alone. **Rejected** because:
- Spec Kit demonstrates the structured methodology (which is the demo's selling point)
- Audience expects to see specification-driven development
- The two tools are complementary, not competing

### Alternative 3: Use Only Spec Kit (Skip modernize-dotnet)

Drive everything through /specify -> /plan -> /implement. **Rejected** because:
- modernize-dotnet has specialized .NET migration knowledge (30+ skills)
- Assessment capabilities are unique to modernize-dotnet
- Misses the opportunity to showcase Microsoft's modernization tooling

### Alternative 4: Target .NET 9 instead of .NET 10

Use .NET 9 (STS) as the migration target. **Rejected** because:
- .NET 9 reaches end-of-support Nov 2026 (only 7 months away)
- .NET 10 LTS provides support until Nov 2028
- No benefit to targeting a shorter-lived runtime
