# Research: .NET 6 to .NET 10 LTS Migration

**Date**: 2026-04-27
**Source**: Assessment tool output + codebase analysis + Microsoft migration docs

## Decision 1: Target Framework Version

- **Decision**: .NET 10.0 LTS (TFM: `net10.0`)
- **Rationale**: .NET 10 is the current LTS release (November 2025), providing 3 years of support. Skipping .NET 8 and 9 reduces total migration effort to a single jump.
- **Alternatives considered**: .NET 8 LTS (rejected: would require another migration soon), .NET 9 STS (rejected: short-term support only)

## Decision 2: EF Core Version

- **Decision**: EF Core 10.0.7 (matching the target framework)
- **Rationale**: Assessment confirms the upgrade from 7.0.4 to 10.0.7 is compatible. No breaking changes in the EF Core data model conventions that affect this schema.
- **Risk**: EF Core 10 has convention changes for TimeSpan mapping. Mitigation: verify `dotnet ef migrations has-pending-model-changes` returns clean.
- **Alternatives considered**: EF Core 9.0.x (rejected: would be out-of-band with the runtime)

## Decision 3: Microsoft.Data.SqlClient Version

- **Decision**: Upgrade from 5.0.1 to 7.0.1
- **Rationale**: Current version has known security vulnerabilities (flagged in assessment). Version 7.0.1 is the latest compatible release for .NET 10.
- **Alternatives considered**: Keeping 5.0.1 (rejected: security vulnerability)

## Decision 4: Azure.Identity Version

- **Decision**: Upgrade from 1.8.2 to 1.21.0
- **Rationale**: Security vulnerability in current version. 1.21.0 is the latest stable release supporting .NET 10.
- **Alternatives considered**: None viable due to security requirement

## Decision 5: Application Insights Package

- **Decision**: Replace deprecated `Microsoft.ApplicationInsights.AspNetCore` 2.21.0 with `Azure.Monitor.OpenTelemetry.AspNetCore`
- **Rationale**: Microsoft.ApplicationInsights.AspNetCore is deprecated as of 2024. The recommended replacement is the OpenTelemetry-based Azure Monitor package which is the modern telemetry SDK.
- **Alternatives considered**: Keeping deprecated package (rejected: deprecated packages may lose support; constitution principle V favors built-in/modern alternatives)

## Decision 6: Endpoint Routing Modernization

- **Decision**: Replace `app.UseEndpoints(endpoints => { endpoints.MapControllers(); })` with `app.MapControllers()` directly on the `WebApplication` instance
- **Rationale**: The `UseEndpoints` pattern is legacy since .NET 6 minimal hosting. In .NET 10, direct mapping on the app instance is the canonical pattern and reduces ceremony.
- **Code change**:
  ```csharp
  // Before (.NET 6 style)
  app.UseEndpoints(endpoints =>
  {
      endpoints.MapControllers();
  });

  // After (.NET 10 style)
  app.MapControllers();
  ```
- **Alternatives considered**: Keeping UseEndpoints (rejected: works but is not idiomatic; assessment flags no issues but modernization is requested)

## Decision 7: NSwag vs Built-in OpenAPI

- **Decision**: Keep NSwag.AspNetCore 13.18.2 for now (marked compatible in assessment)
- **Rationale**: Assessment shows NSwag is compatible with .NET 10. The spec clarification suggested replacing with built-in OpenAPI, but since NSwag provides code generation features beyond what built-in OpenAPI offers and is marked compatible, this can be deferred to a follow-up PR.
- **Alternatives considered**: Replacing with Microsoft.AspNetCore.OpenApi (deferred: scope creep for this migration; no compatibility issue)

## Decision 8: Packages to Remove (Included in Framework)

- **Decision**: Remove `Microsoft.AspNetCore.Razor.Language` and `System.Runtime.Extensions`
- **Rationale**: Assessment confirms these are included in the .NET 10 framework reference. Explicit package references are unnecessary and may cause version conflicts.
- **Alternatives considered**: Keeping them (rejected: assessment flags as redundant)

## Decision 9: Microsoft.Extensions.Azure (Deprecated)

- **Decision**: Keep `Microsoft.Extensions.Azure` 1.6.3 as it still functions; evaluate replacement in follow-up
- **Rationale**: While marked deprecated, there is no direct 1:1 replacement. The package provides `AddAzureClients()` service registration. It still functions on .NET 10.
- **Alternatives considered**: Removing entirely (rejected: would require rewriting Azure client DI registration)

## Decision 10: TimeSpan.FromSeconds API Change

- **Decision**: Cast arguments to `int` or use the new `TimeSpan.FromSeconds(int)` overload where source-incompatible
- **Rationale**: In .NET 10, `TimeSpan.FromSeconds(double)` has a new overload `TimeSpan.FromSeconds(int)` that causes ambiguity with integer literals. Explicit cast resolves the issue.
- **Code fix**: `TimeSpan.FromSeconds((double)5)` or `TimeSpan.FromSeconds(5.0)`
- **Impact**: 5 occurrences in CodedUITest, 1 in WebApplication

## Decision 11: Dockerfile Base Images

- **Decision**: `mcr.microsoft.com/dotnet/aspnet:10.0` (runtime) and `mcr.microsoft.com/dotnet/sdk:10.0` (build)
- **Rationale**: Official MCR images for .NET 10 LTS. Multi-stage build pattern preserved.
- **Alternatives considered**: Alpine variants (deferred: standard images are simpler for initial migration)

## Decision 12: Global.json SDK Version

- **Decision**: Update to `"version": "10.0.203"` with `"rollForward": "latestPatch"`
- **Rationale**: Pin to the installed SDK version with patch-level rollForward for CI stability.
- **Alternatives considered**: `latestMajor` (rejected: too permissive for production builds)

## Decision 13: Upgrade Order

- **Decision**: API → WebApplication → Test → CodedUITest
- **Rationale**: API has the data layer (EF Core, models) and no project dependencies. WebApplication depends on API being functional (calls it via HTTP). Test and CodedUITest have no project references but should be upgraded after the main apps to validate them.
- **Alternatives considered**: All-at-once (rejected: harder to isolate build failures)

## Decision 14: Newtonsoft.Json in WebApplication

- **Decision**: Keep Newtonsoft.Json 13.0.3 → 13.0.4 (minor patch) for this migration
- **Rationale**: Assessment recommends a patch upgrade. Full replacement with System.Text.Json is deferred to avoid scope creep in the framework migration PR. The spec clarification allows this as a follow-up.
- **Alternatives considered**: Replace with System.Text.Json now (deferred: separate PR to minimize risk)

## Decision 15: CI/CD Pipeline Update

- **Decision**: Change `DOTNET_VERSION: '6.0.x'` to `DOTNET_VERSION: '10.0.x'` in `resilience-pipeline.yml`
- **Rationale**: Single variable controls SDK version across all pipeline jobs
- **Alternatives considered**: None needed; straightforward variable change
