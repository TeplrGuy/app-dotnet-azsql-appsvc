# Research: .NET 6 to .NET 10 Migration

**Feature**: 001-net6-to-net10-migration
**Date**: 2026-04-27

## Research Tasks

### R1: NuGet Package Version Mapping (.NET 6 -> .NET 10)

**Decision**: Upgrade all packages to their latest .NET 10-compatible versions.

**Findings**:

| Package (Current) | Target Version | Notes |
|---|---|---|
| Microsoft.EntityFrameworkCore (7.0.4) | 10.0.x | Ships with .NET 10; major version aligns with runtime |
| Microsoft.EntityFrameworkCore.SqlServer (7.0.4) | 10.0.x | Same as above |
| Microsoft.EntityFrameworkCore.Tools (7.0.4) | 10.0.x | Same as above |
| Microsoft.Data.SqlClient (5.0.1) | 6.0.x | Latest stable compatible with EF Core 10 |
| Microsoft.ApplicationInsights.AspNetCore (2.21.0) | 2.22.x+ | Continues to work on .NET 10 |
| Azure.Identity (1.8.2) | 1.13.x+ | Supports .NET 10; no breaking changes |
| Azure.Extensions.AspNetCore.Configuration.Secrets (1.2.2) | 1.3.x+ | Compatible |
| Microsoft.Extensions.Azure (1.6.3) | 10.0.x | Aligns with runtime |
| Microsoft.AspNetCore.Mvc.NewtonsoftJson (6.0.5) | REMOVE | Replace with System.Text.Json (see R4) |
| Microsoft.AspNetCore.Razor.Language (6.0.15) | REMOVE | Part of shared framework in .NET 10 |
| Microsoft.Extensions.DependencyInjection (6.0.0) | REMOVE | Part of shared framework in .NET 10 |
| Microsoft.Extensions.DependencyInjection.Abstractions (6.0.0) | REMOVE | Part of shared framework in .NET 10 |
| Microsoft.VisualStudio.Web.CodeGeneration.Design (6.0.4) | REMOVE | Not needed at runtime; dev-time only tool |
| NSwag.AspNetCore (13.18.2) | REPLACE | Replace with built-in OpenAPI (Microsoft.AspNetCore.OpenApi) |
| Newtonsoft.Json (13.0.3) | EVALUATE | See R4 for replacement decision |
| Bogus (34.0.2) | 35.x+ | netstandard2.0 compatible; works on .NET 10 |
| System.Runtime.Extensions (4.3.1) | REMOVE | Part of .NET runtime since .NET Core 1.0 |
| Microsoft.NET.Test.Sdk (17.2.0) | 17.12.x+ | Latest supports .NET 10 |
| MSTest.TestAdapter (2.2.10) | 3.7.x+ | MSTest v3 is the modern path |
| MSTest.TestFramework (2.2.10) | 3.7.x+ | MSTest v3 |
| coverlet.collector (3.1.2) | 6.0.x+ | Latest stable |
| Selenium.WebDriver (4.1.1) | 4.27.x+ | Latest 4.x supports .NET 10 |
| Selenium.WebDriver.ChromeDriver (101.0.4951.4100) | REPLACE | Use Selenium.Manager (built into WebDriver 4.6+) for auto-driver |

**Rationale**: Aligning major versions with the .NET runtime version ensures
support timeline parity. Packages that are now part of the shared framework
should be removed to avoid diamond-dependency conflicts.

**Alternatives considered**: Keeping EF Core at 9.x -- rejected because 10.x
aligns with the LTS runtime and receives full support until EOL.

---

### R2: Endpoint Routing Modernization (UseEndpoints -> MapControllers)

**Decision**: Replace `UseEndpoints(endpoints => { endpoints.MapControllers(); })`
with top-level `app.MapControllers()`.

**Rationale**: The `UseEndpoints()` pattern is obsolete in .NET 7+ and
generates analyzer warning ASP0014. The modern pattern is simpler and
avoids the intermediate endpoint routing middleware registration.

**Before** (current API Program.cs):
```csharp
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
```

**After** (modernized):
```csharp
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
```

**Impact**: `UseRouting()` is still valid but optional when using top-level
route registration. However, since `UseAuthorization()` depends on endpoint
metadata, keeping `UseRouting()` before `UseAuthorization()` ensures correct
middleware ordering. The explicit `UseRouting()` call is preserved for clarity.

**Alternatives considered**: Converting to Minimal APIs -- rejected because
the API uses MVC controllers extensively and a full rewrite is out of scope
per the constitution ("no architectural rewrites").

---

### R3: NSwag Replacement with Built-in OpenAPI

**Decision**: Replace `NSwag.AspNetCore` with `Microsoft.AspNetCore.OpenApi`
(built-in since .NET 9) plus `Swashbuckle.AspNetCore.SwaggerUI` or the
built-in Scalar UI.

**Rationale**: Constitution Principle VI (Built-in First) mandates preferring
built-in features. ASP.NET Core 9+ includes `Microsoft.AspNetCore.OpenApi`
which generates OpenAPI documents natively. .NET 10 further improves this
with scalar UI integration.

**Migration steps**:
1. Remove `NSwag.AspNetCore` package reference
2. Add `Microsoft.AspNetCore.OpenApi` (part of shared framework in .NET 10)
3. Replace `app.UseOpenApi()` + `app.UseSwaggerUi3()` with:
   ```csharp
   builder.Services.AddOpenApi();
   // ...
   app.MapOpenApi();        // Serves /openapi/v1.json
   app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "Contoso API"));
   ```
4. If Swagger UI is needed, add `Swashbuckle.AspNetCore.SwaggerUI` (UI only,
   no document generation) or use the built-in scalar endpoint.

**Alternatives considered**: Keeping NSwag -- rejected per Principle VI;
NSwag 13.x may not support .NET 10 fully and the built-in option provides
the same functionality.

---

### R4: Newtonsoft.Json Evaluation

**Decision**: Remove `Newtonsoft.Json` from the WebApplication project.
Replace `Microsoft.AspNetCore.Mvc.NewtonsoftJson` in the API project with
`System.Text.Json`.

**Rationale**: Constitution Principle VI (Built-in First) mandates
`System.Text.Json` unless compatibility requires Newtonsoft.

**Analysis of current usage**:
- **WebApplication**: Uses `Newtonsoft.Json` for HTTP client deserialization
  of API responses. Can be replaced with `System.Text.Json` (the default
  `HttpClient` JSON extension methods use STJ).
- **API**: Uses `Microsoft.AspNetCore.Mvc.NewtonsoftJson` which configures
  MVC to serialize/deserialize with Newtonsoft. Since .NET 10 MVC uses STJ
  by default, removing this package aligns with the built-in behavior.

**Risk assessment**:
- If any model uses `[JsonProperty]` attributes from Newtonsoft, those need
  to be replaced with `[JsonPropertyName]` from System.Text.Json.
- If any model relies on Newtonsoft-specific features (e.g., `$type` handling,
  `PreserveReferencesHandling`), a compatibility shim may be needed.

**Mitigation**: Grep codebase for `Newtonsoft.Json` attribute usage and
replace during implementation. If complex serialization is found, document
justification for retention.

**Alternatives considered**: Keeping Newtonsoft as-is -- rejected because no
complex serialization patterns were detected in the codebase, and STJ is the
framework default.

---

### R5: EF Core 10 Convention Changes

**Decision**: Pin EF Core model configuration to match existing database
schema conventions.

**Rationale**: EF Core 10 may introduce new default conventions that differ
from EF Core 7 (the version currently used). The constitution (Principle I)
requires zero schema changes.

**Key conventions to verify/pin**:
- **Table naming**: EF Core uses DbSet property name by default. If the
  existing schema uses different names, `modelBuilder.Entity<T>().ToTable()`
  must be explicit.
- **Pluralization**: EF Core 10 does not pluralize by default (same as EF Core 7).
  No action needed.
- **Cascade delete**: Default is `DeleteBehavior.Cascade` for required
  relationships. Must verify this matches the existing FK constraints.
- **String column length**: Default is `nvarchar(max)`. If existing columns
  have specific lengths, `HasMaxLength()` must be configured.
- **Decimal precision**: EF Core 10 warns if decimal properties lack explicit
  precision. Add `HasPrecision()` for any decimal columns.

**Action**: During implementation, verify the generated migration (if any)
produces an empty `Up()` method, confirming no schema drift.

**Alternatives considered**: Running `Add-Migration` to detect drift --
this is the validation approach, not the fix. The fix is pinning conventions.

---

### R6: Docker Base Image Tags for .NET 10

**Decision**: Use `mcr.microsoft.com/dotnet/aspnet:10.0` and
`mcr.microsoft.com/dotnet/sdk:10.0` as base image tags.

**Rationale**: Official Microsoft container images for .NET 10 LTS are
published to MCR under these tags. The `10.0` tag tracks the latest patch.

**Multi-stage pattern**:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# ...build steps...

FROM base AS final
# ...final image...
```

**Note**: .NET 8+ changed the default port from 80 to 8080. Must update
`EXPOSE` directive and verify App Service/container orchestration expects
port 8080. Alternatively, set `ASPNETCORE_URLS=http://+:80` to preserve
the existing port.

**Decision on port**: Keep port 80 by setting `ENV ASPNETCORE_URLS=http://+:80`
in the Dockerfile to avoid disrupting existing App Service configuration
and docker-compose setup. This is simpler than updating all downstream
configuration.

---

### R7: Bicep netFrameworkVersion Value

**Decision**: Change `netFrameworkVersion: 'v6.0'` to `netFrameworkVersion: 'v10.0'`
in all App Service siteConfig blocks in `infra/resources.bicep`.

**Rationale**: The `netFrameworkVersion` property in Azure App Service ARM/Bicep
controls which .NET runtime is available on the Windows hosting platform.

**Locations** (4 occurrences in resources.bicep): Lines 210, 284, 321, 358.

---

### R8: CI/CD Pipeline SDK Version

**Decision**: Change `DOTNET_VERSION: '6.0.x'` to `DOTNET_VERSION: '10.0.x'`
in `.github/workflows/resilience-pipeline.yml`.

**Rationale**: The `setup-dotnet` action uses this version to install the SDK.
Must match the target framework of the solution.

---

## Summary of Decisions

| # | Topic | Decision |
|---|-------|----------|
| R1 | NuGet packages | Upgrade to .NET 10 versions; remove shared-framework packages |
| R2 | Endpoint routing | Replace UseEndpoints with app.MapControllers() |
| R3 | OpenAPI/Swagger | Replace NSwag with built-in Microsoft.AspNetCore.OpenApi |
| R4 | JSON serialization | Replace Newtonsoft.Json with System.Text.Json |
| R5 | EF Core conventions | Pin model config to prevent schema drift |
| R6 | Docker images | Use aspnet:10.0 and sdk:10.0; keep port 80 |
| R7 | Bicep | Update netFrameworkVersion to v10.0 |
| R8 | CI/CD | Update DOTNET_VERSION to 10.0.x |
