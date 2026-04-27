# Infrastructure and Migration Targets Research

## Research Topics

1. Infrastructure runtime version settings (Bicep files)
2. CI/CD .NET SDK version pinning (GitHub Actions workflows)
3. Build configuration files (EditorConfig, Directory.Build.props)
4. Load test version-specific settings
5. Application settings (appsettings.json files)
6. Solution structure and project references
7. Existing constitution content (.specify/memory/constitution.md)
8. Latest stable .NET version as of April 2026

## Status: Complete

---

## 1. Infrastructure Runtime Version Settings (Bicep)

### infra/resources.bicep -- .NET Version References

The Bicep infrastructure hardcodes `netFrameworkVersion: 'v6.0'` in **five** App Service resource definitions:

| Resource | Line Context | Setting |
|---|---|---|
| `webApp` (MVC Frontend) | `siteConfig` block | `netFrameworkVersion: 'v6.0'` |
| `apiApp` (Web API Backend) | `siteConfig` block | `netFrameworkVersion: 'v6.0'` |
| `apiStagingSlot` (API Staging) | `siteConfig` block | `netFrameworkVersion: 'v6.0'` |
| `apiQaSlot` (API QA) | `siteConfig` block | `netFrameworkVersion: 'v6.0'` |
| Web App slots (staging/qa) | Inherit from parent (no explicit override) | Inherits parent |

**Key observations:**

- App Service Plan uses `sku: 'S1'` (Standard tier), `reserved: false` (Windows).
- Uses `linuxFxVersion` is NOT set; these are Windows App Service deployments.
- The plan is configured for Windows (`reserved: false`).
- All slots use `minTlsVersion: '1.2'`, `ftpsState: 'Disabled'`, `alwaysOn: true`.
- API apps use VNet integration (`virtualNetworkSubnetId` set).
- Key Vault references are used for connection strings (no inline secrets).
- Managed Identity (`SystemAssigned`) is enabled on all app resources.

### infra/main.bicep -- Parameters

- `targetScope = 'subscription'` -- deploys at subscription level, creates resource group.
- No .NET version parameter exposed; the version is hardcoded in resources.bicep.
- Supports two SQL auth modes: `sql` and `aad` (Azure AD-only).
- SRE Agent parameters included (`enableSreAgent`, `sreAgentMode`).
- Resource naming: `rg-${environmentName}`, `${environmentName}-app`, `${environmentName}-api`.

**Migration impact:** The `netFrameworkVersion` values in resources.bicep need to be updated from `'v6.0'` to match the target .NET version in all five locations.

---

## 2. CI/CD .NET SDK Version Pinning

### .github/workflows/resilience-pipeline.yml

- **Environment variable:** `DOTNET_VERSION: '6.0.x'` pinned at workflow level.
- Uses `actions/setup-dotnet@v4` with `dotnet-version: ${{ env.DOTNET_VERSION }}`.
- Builds solution: `dotnet build src/ContosoUniversity.sln --configuration Release`.
- Tests: `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj`.
- Publishes both WebApplication and API projects separately.
- Deployment uses `azure/webapps-deploy@v3` for slot deployments (qa, staging).
- Load testing uses `azure/load-testing@v1`.

### .github/workflows/load-test.yml

- No .NET SDK setup; this workflow only runs load tests (JMeter via Azure Load Testing).
- No version-specific settings.

### .github/workflows/infrastructure.yml

- No .NET SDK setup; this workflow deploys Bicep templates only.
- Uses `az deployment sub create` for infrastructure.
- References `infra/main.bicep` for deployment.

### .github/workflows/chaos-experiment.yml

- No .NET SDK setup; this workflow runs chaos experiments only.
- No version-specific settings.

**Migration impact:** Only `resilience-pipeline.yml` has the `DOTNET_VERSION: '6.0.x'` env variable that needs updating.

---

## 3. Build Configuration Files

### .editorconfig

**Not found.** No `.editorconfig` file exists at the repository root or under `src/`.

### Directory.Build.props

**Not found.** No `Directory.Build.props` file exists at the repository root or under `src/`.

### global.json (Two Files)

Both `src/ContosoUniversity.WebApplication/global.json` and `src/ContosoUniversity.API/global.json` contain identical content:

```json
{
  "sdk": {
    "version": "6.0.300",
    "rollForward": "latestFeature"
  }
}
```

**Migration impact:** Both `global.json` files need updating to the target SDK version. The `rollForward: "latestFeature"` policy allows using newer patch versions within the same feature band but won't cross major versions.

---

## 4. Dockerfiles and Container Configuration

### src/ContosoUniversity.WebApplication/Dockerfile

- Base image: `mcr.microsoft.com/dotnet/aspnet:6.0`
- Build image: `mcr.microsoft.com/dotnet/sdk:6.0`
- Multi-stage build (base, build, publish, final).
- Exposes port 80.

### src/ContosoUniversity.API/Dockerfile

- Base image: `mcr.microsoft.com/dotnet/aspnet:6.0`
- Build image: `mcr.microsoft.com/dotnet/sdk:6.0`
- Multi-stage build (base, build, publish, final).
- Exposes port 80.

### docker-compose.yml

- Uses `mcr.microsoft.com/azure-sql-edge:latest` for local SQL.
- References both Dockerfiles for API and WebApp builds.
- Hardcoded SA password (`YourStrong!Passw0rd`) -- local dev only.
- No .NET version references in compose file itself.

**Migration impact:** Both Dockerfiles need the image tags updated from `:6.0` to the target version.

---

## 5. Load Test Settings

### loadtests/config.yaml

- Version: `v0.1`
- Test plan: `templates/http-test.jmx`
- No .NET version-specific settings.
- Failure criteria: avg response < 5s, error rate < 1%, p95 < 10s, p99 < 35s.
- Auto-stop at 10% error rate.

### loadtests/manifest.yaml

- Version: `"1.0"`
- Defines profiles: smoke (5 users, 60s), load (50 users, 300s), stress (200 users, 600s), chaos (30 users, 900s).
- Three test definitions: instructor-pagination, homepage-loadtime, about-page.
- Default criteria: avg response < 5000ms, p95 < 6000ms.
- No .NET version-specific settings.

**Migration impact:** None. Load test configurations are version-agnostic.

---

## 6. Application Settings

### src/ContosoUniversity.WebApplication/appsettings.json

- Logging: Warning level.
- ApplicationInsights: Empty ConnectionString placeholder.
- Api Address: `https://localhost:44306/` (local dev).
- Infos: Ambiente=LocalHost, Versao=0.0.0.0.

### src/ContosoUniversity.WebApplication/appsettings.Development.json

- Logging overrides: Debug default, Information for System/Microsoft.

### src/ContosoUniversity.API/appsettings.json

- Logging: Warning level.
- ConnectionStrings: `ContosoUniversityAPIContext` pointing to local SQL Express (`LEADRO-2021\\SQLEXPRESS`).
- ApplicationInsights: Hardcoded InstrumentationKey (legacy format).

### src/ContosoUniversity.API/appsettings.Development.json

- Logging overrides: Debug default, Information for System/Microsoft.

**Migration impact:** No version-specific settings, but note the API has a hardcoded instrumentation key that should use connection string format (already an issue independent of migration).

---

## 7. Solution Structure

### src/ContosoUniversity.sln

Four projects in the solution (all use `{9A19103F-16F7-4668-BE54-9A1E7A4F7556}` GUID = SDK-style C# projects):

| Project | Path | Type |
|---|---|---|
| ContosoUniversity.WebApplication | `ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj` | MVC Frontend |
| ContosoUniversity.API | `ContosoUniversity.API/ContosoUniversity.API.csproj` | Web API Backend |
| ContosoUniversity.CodedUITest | `ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj` | Coded UI Test (legacy) |
| ContosoUniversity.Test | `ContosoUniversity.Test/ContosoUniversity.Test.csproj` | Unit Tests |

- Visual Studio Version 17 (VS 2022).
- Solution folder `infra` references legacy Bicep files (ContainerRegistry, KeyVault, Kubernetes, SQLServer).
- Debug and Release configurations for all projects.

**Migration impact:** All four `.csproj` files need `TargetFramework` updated. The CodedUITest project may have special considerations (Coded UI Tests are deprecated).

---

## 8. Existing Constitution Content

### .specify/memory/constitution.md

The file exists but contains only a **template with placeholder markers** (e.g., `[PROJECT_NAME]`, `[PRINCIPLE_1_NAME]`). No actual project-specific constitution content has been filled in. It is a blank template ready to be populated.

---

## 9. Latest Stable .NET Version (April 2026)

Source: https://dotnet.microsoft.com/en-us/download/dotnet and https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview

### Current .NET Release Landscape

| Version | Support Type | Status | Latest Patch | End of Support |
|---|---|---|---|---|
| **.NET 11.0** | STS (Standard Term) | **Preview** (11.0.0-preview.3) | April 14, 2026 | TBD |
| **.NET 10.0** | **LTS (Long Term)** | **Active (Latest Stable)** | **10.0.7** | **November 14, 2028** |
| .NET 9.0 | STS | Active | 9.0.15 | November 10, 2026 |
| .NET 8.0 | LTS | Active | 8.0.26 | November 10, 2026 |
| .NET 6.0 | LTS | **Out of Support** | - | November 12, 2024 |

### Key Facts

- **.NET 10.0 is the latest stable LTS release** as of April 2026 (released November 2025).
- .NET 10 includes: C# 14, EF Core 10, ASP.NET Core 10, runtime improvements (JIT, NativeAOT), post-quantum cryptography support.
- .NET 10 is supported until **November 14, 2028** (3-year LTS).
- .NET 6.0 went **end-of-support on November 12, 2024** -- the current project is running on an unsupported runtime.
- .NET 8.0 is still supported but reaches end-of-support November 10, 2026 (only ~7 months away).
- .NET 9.0 is STS and also reaches end-of-support November 10, 2026.
- The dotnet.microsoft.com site itself runs on .NET 10.0.7.

### Recommended Migration Target

**Target: .NET 10.0 (LTS)** -- provides the longest support window (until Nov 2028) and is the latest stable production-ready version. Skipping .NET 8.0 and 9.0 is recommended since both have shorter remaining support.

---

## Complete .NET 6.0 Version Reference Inventory

All files containing `.NET 6.0` version references that require updating during migration:

| File | Setting/Value | Count |
|---|---|---|
| `infra/resources.bicep` | `netFrameworkVersion: 'v6.0'` | 5 occurrences |
| `.github/workflows/resilience-pipeline.yml` | `DOTNET_VERSION: '6.0.x'` | 1 occurrence |
| `src/ContosoUniversity.WebApplication/global.json` | `"version": "6.0.300"` | 1 occurrence |
| `src/ContosoUniversity.API/global.json` | `"version": "6.0.300"` | 1 occurrence |
| `src/ContosoUniversity.WebApplication/Dockerfile` | `aspnet:6.0`, `sdk:6.0` | 2 occurrences |
| `src/ContosoUniversity.API/Dockerfile` | `aspnet:6.0`, `sdk:6.0` | 2 occurrences |
| `src/ContosoUniversity.WebApplication/*.csproj` | `TargetFramework` (not read yet) | TBD |
| `src/ContosoUniversity.API/*.csproj` | `TargetFramework` (not read yet) | TBD |
| `src/ContosoUniversity.Test/*.csproj` | `TargetFramework` (not read yet) | TBD |
| `src/ContosoUniversity.CodedUITest/*.csproj` | `TargetFramework` (not read yet) | TBD |

**Total known version references: 12+** (excluding csproj files not yet read)

---

## Follow-on Questions

1. What are the exact `TargetFramework` values in each `.csproj` file? (Not read in this research session)
2. What NuGet packages are referenced and do they have .NET 10 compatible versions?
3. Are there any breaking changes between .NET 6 and .NET 10 that affect EF Core, ASP.NET Core MVC, or Web API patterns used in this project?
4. Does the CodedUITest project need to be migrated or can it be dropped (Coded UI Tests were deprecated)?
5. What Bicep API version updates are needed when changing `netFrameworkVersion`?
