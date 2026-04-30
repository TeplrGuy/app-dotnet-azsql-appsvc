# Feature Specification: .NET 6 to .NET 10 LTS Migration

**Feature Branch**: `001-net6-to-net10-migration-spec`
**Created**: 2026-04-27
**Status**: Draft
**Input**: Modernize the Contoso University .NET 6 application to .NET 10 LTS. The app has 4 projects: a Razor Pages frontend, a Web API with EF Core and SQL Server, unit tests (MSTest), and Selenium UI tests. Currently deployed to Azure App Service (Windows). Need to update all target frameworks, NuGet packages, Dockerfiles, Bicep infrastructure, and CI/CD pipeline. Must maintain backward compatibility with existing Azure SQL database.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Update Target Frameworks and SDK (Priority: P1)

As a development team, we need all four projects to compile and run on .NET 10 so the application benefits from long-term support, security patches, and modern runtime performance.

**Why this priority**: The target framework and SDK updates are the foundation; nothing else can proceed until the projects build on .NET 10.

**Independent Test**: Build all four projects with `dotnet build src/ContosoUniversity.sln` targeting `net10.0` and confirm zero errors.

**Acceptance Scenarios**:

1. **Given** the solution contains four `.csproj` files targeting `net6.0`, **When** all `TargetFramework` values are changed to `net10.0` and `global.json` files are updated to the .NET 10 SDK, **Then** `dotnet build src/ContosoUniversity.sln` succeeds with zero errors.
2. **Given** the updated solution, **When** `dotnet run --project src/ContosoUniversity.WebApplication` is executed, **Then** the application starts and the home page loads successfully.
3. **Given** the updated solution, **When** `dotnet run --project src/ContosoUniversity.API` is executed, **Then** the API starts and the Swagger endpoint returns a valid response.

---

### User Story 2 - Upgrade NuGet Dependencies (Priority: P1)

As a development team, we need all NuGet packages upgraded to versions compatible with .NET 10 so the application compiles without package-version conflicts and benefits from the latest fixes.

**Why this priority**: Outdated packages block compilation on .NET 10 and may contain known vulnerabilities.

**Independent Test**: Run `dotnet restore src/ContosoUniversity.sln` followed by `dotnet build` and confirm all packages resolve without downgrade warnings or version conflicts.

**Acceptance Scenarios**:

1. **Given** the solution references .NET 6-era NuGet packages, **When** packages are updated to .NET 10-compatible versions, **Then** `dotnet restore` and `dotnet build` complete with zero warnings related to package incompatibility.
2. **Given** EF Core is updated to version 10.x, **When** the API project connects to the existing Azure SQL database, **Then** all CRUD operations work identically to the .NET 6 version.
3. **Given** Microsoft.ApplicationInsights.AspNetCore is updated, **When** the application runs, **Then** telemetry continues to flow to Application Insights without configuration changes.

---

### User Story 3 - Pass Existing Test Suite (Priority: P1)

As a development team, we need all existing unit tests to pass on .NET 10 so we have confidence the migration does not introduce regressions.

**Why this priority**: The test gate is a non-negotiable governance constraint; no merge is permitted with failing tests.

**Independent Test**: Run `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj` and confirm all tests pass.

**Acceptance Scenarios**:

1. **Given** the unit test project targets `net10.0` with updated MSTest packages, **When** `dotnet test` is run, **Then** all existing tests pass with the same assertions as before.
2. **Given** any test required signature or API changes due to .NET 10 evolution, **When** those tests are updated, **Then** the updated tests verify the same behavior as the original tests.

---

### User Story 4 - Update Dockerfiles (Priority: P2)

As a deployment engineer, we need both Dockerfiles to use official .NET 10 base images so container builds produce images running the correct runtime.

**Why this priority**: Containers must match the application runtime; however, local development and CI can proceed without Docker changes first.

**Independent Test**: Build each Docker image and run it locally, confirming the application starts and responds on port 80.

**Acceptance Scenarios**:

1. **Given** the WebApplication Dockerfile references `mcr.microsoft.com/dotnet/aspnet:6.0` and `sdk:6.0`, **When** both tags are updated to their `.NET 10` equivalents, **Then** `docker build` succeeds and produces a runnable image.
2. **Given** the API Dockerfile references `mcr.microsoft.com/dotnet/aspnet:6.0` and `sdk:6.0`, **When** both tags are updated to their `.NET 10` equivalents, **Then** `docker build` succeeds and produces a runnable image.
3. **Given** both updated images, **When** `docker-compose up` is run, **Then** both services start and respond to HTTP requests.

---

### User Story 5 - Update Bicep Infrastructure (Priority: P2)

As an infrastructure engineer, we need the Bicep templates to reference the .NET 10 runtime stack so Azure App Service runs the correct framework version.

**Why this priority**: Infrastructure alignment is required but can trail the application code update.

**Independent Test**: Run `az bicep build` on the updated templates and validate the runtime stack parameter matches `.NET 10`.

**Acceptance Scenarios**:

1. **Given** Bicep templates specify a .NET 6 runtime stack for App Service, **When** the runtime reference is updated to .NET 10, **Then** `az bicep build` succeeds with no errors.
2. **Given** the updated Bicep templates, **When** deployed to a test resource group, **Then** the App Service Configuration shows the .NET 10 runtime stack.

---

### User Story 6 - Update CI/CD Pipeline (Priority: P2)

As a DevOps engineer, we need the GitHub Actions workflow to use the .NET 10 SDK so builds and tests run against the correct framework.

**Why this priority**: CI must match the application's target framework to enforce the quality gates.

**Independent Test**: Trigger the pipeline on the feature branch and confirm it installs .NET 10 SDK, builds, and runs tests successfully.

**Acceptance Scenarios**:

1. **Given** the resilience pipeline sets `DOTNET_VERSION: '6.0.x'`, **When** the value is changed to `'10.0.x'`, **Then** the pipeline installs the .NET 10 SDK and all build/test steps succeed.
2. **Given** the updated pipeline, **When** a PR is opened, **Then** the build and test gates pass before the PR is mergeable.

---

### User Story 7 - Update Selenium UI Test Project (Priority: P3)

As a QA engineer, we need the Selenium CodedUITest project to target .NET 10 with updated Selenium packages so UI tests can run against the migrated application.

**Why this priority**: UI tests provide end-to-end validation but are not blocking for the core migration; they run after the application is functional on .NET 10.

**Independent Test**: Run the Selenium test suite against a locally running .NET 10 instance and confirm tests pass.

**Acceptance Scenarios**:

1. **Given** the CodedUITest project targets `net6.0` with Selenium.WebDriver 4.1.1, **When** the target framework is updated to `net10.0` and Selenium packages are upgraded, **Then** the project compiles without errors.
2. **Given** the migrated application is running locally, **When** the Selenium tests execute, **Then** navigation and department tests pass.

---

### Edge Cases

- What happens if a NuGet package used in .NET 6 has no .NET 10-compatible version? The package must be replaced with a .NET 10 built-in alternative or a compatible fork, documented in the PR.
- What happens if EF Core 10 changes default conventions (e.g., pluralization, cascade delete) that break the existing schema? EF model configuration must explicitly set conventions to match the current database schema.
- What happens if the Selenium ChromeDriver version is incompatible with the installed Chrome version? The ChromeDriver package must be updated to match the CI/local Chrome version, or a version-agnostic driver management approach must be used.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: All four `.csproj` files MUST target `net10.0`.
- **FR-002**: Both `global.json` files MUST reference the .NET 10 SDK version.
- **FR-003**: All NuGet package references MUST be updated to versions compatible with .NET 10.
- **FR-004**: EF Core MUST be updated to version 10.x while maintaining full compatibility with the existing Azure SQL database schema (no column, table, or type changes).
- **FR-005**: The `Newtonsoft.Json` dependency MUST be evaluated for replacement with `System.Text.Json`; if not feasible in this migration, retention MUST be documented with justification.
- **FR-006**: Both Dockerfiles MUST use official `mcr.microsoft.com/dotnet/aspnet:10.0` and `sdk:10.0` base images in multi-stage builds.
- **FR-007**: Bicep templates MUST reference the .NET 10 runtime stack for App Service configuration.
- **FR-008**: The CI/CD pipeline MUST set the .NET SDK version to `10.0.x` and all quality gates (build, test) MUST pass.
- **FR-009**: All existing unit tests in `ContosoUniversity.Test` MUST pass after the migration.
- **FR-010**: The Selenium UI test project MUST compile on `net10.0` with updated driver packages.
- **FR-011**: Deprecated .NET 6 APIs used in the codebase MUST be replaced with their .NET 10 equivalents.
- **FR-012**: The application MUST continue to use dependency injection and `async`/`await` for all I/O operations (no synchronous blocking).

### Key Entities

- **Solution**: `ContosoUniversity.sln` containing four projects -- the central build artifact.
- **WebApplication project**: Razor Pages frontend (`ContosoUniversity.WebApplication.csproj`) -- serves the student/course/instructor UI.
- **API project**: Web API with EF Core (`ContosoUniversity.API.csproj`) -- provides REST endpoints and database access.
- **Test project**: MSTest unit tests (`ContosoUniversity.Test.csproj`) -- regression safety net.
- **CodedUITest project**: Selenium browser tests (`ContosoUniversity.CodedUITest.csproj`) -- end-to-end UI validation.
- **Dockerfiles**: Two multi-stage build files defining container images for WebApplication and API.
- **Bicep templates**: Infrastructure as Code in `infra/` defining Azure App Service, SQL, and supporting resources.
- **CI/CD pipeline**: `resilience-pipeline.yml` defining build, test, and deploy stages.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All four projects build successfully targeting .NET 10 with zero errors and zero package-compatibility warnings.
- **SC-002**: 100% of existing unit tests pass after migration with no test removals.
- **SC-003**: The application starts and serves all key pages (Home, Students, Courses, Enrollments) within 5 seconds of launch.
- **SC-004**: Both Docker images build and run successfully, responding to HTTP requests on the expected port.
- **SC-005**: The CI/CD pipeline completes the build-and-test stage without failures on the .NET 10 SDK.
- **SC-006**: Bicep templates validate and deploy with the .NET 10 runtime stack configured on App Service.
- **SC-007**: No data loss or schema changes occur in the Azure SQL database as a result of the migration.
- **SC-008**: Application performance (page load times, API response times) does not degrade beyond 10% compared to the .NET 6 baseline under equivalent load (validated post-deployment via existing Azure Load Testing configuration; no new test infrastructure required).

## Assumptions

- The .NET 10 SDK and runtime are available as a stable LTS release at the time of implementation.
- The existing Azure SQL database schema will not be modified as part of this migration; EF Core model configuration will be adjusted to match the existing schema if default conventions change.
- The `Newtonsoft.Json` dependency in the API project may need to be retained temporarily if serialization contracts depend on Newtonsoft-specific attributes; a documented justification is acceptable per the constitution.
- Selenium WebDriver packages have .NET 10-compatible versions available; if not, the CodedUITest project upgrade may be deferred to a follow-up task.
- The CI/CD pipeline runs on GitHub-hosted runners that support the .NET 10 SDK.
- Load testing and chaos testing configurations do not require changes as part of the framework migration (they test deployed endpoints, not framework internals).
- The existing `docker-compose.yml` at the repo root will be updated to reference the new images but no architectural changes to the compose setup are required.
