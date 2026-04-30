# Data Model: .NET 6 to .NET 10 Migration Dependency Map

**Feature**: 001-net6-to-net10-migration
**Date**: 2026-04-27

This document maps the migration entities, their dependencies, and the
order of operations required to complete the upgrade without breaking the build.

## Migration Dependency Graph

```text
┌─────────────────────────────────────────────────────────────┐
│                    Migration Order                           │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Step 1: global.json (SDK version)                          │
│      │                                                      │
│      ▼                                                      │
│  Step 2: ContosoUniversity.API.csproj                       │
│      │   (TFM + NuGet + data layer + EF Core)               │
│      │                                                      │
│      ▼                                                      │
│  Step 3: ContosoUniversity.WebApplication.csproj            │
│      │   (TFM + NuGet + depends on API running)             │
│      │                                                      │
│      ▼                                                      │
│  Step 4: ContosoUniversity.Test.csproj                      │
│      │   (TFM + MSTest packages)                            │
│      │                                                      │
│      ▼                                                      │
│  Step 5: ContosoUniversity.CodedUITest.csproj               │
│      │   (TFM + Selenium packages)                          │
│      │                                                      │
│      ▼                                                      │
│  Step 6: Dockerfiles (both)                                 │
│      │                                                      │
│      ▼                                                      │
│  Step 7: Bicep infrastructure                               │
│      │                                                      │
│      ▼                                                      │
│  Step 8: CI/CD pipeline                                     │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Entity: Project Files (.csproj)

### ContosoUniversity.API.csproj (Step 2 -- Primary)

| Field | Current | Target |
|-------|---------|--------|
| TargetFramework | net6.0 | net10.0 |
| EF Core packages | 7.0.4 | 10.0.x |
| Microsoft.Data.SqlClient | 5.0.1 | 6.0.x |
| Azure.Identity | 1.8.2 | 1.13.x+ |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.2.2 | 1.3.x+ |
| Microsoft.Extensions.Azure | 1.6.3 | 10.0.x |
| Microsoft.ApplicationInsights.AspNetCore | 2.21.0 | 2.22.x+ |
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 6.0.5 | REMOVE |
| Microsoft.AspNetCore.Razor.Language | 6.0.15 | REMOVE (shared framework) |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 6.0.4 | REMOVE |
| NSwag.AspNetCore | 13.18.2 | REMOVE (replace with OpenApi) |
| System.Runtime.Extensions | 4.3.1 | REMOVE (built-in) |
| Bogus | 34.0.2 | 35.x+ |
| NEW: Microsoft.AspNetCore.OpenApi | -- | 10.0.x (or shared framework) |
| NEW: Swashbuckle.AspNetCore.SwaggerUI | -- | 7.x (UI only) |

**Validation**: `dotnet build` succeeds; `dotnet ef migrations add VerifyNoChanges`
produces empty Up()/Down() methods.

### ContosoUniversity.WebApplication.csproj (Step 3)

| Field | Current | Target |
|-------|---------|--------|
| TargetFramework | net6.0 | net10.0 |
| Microsoft.ApplicationInsights.AspNetCore | 2.21.0 | 2.22.x+ |
| Microsoft.AspNetCore.Razor.Language | 6.0.15 | REMOVE (shared framework) |
| Microsoft.Extensions.DependencyInjection | 6.0.0 | REMOVE (shared framework) |
| Microsoft.Extensions.DependencyInjection.Abstractions | 6.0.0 | REMOVE (shared framework) |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 6.0.4 | REMOVE |
| Newtonsoft.Json | 13.0.3 | REMOVE (use System.Text.Json) |

**Validation**: `dotnet build` succeeds; home page loads at localhost.

### ContosoUniversity.Test.csproj (Step 4)

| Field | Current | Target |
|-------|---------|--------|
| TargetFramework | net6.0 | net10.0 |
| Microsoft.NET.Test.Sdk | 17.2.0 | 17.12.x+ |
| MSTest.TestAdapter | 2.2.10 | 3.7.x+ |
| MSTest.TestFramework | 2.2.10 | 3.7.x+ |
| coverlet.collector | 3.1.2 | 6.0.x+ |

**Validation**: `dotnet test` passes all existing tests.

### ContosoUniversity.CodedUITest.csproj (Step 5)

| Field | Current | Target |
|-------|---------|--------|
| TargetFramework | net6.0 | net10.0 |
| Microsoft.NET.Test.Sdk | 17.2.0 | 17.12.x+ |
| MSTest.TestAdapter | 2.2.10 | 3.7.x+ |
| MSTest.TestFramework | 2.2.10 | 3.7.x+ |
| Selenium.WebDriver | 4.1.1 | 4.27.x+ |
| Selenium.WebDriver.ChromeDriver | 101.0.4951.4100 | REMOVE (use Selenium.Manager) |

**Validation**: `dotnet build` succeeds; Selenium tests run against local instance.

## Entity: global.json (Step 1)

Two files: `src/ContosoUniversity.API/global.json` and
`src/ContosoUniversity.WebApplication/global.json`.

| Field | Current | Target |
|-------|---------|--------|
| sdk.version | 6.0.300 | 10.0.100 |
| sdk.rollForward | latestFeature | latestFeature (unchanged) |

## Entity: Program.cs Modernization (API only)

| Change | Before | After |
|--------|--------|-------|
| Endpoint routing | `app.UseEndpoints(e => e.MapControllers())` | `app.MapControllers()` |
| OpenAPI | `app.UseOpenApi(); app.UseSwaggerUi3()` | `app.MapOpenApi(); app.UseSwaggerUI(...)` |
| Services | -- | `builder.Services.AddOpenApi()` |

## Entity: Dockerfiles (Step 6)

| Change | Before | After |
|--------|--------|-------|
| Base image | `mcr.microsoft.com/dotnet/aspnet:6.0` | `mcr.microsoft.com/dotnet/aspnet:10.0` |
| SDK image | `mcr.microsoft.com/dotnet/sdk:6.0` | `mcr.microsoft.com/dotnet/sdk:10.0` |
| Port | EXPOSE 80 | EXPOSE 80 + `ENV ASPNETCORE_URLS=http://+:80` |

## Entity: Bicep Infrastructure (Step 7)

File: `infra/resources.bicep`

4 occurrences of `netFrameworkVersion: 'v6.0'` changed to `'v10.0'`.

## Entity: CI/CD Pipeline (Step 8)

File: `.github/workflows/resilience-pipeline.yml`

| Change | Before | After |
|--------|--------|-------|
| DOTNET_VERSION | '6.0.x' | '10.0.x' |

## State Transitions

```text
[Current: .NET 6 + EF Core 7]
    │
    ├── SDK updated (global.json) ──► [SDK 10.0, projects still net6.0]
    │
    ├── API project migrated ──► [API on net10.0, WebApp still net6.0]
    │                              (solution does NOT build yet)
    │
    ├── WebApp project migrated ──► [API + WebApp on net10.0]
    │                                (solution builds, tests may fail)
    │
    ├── Test projects migrated ──► [All projects on net10.0]
    │                               (solution builds, tests pass)
    │
    ├── Dockerfiles updated ──► [Containers build with .NET 10]
    │
    ├── Bicep updated ──► [Infrastructure aligned]
    │
    └── Pipeline updated ──► [CI/CD runs on .NET 10 SDK]
        │
        └── DONE: Full migration complete
```
