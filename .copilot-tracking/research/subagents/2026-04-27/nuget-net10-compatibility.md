# NuGet Package .NET 10 Compatibility Research

## Research Questions

For each of 23 NuGet packages, determine:

1. Whether the current version is compatible with .NET 10 (net10.0 TFM)
2. The recommended latest stable version as of April 2026
3. Whether the package is deprecated or has been replaced
4. Any known security vulnerabilities (CVEs)
5. Whether the package is included in the framework and can be removed

## Status: Complete

## Sources

- nuget.org package pages (fetched 2026-04-27)
- Known CVE databases for .NET packages

---

## Package Findings

### 1. Microsoft.ApplicationInsights.AspNetCore

| Field | Value |
|---|---|
| Current Version | 2.21.0 |
| Latest Stable | **3.1.0** |
| Target Frameworks | .NET 8.0+ |
| .NET 10 Compatible (current) | Yes (via .NET Standard 2.0 in older versions), but old API surface |
| .NET 10 Compatible (latest) | Yes -- targets .NET 8.0+ |
| Deprecated/Replaced | Not deprecated. However, Microsoft recommends migrating to **Azure Monitor OpenTelemetry Distro** (`Azure.Monitor.OpenTelemetry.AspNetCore`) for new projects. The classic App Insights SDK is in maintenance mode. |
| Known CVEs | None known for 2.21.0 |
| Framework-included | No -- must be explicitly referenced |
| Recommendation | **Upgrade to 3.1.0**. Consider migrating to `Azure.Monitor.OpenTelemetry.AspNetCore` for the modern OpenTelemetry-based approach. |

### 2. Microsoft.AspNetCore.Razor.Language

| Field | Value |
|---|---|
| Current Version | 6.0.15 |
| Latest Stable | **6.0.36** (last .NET 6 patch) |
| Target Frameworks | .NET Standard 2.0 |
| .NET 10 Compatible (current) | Technically yes via netstandard2.0, but mismatched with .NET 10 runtime |
| .NET 10 Compatible (latest) | Same -- 6.0.36 is the final version |
| Deprecated/Replaced | **Effectively deprecated.** Starting with .NET 7, Razor Language is included in the ASP.NET Core shared framework. No 7.x/8.x/9.x/10.x standalone package exists. |
| Known CVEs | None known |
| Framework-included | **Yes -- REMOVE this package.** The Razor compiler/language is part of the ASP.NET Core shared framework in .NET 10. |
| Recommendation | **Remove entirely.** No replacement package needed. |

### 3. Microsoft.Extensions.DependencyInjection

| Field | Value |
|---|---|
| Current Version | 6.0.0 |
| Latest Stable | **10.0.7** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0, .NET Framework 4.6.2 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0) |
| .NET 10 Compatible (latest) | Yes -- targets .NET 8.0+ |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known |
| Framework-included | **Yes -- can likely be removed.** In ASP.NET Core projects targeting .NET 10, DI is provided by the shared framework (`Microsoft.AspNetCore.App`). Only needed if used in a non-ASP.NET context (e.g., class library, console app). |
| Recommendation | **Remove if ASP.NET Core project** (provided by shared framework). If kept, upgrade to 10.0.7. |

### 4. Microsoft.Extensions.DependencyInjection.Abstractions

| Field | Value |
|---|---|
| Current Version | 6.0.0 |
| Latest Stable | **10.0.7** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0, .NET Framework 4.6.2 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known |
| Framework-included | **Yes -- can likely be removed.** Provided by the ASP.NET Core shared framework. |
| Recommendation | **Remove if ASP.NET Core project.** If kept, upgrade to 10.0.7. |

### 5. Microsoft.VisualStudio.Web.CodeGeneration.Design

| Field | Value |
|---|---|
| Current Version | 6.0.4 |
| Latest Stable | **10.0.2** |
| Target Frameworks | .NET 10.0 |
| .NET 10 Compatible (current) | No -- 6.0.x targets .NET 6 specifically |
| .NET 10 Compatible (latest) | **Yes** -- 10.0.2 targets .NET 10.0 |
| Deprecated/Replaced | Not deprecated. This is the scaffolding tool. |
| Known CVEs | None known |
| Framework-included | No -- design-time tool |
| Recommendation | **Upgrade to 10.0.2.** Version must match target framework major version. |

### 6. Newtonsoft.Json

| Field | Value |
|---|---|
| Current Version | 13.0.3 |
| Latest Stable | **13.0.4** |
| Target Frameworks | .NET 6.0+, .NET Standard 1.0, .NET Framework 2.0 |
| .NET 10 Compatible (current) | Yes (via netstandard/net6.0 TFMs) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated. Still widely used, though `System.Text.Json` is the built-in alternative. |
| Known CVEs | 13.0.3 had no critical CVEs, but **13.0.4 includes security hardening**. |
| Framework-included | No -- third-party package |
| Recommendation | **Upgrade to 13.0.4.** Consider migrating to `System.Text.Json` for new code. |

### 7. Azure.Extensions.AspNetCore.Configuration.Secrets

| Field | Value |
|---|---|
| Current Version | 1.2.2 |
| Latest Stable | **1.5.0** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known |
| Framework-included | No |
| Recommendation | **Upgrade to 1.5.0.** |

### 8. Azure.Identity

| Field | Value |
|---|---|
| Current Version | 1.8.2 |
| Latest Stable | **1.21.0** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | **Yes -- multiple CVEs fixed between 1.8.2 and 1.21.0**, including CVE-2024-35255 (elevation of privilege). Urgent upgrade recommended. |
| Framework-included | No |
| Recommendation | **Upgrade to 1.21.0 immediately** (security fixes). |

### 9. Bogus

| Field | Value |
|---|---|
| Current Version | 34.0.2 |
| Latest Stable | **35.6.5** |
| Target Frameworks | .NET 6.0+, .NET Standard 1.3, .NET Framework 4.0 |
| .NET 10 Compatible (current) | Yes (via netstandard1.3/net6.0) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known |
| Framework-included | No -- third-party fake data generator |
| Recommendation | **Upgrade to 35.6.5.** |

### 10. Microsoft.AspNetCore.Mvc.NewtonsoftJson

| Field | Value |
|---|---|
| Current Version | 6.0.5 |
| Latest Stable | **10.0.7** |
| Target Frameworks | .NET 10.0 |
| .NET 10 Compatible (current) | No -- 6.0.x targets .NET 6.0 |
| .NET 10 Compatible (latest) | **Yes** -- targets .NET 10.0 |
| Deprecated/Replaced | Not deprecated. Required if using Newtonsoft.Json with ASP.NET Core MVC. |
| Known CVEs | None known |
| Framework-included | No -- opt-in package for Newtonsoft.Json integration |
| Recommendation | **Upgrade to 10.0.7.** Version must match ASP.NET Core major version. |

### 11. Microsoft.Data.SqlClient

| Field | Value |
|---|---|
| Current Version | 5.0.1 |
| Latest Stable | **7.0.1** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0, .NET Framework 4.6.2 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0), but outdated |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated. Note: v7.0 requires `Microsoft.Data.SqlClient.Extensions.Azure` for Entra ID auth (breaking change). |
| Known CVEs | **Yes -- CVE-2024-0056** (information disclosure) affects versions before 5.1.4. **Urgent upgrade required.** |
| Framework-included | No |
| Recommendation | **Upgrade to 7.0.1** (security + .NET 10 support). Note the Entra ID auth breaking change in v7. |

### 12. Microsoft.EntityFrameworkCore

| Field | Value |
|---|---|
| Current Version | 7.0.4 |
| Latest Stable | **10.0.7** |
| Target Frameworks | .NET 10.0 |
| .NET 10 Compatible (current) | No -- 7.0.x targets .NET 6.0 only |
| .NET 10 Compatible (latest) | **Yes** -- targets .NET 10.0 |
| Deprecated/Replaced | Not deprecated. EF Core 7 is out of support (EOL Nov 2024). |
| Known CVEs | None known for 7.0.4 specifically |
| Framework-included | No |
| Recommendation | **Upgrade to 10.0.7.** Major version jump (7 to 10) -- review EF Core breaking changes for 8, 9, and 10. |

### 13. Microsoft.EntityFrameworkCore.SqlServer

| Field | Value |
|---|---|
| Current Version | 7.0.4 |
| Latest Stable | **10.0.7** |
| Target Frameworks | .NET 10.0 |
| .NET 10 Compatible (current) | No |
| .NET 10 Compatible (latest) | **Yes** |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known |
| Framework-included | No |
| Recommendation | **Upgrade to 10.0.7.** Must match EF Core version. |

### 14. Microsoft.EntityFrameworkCore.Tools

| Field | Value |
|---|---|
| Current Version | 7.0.4 |
| Latest Stable | **10.0.7** |
| Target Frameworks | .NET 8.0+ |
| .NET 10 Compatible (current) | No |
| .NET 10 Compatible (latest) | **Yes** |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known |
| Framework-included | No -- design-time tooling |
| Recommendation | **Upgrade to 10.0.7.** Must match EF Core version. |

### 15. Microsoft.Extensions.Azure

| Field | Value |
|---|---|
| Current Version | 1.6.3 |
| Latest Stable | **1.14.0** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known |
| Framework-included | No |
| Recommendation | **Upgrade to 1.14.0.** |

### 16. NSwag.AspNetCore

| Field | Value |
|---|---|
| Current Version | 13.18.2 |
| Latest Stable | **14.7.1** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0, .NET Framework 4.6.2 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0), but may have runtime issues |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated. Note: ASP.NET Core 9+ includes built-in OpenAPI support via `Microsoft.AspNetCore.OpenApi`. Consider migrating away from NSwag for Swagger UI; can use Scalar or SwaggerUI separately. |
| Known CVEs | None known |
| Framework-included | No |
| Recommendation | **Upgrade to 14.7.1.** Consider migrating to built-in `Microsoft.AspNetCore.OpenApi` + Scalar UI. |

### 17. System.Runtime.Extensions

| Field | Value |
|---|---|
| Current Version | 4.3.1 |
| Latest Stable | **4.3.1** (last updated Feb 2019) |
| Target Frameworks | .NET Standard 1.0, .NET Framework 4.5 |
| .NET 10 Compatible (current) | Yes (via netstandard1.0) but unnecessary |
| .NET 10 Compatible (latest) | Same version |
| Deprecated/Replaced | **Effectively obsolete.** This is a legacy .NET Core 1.x/2.x compatibility shim. All types (`Math`, `Environment`, `Convert`, `Path`, `Random`, `Stopwatch`, etc.) are built into the .NET runtime. |
| Known CVEs | **Yes -- CVE-2018-0765** (DoS) was patched in 4.3.1, but this package has had no updates since 2019. |
| Framework-included | **Yes -- REMOVE this package.** All types are in the .NET 10 base class library. |
| Recommendation | **Remove entirely.** |

### 18. Microsoft.NET.Test.Sdk

| Field | Value |
|---|---|
| Current Version | 17.2.0 |
| Latest Stable | **18.4.0** |
| Target Frameworks | .NET 8.0+, .NET Core 2.0, .NET Standard 2.0, .NET Framework 4.6.2 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known |
| Framework-included | No -- test infrastructure |
| Recommendation | **Upgrade to 18.4.0.** |

### 19. MSTest.TestAdapter

| Field | Value |
|---|---|
| Current Version | 2.2.10 |
| Latest Stable | **4.2.1** |
| Target Frameworks | .NET 8.0+, .NET Framework 4.6.2 |
| .NET 10 Compatible (current) | Yes (older versions have broader TFM support) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated. MSTest v2 packages are superseded by v3/v4. Consider using the unified `MSTest` meta-package (available since MSTest 3.x). |
| Known CVEs | None known |
| Framework-included | No |
| Recommendation | **Upgrade to 4.2.1.** Consider switching to the unified `MSTest` package. |

### 20. MSTest.TestFramework

| Field | Value |
|---|---|
| Current Version | 2.2.10 |
| Latest Stable | **4.2.1** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0, .NET Framework 4.6.2 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated. See note on MSTest.TestAdapter above. |
| Known CVEs | None known |
| Framework-included | No |
| Recommendation | **Upgrade to 4.2.1.** Consider switching to the unified `MSTest` package. |

### 21. coverlet.collector

| Field | Value |
|---|---|
| Current Version | 3.1.2 |
| Latest Stable | **10.0.0** |
| Target Frameworks | .NET 8.0+ |
| .NET 10 Compatible (current) | Unlikely -- 3.x targets older runtimes |
| .NET 10 Compatible (latest) | Yes -- targets .NET 8.0+ |
| Deprecated/Replaced | Not deprecated. Major version jump (3 to 10) aligns with .NET versioning. |
| Known CVEs | None known |
| Framework-included | No -- code coverage tool |
| Recommendation | **Upgrade to 10.0.0.** |

### 22. Selenium.WebDriver

| Field | Value |
|---|---|
| Current Version | 4.1.1 |
| Latest Stable | **4.43.0** |
| Target Frameworks | .NET 8.0+, .NET Standard 2.0, .NET Framework 4.6.2 |
| .NET 10 Compatible (current) | Yes (via netstandard2.0) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | Not deprecated |
| Known CVEs | None known for 4.1.1, but many bug fixes and improvements in newer versions |
| Framework-included | No -- third-party browser automation |
| Recommendation | **Upgrade to 4.43.0.** Note: Selenium 4.6+ includes Selenium Manager which auto-downloads drivers, reducing need for ChromeDriver package. |

### 23. Selenium.WebDriver.ChromeDriver

| Field | Value |
|---|---|
| Current Version | 101.0.4951.4100 |
| Latest Stable | **147.0.7727.11700** |
| Target Frameworks | No specific TFM -- MSBuild targets only |
| .NET 10 Compatible (current) | Yes (framework-agnostic native binary) |
| .NET 10 Compatible (latest) | Yes |
| Deprecated/Replaced | **Effectively optional** since Selenium 4.6+ (Selenium Manager auto-downloads drivers). The package itself notes this. |
| Known CVEs | Chrome/ChromeDriver versions before ~115 have known security issues |
| Framework-included | No |
| Recommendation | **Consider removing** if using Selenium 4.6+ (Selenium Manager handles driver downloads). If keeping, upgrade to match Chrome version. |

---

## Summary: Packages to Remove

| Package | Reason |
|---|---|
| `Microsoft.AspNetCore.Razor.Language` | Included in ASP.NET Core shared framework since .NET 7 |
| `System.Runtime.Extensions` | Legacy shim; all types built into .NET runtime |
| `Microsoft.Extensions.DependencyInjection` | Provided by ASP.NET Core shared framework (verify usage in non-web projects) |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | Provided by ASP.NET Core shared framework (verify usage in non-web projects) |
| `Selenium.WebDriver.ChromeDriver` | Optional with Selenium 4.6+ Selenium Manager |

## Summary: Security-Critical Upgrades

| Package | Current | Upgrade To | CVE/Issue |
|---|---|---|---|
| `Azure.Identity` | 1.8.2 | 1.21.0 | CVE-2024-35255 (elevation of privilege) |
| `Microsoft.Data.SqlClient` | 5.0.1 | 7.0.1 | CVE-2024-0056 (information disclosure) |
| `Newtonsoft.Json` | 13.0.3 | 13.0.4 | Security hardening |

## Summary: All Upgrade Targets

| # | Package | Current | Target | Action |
|---|---|---|---|---|
| 1 | Microsoft.ApplicationInsights.AspNetCore | 2.21.0 | 3.1.0 | Upgrade |
| 2 | Microsoft.AspNetCore.Razor.Language | 6.0.15 | -- | **Remove** |
| 3 | Microsoft.Extensions.DependencyInjection | 6.0.0 | 10.0.7 or Remove | Upgrade/Remove |
| 4 | Microsoft.Extensions.DependencyInjection.Abstractions | 6.0.0 | 10.0.7 or Remove | Upgrade/Remove |
| 5 | Microsoft.VisualStudio.Web.CodeGeneration.Design | 6.0.4 | 10.0.2 | Upgrade |
| 6 | Newtonsoft.Json | 13.0.3 | 13.0.4 | Upgrade |
| 7 | Azure.Extensions.AspNetCore.Configuration.Secrets | 1.2.2 | 1.5.0 | Upgrade |
| 8 | Azure.Identity | 1.8.2 | 1.21.0 | **Upgrade (security)** |
| 9 | Bogus | 34.0.2 | 35.6.5 | Upgrade |
| 10 | Microsoft.AspNetCore.Mvc.NewtonsoftJson | 6.0.5 | 10.0.7 | Upgrade |
| 11 | Microsoft.Data.SqlClient | 5.0.1 | 7.0.1 | **Upgrade (security)** |
| 12 | Microsoft.EntityFrameworkCore | 7.0.4 | 10.0.7 | Upgrade |
| 13 | Microsoft.EntityFrameworkCore.SqlServer | 7.0.4 | 10.0.7 | Upgrade |
| 14 | Microsoft.EntityFrameworkCore.Tools | 7.0.4 | 10.0.7 | Upgrade |
| 15 | Microsoft.Extensions.Azure | 1.6.3 | 1.14.0 | Upgrade |
| 16 | NSwag.AspNetCore | 13.18.2 | 14.7.1 | Upgrade |
| 17 | System.Runtime.Extensions | 4.3.1 | -- | **Remove** |
| 18 | Microsoft.NET.Test.Sdk | 17.2.0 | 18.4.0 | Upgrade |
| 19 | MSTest.TestAdapter | 2.2.10 | 4.2.1 | Upgrade |
| 20 | MSTest.TestFramework | 2.2.10 | 4.2.1 | Upgrade |
| 21 | coverlet.collector | 3.1.2 | 10.0.0 | Upgrade |
| 22 | Selenium.WebDriver | 4.1.1 | 4.43.0 | Upgrade |
| 23 | Selenium.WebDriver.ChromeDriver | 101.0.4951.4100 | Remove or 147.x | Remove/Upgrade |

## Breaking Change Alerts

1. **Microsoft.Data.SqlClient 7.0** -- Entra ID authentication moved to separate extension package `Microsoft.Data.SqlClient.Extensions.Azure`. `Encrypt=Mandatory` is now the default.
2. **EF Core 7 to 10** -- Multiple breaking changes across 8, 9, and 10 releases. Review migration guides.
3. **MSTest 2.x to 4.x** -- New test runner, some API changes. Consider unified `MSTest` package.
4. **NSwag 13 to 14** -- API changes in Swagger generation. Consider migrating to built-in OpenAPI.
5. **coverlet 3.x to 10.x** -- Minimum .NET 8 runtime required for coverage collection.
