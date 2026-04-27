# Feature Specification: .NET 6 to .NET 10 LTS Migration

**Feature Branch**: `modernize/net6-to-net10`
**Created**: 2026-04-27
**Status**: Draft
**Input**: User description: "Modernize the Contoso University .NET 6 application to .NET 10 LTS. The app has 4 projects: a Razor Pages frontend, a Web API with EF Core and SQL Server, unit tests (MSTest), and Selenium UI tests. Currently deployed to Azure App Service (Windows). Need to update all target frameworks, NuGet packages, Dockerfiles, Bicep infrastructure, and CI/CD pipeline. Must maintain backward compatibility with existing Azure SQL database."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Framework and Package Upgrade (Priority: P1)

As a development team, we need all four solution projects upgraded from
`net6.0` to `net10.0` with compatible NuGet package versions so the
application compiles, runs, and passes all existing unit tests on the
.NET 10 LTS runtime.

**Why this priority**: Without a successful build on .NET 10, no other
migration work can proceed. This is the foundational gate for every
subsequent story.

**Independent Test**: Run `dotnet build src/ContosoUniversity.sln` and
`dotnet test src/ContosoUniversity.Test` on .NET 10 SDK. Both must
succeed with zero errors or warnings related to obsolete APIs.

**Acceptance Scenarios**:

1. **Given** the solution currently targets `net6.0`, **When** all
   `.csproj` files are updated to `net10.0` and NuGet packages are
   upgraded to .NET 10-compatible versions, **Then** `dotnet build`
   succeeds with no errors.
2. **Given** the upgraded solution, **When** the existing unit test
   project is executed, **Then** all tests pass without modification
   to test logic.
3. **Given** `global.json` references SDK 6.0.300, **When** it is
   updated to the .NET 10 LTS SDK version, **Then** the build toolchain
   resolves correctly.

---

### User Story 2 - Container Image Modernization (Priority: P2)

As an operations engineer, I need the Dockerfiles for both the
WebApplication and API projects updated to use official .NET 10
Microsoft container images so that deployed containers run on a
supported, patched runtime.

**Why this priority**: Once the code compiles on .NET 10, container
images must be updated before any deployment to Azure App Service
(Linux containers) or testing environments can occur.

**Independent Test**: Build each Dockerfile locally with
`docker build -t contoso-web ./src` and
`docker build -t contoso-api ./src`. Confirm containers start, respond
on port 80, and serve the homepage or Swagger endpoint.

**Acceptance Scenarios**:

1. **Given** the Dockerfiles reference `mcr.microsoft.com/dotnet/aspnet:6.0`
   and `sdk:6.0`, **When** updated to the .NET 10 equivalents, **Then**
   `docker build` succeeds for both projects.
2. **Given** a built container image, **When** started with
   `docker run -p 8080:80`, **Then** the application responds with
   HTTP 200 on the health-check endpoint.
3. **Given** the multi-stage Dockerfile, **When** the final image is
   inspected, **Then** it contains only the ASP.NET runtime (no SDK
   layer) and uses an official MCR base tag.

---

### User Story 3 - Infrastructure Alignment (Priority: P3)

As a platform engineer, I need the Bicep infrastructure templates and
CI/CD pipeline updated so that Azure App Service is configured for the
.NET 10 runtime stack and the pipeline builds/tests using the .NET 10
SDK.

**Why this priority**: After code and containers are validated locally,
the cloud deployment artifacts must align so that automated deployments
succeed end-to-end.

**Independent Test**: Deploy to a QA resource group using
`az deployment group create -g <rg> -f infra/core/main.bicep`. Verify
the App Service runtime shows `.NET 10` in the Azure Portal. Run the
CI pipeline on the feature branch and confirm build + test jobs pass.

**Acceptance Scenarios**:

1. **Given** the Bicep template configures App Service with the
   `DOTNET|6.0` runtime stack, **When** updated to `DOTNET|10.0`,
   **Then** `az deployment group what-if` shows the runtime change with
   no errors.
2. **Given** the GitHub Actions workflow installs .NET SDK 6.0, **When**
   updated to install .NET 10 SDK, **Then** the pipeline build and test
   jobs succeed.
3. **Given** the `docker-compose.yml` references .NET 6 images, **When**
   updated to .NET 10, **Then** `docker compose up` starts all services
   without errors.

---

### User Story 4 - Database Compatibility Verification (Priority: P4)

As a database administrator, I need confirmation that the upgraded
Entity Framework Core version works with the existing Azure SQL
database schema without requiring destructive migrations, ensuring
zero data loss and no downtime.

**Why this priority**: Database integrity is non-negotiable but depends
on the framework upgrade (US1) being complete first. This is the final
validation before production release.

**Independent Test**: Point the upgraded API at a copy of the production
database. Execute all CRUD operations (Students, Courses, Instructors,
Enrollments) and confirm no schema changes are generated by EF Core.

**Acceptance Scenarios**:

1. **Given** the API uses EF Core 7.0.4, **When** upgraded to the
   EF Core version compatible with .NET 10, **Then** the existing
   database context maps correctly to the unchanged schema.
2. **Given** the upgraded EF Core version, **When**
   `dotnet ef migrations has-pending-model-changes` is run, **Then**
   no pending model changes are detected.
3. **Given** the upgraded application, **When** the Bogus seeder
   (`DbInitializer`) runs against a fresh database, **Then** it
   populates all tables successfully.

---

### Edge Cases

- What happens if a NuGet package has no .NET 10-compatible version?
  The team must evaluate alternatives or pin to a compatible
  multi-target version and document the exception.
- What if EF Core detects model differences due to convention changes
  between versions? Explicit fluent configuration must be added to
  preserve the existing schema mapping.
- What if the Selenium WebDriver package version is incompatible with
  .NET 10? The CodedUITest project may require a version bump or
  alternative driver package while retaining test coverage.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: All four projects (`WebApplication`, `API`, `Test`,
  `CodedUITest`) MUST compile on `net10.0` without errors.
- **FR-002**: The solution MUST retain all existing functionality
  (Razor Pages routing, API endpoints, health checks, Swagger UI)
  with no behavioral regression.
- **FR-003**: The `ContosoUniversity.Test` unit tests MUST pass without
  modification to test logic (only framework/package references may
  change).
- **FR-004**: EF Core MUST connect to the existing Azure SQL database
  schema without requiring new migrations or schema alterations.
- **FR-005**: Dockerfiles MUST produce runnable Linux container images
  using official `mcr.microsoft.com/dotnet` .NET 10 base images for
  deployment to Linux App Service.
- **FR-006**: The `global.json` files MUST reference the .NET 10 LTS
  SDK version.
- **FR-007**: Bicep templates MUST configure Azure App Service for
  Linux containers with the `.NET 10` runtime stack.
- **FR-008**: The CI/CD pipeline MUST install the .NET 10 SDK and gate
  deployments on build success and test pass. Deployment MUST use slot
  swap for zero-downtime rollout.
- **FR-009**: Application Insights telemetry MUST continue to function
  after the upgrade.
- **FR-010**: The API MUST continue to use `async`/`await` for all
  database operations and dependency injection for all services.
- **FR-011**: The `docker-compose.yml` MUST be updated to reference
  .NET 10 container images.
- **FR-012**: Any deprecated .NET 6 APIs used in the codebase MUST be
  replaced with their .NET 10 equivalents.
- **FR-013**: `Newtonsoft.Json` in the WebApplication MUST be replaced
  with `System.Text.Json` while preserving all serialization behavior.
- **FR-014**: `NSwag.AspNetCore` in the API MUST be replaced with
  built-in ASP.NET Core OpenAPI (`Microsoft.AspNetCore.OpenApi`).

### Key Entities

- **Student**: Core academic entity with personal information and
  enrollment relationships. Schema must remain unchanged.
- **Course**: Academic offering with credit hours and department
  affiliation. Schema must remain unchanged.
- **Instructor**: Faculty member with office assignment and course
  assignments. Schema must remain unchanged.
- **Enrollment**: Junction entity linking students to courses with a
  grade. Schema must remain unchanged.
- **Department**: Organizational unit owning courses and employing
  instructors. Schema must remain unchanged.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: The full solution builds successfully on .NET 10 with
  zero compilation errors.
- **SC-002**: 100% of existing unit tests pass without modification to
  test assertions or logic.
- **SC-003**: Both container images build and start serving HTTP
  responses within 30 seconds.
- **SC-004**: The application connects to the existing Azure SQL
  database and performs CRUD operations with no schema changes required.
- **SC-005**: The CI/CD pipeline completes build and test stages
  successfully on the first run after migration.
- **SC-006**: Page load times remain within 10% of pre-migration
  baseline for all key endpoints (/, /Students, /Courses).
- **SC-007**: Application Insights continues to receive telemetry
  events after the upgrade.
- **SC-008**: No new security vulnerabilities are introduced by the
  package upgrades (verified by `dotnet list package --vulnerable`).

## Clarifications

### Session 2026-04-27

- Q: What is the target hosting model for .NET 10? → A: Linux App Service with containers (existing Dockerfiles)
- Q: Should Newtonsoft.Json be replaced with System.Text.Json? → A: Yes, replace per constitution principle V (built-in over third-party)
- Q: What is the rollback strategy for zero-downtime migration? → A: Deployment slot swap (blue-green via App Service slots)
- Q: How should the CodedUITest project be handled if incompatible? → A: Best-effort upgrade; exclude from CI gate if driver issues arise
- Q: Should NSwag be replaced with built-in ASP.NET Core OpenAPI? → A: Yes, replace per constitution principle V

## Assumptions

- The .NET 10 LTS SDK is available and stable for production use
  (released November 2025).
- All critical NuGet packages used by the solution have published
  .NET 10-compatible versions.
- The existing Azure SQL database schema is the source of truth; the
  application code adapts to it, not the reverse.
- The Selenium/CodedUITest project upgrade is best-effort; if driver
  compatibility issues arise, the project will be excluded from the
  CI gate while retaining the source for future resolution.
- Managed identity configuration for Azure SQL remains unchanged; only
  the runtime and SDK versions change.
- The `Newtonsoft.Json` dependency in the WebApplication WILL be
  replaced with `System.Text.Json` (built into .NET 10) per the
  "built-in over third-party" constitution principle. All serialization
  behavior must be preserved.
- `NSwag.AspNetCore` in the API project WILL be replaced with the
  built-in ASP.NET Core OpenAPI support (Microsoft.AspNetCore.OpenApi).
- The target deployment platform is **Linux App Service with containers**
  using the existing multi-stage Dockerfiles.
- Rollback strategy is **deployment slot swap** (blue-green) to achieve
  zero-downtime migration with instant rollback capability.
- Load test configurations in `/loadtests` do not require changes
  since they target HTTP endpoints, not the runtime itself.
