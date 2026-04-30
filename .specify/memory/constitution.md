<!--
  ## Sync Impact Report
  - Version change: N/A (initial) -> 1.0.0
  - Modified principles: None (initial ratification)
  - Added sections:
    - Core Principles (9 principles)
    - Technology Stack & Migration Scope
    - Quality Gates & Development Workflow
    - Governance
  - Removed sections: None
  - Templates requiring updates:
    - `.specify/templates/plan-template.md` -- No updates needed
    - `.specify/templates/spec-template.md` -- No updates needed
    - `.specify/templates/tasks-template.md` -- No updates needed
    - `.specify/templates/checklist-template.md` -- No updates needed
  - Follow-up TODOs: None
-->

# Contoso University .NET Migration Constitution

## Core Principles

### I. Database Schema Compatibility

All migration work MUST maintain backward compatibility with the
existing Azure SQL database schema. Entity Framework model changes
MUST NOT alter column types, remove columns, or drop tables. New
EF Core conventions (e.g., pluralization, naming) MUST be suppressed
or configured to match the current schema. Any schema evolution MUST
use additive-only migrations validated against the production schema.

### II. Zero-Downtime Migration (NON-NEGOTIABLE)

No data loss or migration downtime is permitted. Database migrations
MUST be backward-compatible so that both old and new application
versions can run concurrently during rollout. Deployment MUST use
slot-swap or rolling-update strategies. Destructive operations
(column removal, type narrowing) are forbidden in the same release
as the code change that removes their usage.

### III. Microsoft Modernization Patterns

All changes MUST follow Microsoft's recommended modernization
patterns for .NET upgrades. This includes:

- Incremental migration over big-bang rewrites
- Using the .NET Upgrade Assistant guidance where applicable
- Adopting minimal hosting model (`WebApplication.CreateBuilder`)
- Replacing obsolete APIs with their documented successors
- Following the official .NET 6-to-10 migration guides

### IV. Test Gate (NON-NEGOTIABLE)

All changes MUST pass the existing unit test suite in
`ContosoUniversity.Test` before merging. No PR may be merged with
a red test. If a test must change due to an API evolution, the
updated test MUST be reviewed and approved in the same PR. Test
coverage MUST NOT decrease as a result of the migration.

### V. Dependency Injection & Async I/O

All service registrations MUST use constructor-based dependency
injection. All I/O-bound operations (database queries, HTTP calls,
file access) MUST use `async`/`await`. Synchronous blocking calls
(`Task.Result`, `Task.Wait()`, `.GetAwaiter().GetResult()`) are
forbidden in request-handling paths.

### VI. Built-in First

Where .NET 10 provides a built-in capability that replaces a
third-party package, the built-in option MUST be preferred. Examples:

- `System.Text.Json` over `Newtonsoft.Json` (unless serialization
  compatibility requires Newtonsoft temporarily)
- Built-in minimal API or Razor conventions over external scaffolding
- `Microsoft.Extensions.*` packages from the shared framework over
  standalone NuGet equivalents

Exceptions MUST be documented with a rationale in the PR description.

### VII. Official Container Images

Dockerfiles MUST use official Microsoft container base images from
`mcr.microsoft.com`. The runtime and SDK tags MUST target the .NET
10 version matching the application's `TargetFramework`. Multi-stage
builds MUST use the SDK image for build and the ASP.NET runtime image
for the final stage. No third-party or community base images are
permitted without explicit written justification.

### VIII. Infrastructure Alignment

All Infrastructure as Code (Bicep) MUST stay aligned with the
application's target framework version. When the application moves
to .NET 10, the App Service `linuxFxVersion` (or Windows equivalent),
container image tags, and any runtime-stack references in Bicep MUST
be updated in the same PR or immediately following PR. Bicep and
application target framework MUST NOT diverge.

### IX. CI/CD Quality Gates

The CI/CD pipeline MUST gate deployments on:

1. Successful `dotnet build` with zero warnings treated as errors
2. All unit tests passing (`dotnet test`)
3. No critical security vulnerabilities in dependency scan

No deployment to any environment (QA, staging, production) is
permitted unless all three gates pass. Pipeline definitions MUST
be updated to reflect .NET 10 SDK and runtime versions.

## Technology Stack & Migration Scope

| Component | Current (Source) | Target |
|---|---|---|
| Runtime | .NET 6.0 (LTS, EOL) | .NET 10.0 (LTS) |
| Web Framework | ASP.NET Core 6.0 MVC/Razor | ASP.NET Core 10.0 MVC/Razor |
| ORM | EF Core 7.0.4 | EF Core 10.0 |
| Database | Azure SQL Database | Azure SQL Database (unchanged) |
| Hosting | Azure App Service | Azure App Service |
| IaC | Bicep | Bicep (updated runtime refs) |
| Containers | .NET 6 base images | .NET 10 base images |
| CI/CD | GitHub Actions | GitHub Actions (updated SDK) |
| Telemetry | Application Insights SDK | Application Insights SDK |

**In scope**: Framework upgrade, NuGet dependency updates, Dockerfile
base image updates, Bicep runtime references, CI/CD SDK versions,
deprecated API replacements.

**Out of scope**: Feature additions, UI redesign, database schema
changes, new Azure service adoption, architectural rewrites.

## Quality Gates & Development Workflow

### Branch Strategy

All migration work MUST occur on the `001-net6-to-net10-migration`
feature branch. PRs MUST target `main` and require at least one
approval before merge.

### Migration Sequence

1. Update `global.json` and `TargetFramework` in all `.csproj` files
2. Update NuGet package references to .NET 10-compatible versions
3. Replace deprecated APIs with .NET 10 equivalents
4. Update Dockerfiles to .NET 10 base images
5. Update Bicep infrastructure references
6. Update CI/CD pipeline SDK version
7. Validate: build, test, and local run
8. Load test and chaos test in staging (pre-production gate)

### Code Review Checklist

Every PR MUST verify:

- No synchronous blocking in async paths
- No hardcoded connection strings or secrets
- DI registration for all new services
- EF queries use `.AsNoTracking()` where applicable
- No `Newtonsoft.Json` usage without documented justification
- Dockerfile base image matches target framework
- Bicep runtime references match target framework

## Governance

This constitution supersedes all other practices for the duration
of the .NET 6-to-.NET 10 migration. All PRs and code reviews MUST
verify compliance with the principles above.

### Amendment Procedure

1. Propose amendment via PR modifying this file
2. Include rationale and impact assessment
3. Require approval from at least one migration stakeholder
4. Update version number per semantic versioning (see below)

### Versioning Policy

- **MAJOR**: Removal or redefinition of a non-negotiable principle
- **MINOR**: New principle added or existing principle materially
  expanded
- **PATCH**: Clarifications, wording fixes, non-semantic refinements

### Compliance Review

At each migration milestone (csproj update, API replacement,
Dockerfile update, Bicep update, pipeline update), verify all
nine principles are satisfied before proceeding to the next
milestone.

**Version**: 1.0.0 | **Ratified**: 2026-04-27 | **Last Amended**: 2026-04-27
