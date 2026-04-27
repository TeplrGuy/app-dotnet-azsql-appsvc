<!--
  Sync Impact Report
  ==================
  Version change: N/A -> 1.0.0 (initial ratification)
  Modified principles: N/A (initial)
  Added sections:
    - Core Principles (7 principles)
    - Migration Constraints
    - Quality Gates and CI/CD Workflow
    - Governance
  Removed sections: N/A
  Templates requiring updates:
    - .specify/templates/plan-template.md ✅ no update needed (generic template)
    - .specify/templates/spec-template.md ✅ no update needed (generic template)
    - .specify/templates/tasks-template.md ✅ no update needed (generic template)
    - .specify/templates/checklist-template.md ✅ no update needed (generic template)
  Follow-up TODOs: none
-->

# Contoso University Constitution

## Core Principles

### I. Database Schema Preservation (NON-NEGOTIABLE)

- All changes MUST maintain backward compatibility with the existing
  Azure SQL database schema.
- No data loss is permitted at any stage of the migration.
- Zero-downtime migration is required; schema changes MUST use additive
  patterns (new columns, new tables) rather than destructive alterations.
- Entity Framework Core migrations MUST be reviewed for backward
  compatibility before merging.
- Rationale: The production Azure SQL database serves live users;
  breaking schema changes risk data loss and service outages.

### II. Microsoft Modernization Patterns

- The migration MUST follow Microsoft's recommended App Modernization
  guidance at
  <https://learn.microsoft.com/en-us/azure/app-modernization-guidance/>.
- Incremental migration is preferred over big-bang rewrites.
- Deprecated .NET 6 APIs MUST be replaced with their .NET 10 LTS
  equivalents using official migration documentation.
- Rationale: Microsoft-endorsed patterns reduce risk, align with
  long-term support, and ensure access to security patches.

### III. Test-Gated Changes (NON-NEGOTIABLE)

- All changes MUST pass the existing unit test suite
  (`ContosoUniversity.Test`) before merging into any shared branch.
- New functionality SHOULD include corresponding test coverage.
- No pull request may be merged with a failing test.
- Rationale: Existing tests are the safety net that confirms functional
  parity between the .NET 6 baseline and the .NET 10 target.

### IV. Async-First with Dependency Injection

- All I/O-bound operations (database, HTTP, file system) MUST use
  `async`/`await`.
- All services MUST be registered and resolved through the built-in
  .NET dependency injection container.
- Constructor injection is the required pattern; service locator
  anti-pattern is prohibited.
- Rationale: Async I/O maximizes throughput under load; DI ensures
  testability and loose coupling.

### V. Built-in Over Third-Party

- Built-in .NET 10 features MUST be preferred over third-party NuGet
  packages where a built-in equivalent exists.
- Third-party dependencies are permitted only when no built-in
  alternative provides equivalent functionality.
- Any new third-party dependency MUST be justified in the pull request
  description.
- Rationale: Reducing external dependencies lowers supply-chain risk
  and simplifies long-term maintenance.

### VI. Official Container Images

- Dockerfiles MUST use official Microsoft container base images from
  `mcr.microsoft.com`.
- Images MUST target the .NET 10 runtime or SDK tags appropriate for
  the build stage.
- Multi-stage builds are required to keep production images minimal.
- Rationale: Official images receive timely security patches and are
  validated against the .NET runtime.

### VII. Infrastructure-Application Alignment

- All Bicep Infrastructure as Code files in `/infra` MUST stay aligned
  with the application's target framework version (.NET 10 LTS).
- App Service runtime stack, container image tags, and SDK references
  MUST reflect the migrated framework.
- Changes to application target framework MUST be accompanied by
  corresponding Bicep parameter or resource updates in the same PR.
- Rationale: Drift between IaC and application code causes deployment
  failures and environment inconsistencies.

## Migration Constraints

- **Source framework**: .NET 6 (end of support)
- **Target framework**: .NET 10 LTS
- **Migration scope**: WebApplication (MVC + Razor Pages), API,
  unit tests, Dockerfiles, Bicep infrastructure, CI/CD pipelines
- **Database engine**: Azure SQL Database (schema preserved as-is)
- **Deployment target**: Azure App Service
- **Monitoring**: Application Insights + Azure Monitor (retain
  existing instrumentation)
- **Security**: Connection strings via environment variables or
  Azure Key Vault; managed identity where supported; parameterized
  queries only

## Quality Gates and CI/CD Workflow

- The CI/CD pipeline MUST gate on **build success** and **all tests
  passing** before any deployment to QA, staging, or production.
- Pull requests MUST trigger an automated build and test run.
- Load tests (JMeter / Locust in `/loadtests`) MUST pass defined
  thresholds before production deployment.
- Chaos experiments in `/infra/chaos` SHOULD be validated in staging
  before production release.
- Code review by at least one team member is required for all PRs.

## Governance

- This constitution supersedes ad-hoc practices for the duration of
  the .NET 6 to .NET 10 migration.
- Amendments require: (1) a written proposal, (2) review by the
  migration lead, and (3) an updated version number following
  semantic versioning (MAJOR.MINOR.PATCH).
- Version policy:
  - **MAJOR**: Removal or redefinition of a core principle.
  - **MINOR**: Addition of a new principle or material expansion.
  - **PATCH**: Wording clarifications or typo fixes.
- All PRs and code reviews MUST verify compliance with these
  principles. Non-compliance MUST be flagged and resolved before
  merge.
- Use `AGENTS.md` and `.github/copilot-instructions.md` for runtime
  development guidance that supplements this constitution.

**Version**: 1.0.0 | **Ratified**: 2026-04-27 | **Last Amended**: 2026-04-27
