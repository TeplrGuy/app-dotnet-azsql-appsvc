# Tasks: .NET 6 to .NET 10 LTS Migration

**Input**: Design documents from `/specs/001-net6-to-net10-migration/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: Not explicitly requested in spec -- test tasks omitted. Existing tests must pass as validation gates.

**Organization**: Tasks grouped by user story (US1-US4) to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story (US1=Framework Upgrade, US2=Containers, US3=Infrastructure, US4=DB Compatibility)
- Exact file paths included in descriptions

---

## Phase 1: Setup

**Purpose**: SDK and toolchain configuration shared across all projects

- [ ] T001 Update global.json SDK version to 10.0.203 with latestPatch rollForward in src/ContosoUniversity.API/global.json
- [ ] T002 [P] Update global.json SDK version to 10.0.203 with latestPatch rollForward in src/ContosoUniversity.WebApplication/global.json

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Target framework changes that MUST complete before package upgrades or code changes

**CRITICAL**: No user story work can begin until TFMs are updated

- [ ] T003 Change TargetFramework from net6.0 to net10.0 in src/ContosoUniversity.API/ContosoUniversity.API.csproj
- [ ] T004 [P] Change TargetFramework from net6.0 to net10.0 in src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj
- [ ] T005 [P] Change TargetFramework from net6.0 to net10.0 in src/ContosoUniversity.Test/ContosoUniversity.Test.csproj
- [ ] T006 [P] Change TargetFramework from net6.0 to net10.0 in src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj

**Checkpoint**: All projects target net10.0 -- package upgrades can now proceed

---

## Phase 3: User Story 1 - Framework and Package Upgrade (Priority: P1) MVP

**Goal**: All four projects compile, run, and pass tests on .NET 10 LTS with compatible NuGet packages.

**Independent Test**: `dotnet build src/ContosoUniversity.sln && dotnet test src/ContosoUniversity.Test` both succeed with zero errors.

### API Project Packages

- [ ] T007 [US1] Upgrade Microsoft.EntityFrameworkCore packages (Design, SqlServer, Tools) to 10.0.7 in src/ContosoUniversity.API/ContosoUniversity.API.csproj
- [ ] T008 [US1] Upgrade Microsoft.Data.SqlClient from 5.0.1 to 7.0.1 in src/ContosoUniversity.API/ContosoUniversity.API.csproj
- [ ] T009 [US1] Upgrade Azure.Identity from 1.8.2 to 1.21.0 in src/ContosoUniversity.API/ContosoUniversity.API.csproj
- [ ] T010 [US1] Upgrade Microsoft.AspNetCore.Mvc.NewtonsoftJson to 10.0.7 in src/ContosoUniversity.API/ContosoUniversity.API.csproj
- [ ] T011 [US1] Upgrade NSwag.AspNetCore to latest 14.x in src/ContosoUniversity.API/ContosoUniversity.API.csproj
- [ ] T012 [US1] Upgrade Microsoft.VisualStudio.Web.CodeGeneration.Design to 10.0.2 in src/ContosoUniversity.API/ContosoUniversity.API.csproj
- [ ] T013 [US1] Remove Microsoft.AspNetCore.Razor.Language package from src/ContosoUniversity.API/ContosoUniversity.API.csproj
- [ ] T014 [US1] Remove System.Runtime.Extensions package from src/ContosoUniversity.API/ContosoUniversity.API.csproj

### API Code Modernization

- [ ] T015 [US1] Replace UseEndpoints(endpoints => endpoints.MapControllers()) with app.MapControllers() in src/ContosoUniversity.API/Program.cs

### WebApplication Project Packages

- [ ] T016 [P] [US1] Upgrade Microsoft.Extensions.DependencyInjection to 10.0.7 in src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj
- [ ] T017 [P] [US1] Upgrade Microsoft.Extensions.DependencyInjection.Abstractions to 10.0.7 in src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj
- [ ] T018 [P] [US1] Upgrade Newtonsoft.Json from 13.0.3 to 13.0.4 in src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj
- [ ] T019 [P] [US1] Upgrade Microsoft.VisualStudio.Web.CodeGeneration.Design to 10.0.2 in src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj
- [ ] T020 [P] [US1] Remove Microsoft.AspNetCore.Razor.Language package from src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj

### WebApplication Telemetry Replacement

- [ ] T021 [US1] Replace Microsoft.ApplicationInsights.AspNetCore with Azure.Monitor.OpenTelemetry.AspNetCore in src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj
- [ ] T022 [US1] Update telemetry registration in Program.cs: replace AddApplicationInsightsTelemetry() with UseAzureMonitor() in src/ContosoUniversity.WebApplication/Program.cs

### WebApplication Code Fix

- [ ] T023 [US1] Fix TimeSpan.FromSeconds integer ambiguity (use double literal e.g. 5.0) in src/ContosoUniversity.WebApplication/Program.cs

### Test Project Packages

- [ ] T024 [P] [US1] Upgrade MSTest packages to latest .NET 10-compatible versions in src/ContosoUniversity.Test/ContosoUniversity.Test.csproj
- [ ] T025 [P] [US1] Upgrade MSTest and Selenium packages to .NET 10-compatible versions in src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj
- [ ] T026 [US1] Fix TimeSpan.FromSeconds integer ambiguity (5 occurrences, use double literal) in src/ContosoUniversity.CodedUITest/DepartmentsTest.cs and NavigationMenuTest.cs

### Build Verification

- [ ] T027 [US1] Run dotnet build src/ContosoUniversity.sln --configuration Release and verify zero errors
- [ ] T028 [US1] Run dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj and verify all tests pass

**Checkpoint**: All 4 projects build and unit tests pass on .NET 10

---

## Phase 4: User Story 2 - Container Image Modernization (Priority: P2)

**Goal**: Dockerfiles produce runnable Linux containers using official .NET 10 MCR images.

**Independent Test**: `docker build` succeeds for both projects; containers start and respond on health endpoint.

- [ ] T029 [P] [US2] Update base images from aspnet:6.0/sdk:6.0 to aspnet:10.0/sdk:10.0 in src/ContosoUniversity.API/Dockerfile
- [ ] T030 [P] [US2] Update base images from aspnet:6.0/sdk:6.0 to aspnet:10.0/sdk:10.0 in src/ContosoUniversity.WebApplication/Dockerfile
- [ ] T031 [US2] Verify docker compose build succeeds for all services in docker-compose.yml

**Checkpoint**: Both containers build and start serving HTTP responses

---

## Phase 5: User Story 3 - Infrastructure Alignment (Priority: P3)

**Goal**: Bicep templates and CI/CD pipeline target .NET 10 runtime and SDK.

**Independent Test**: `az deployment group what-if` shows runtime change; pipeline build+test jobs pass.

- [ ] T032 [US3] Update all 4 netFrameworkVersion values from 'v6.0' to 'v10.0' in infra/resources.bicep
- [ ] T033 [P] [US3] Update DOTNET_VERSION from '6.0.x' to '10.0.x' in .github/workflows/resilience-pipeline.yml

**Checkpoint**: Infrastructure and CI/CD aligned with .NET 10

---

## Phase 6: User Story 4 - Database Compatibility Verification (Priority: P4)

**Goal**: Confirm EF Core 10.0.7 works with existing schema without destructive migrations.

**Independent Test**: `dotnet ef migrations has-pending-model-changes` returns no pending changes; CRUD operations succeed.

- [ ] T034 [US4] Run dotnet ef migrations has-pending-model-changes in src/ContosoUniversity.API and verify clean output
- [ ] T035 [US4] Verify API starts and connects to database: run application and test /health endpoint

**Checkpoint**: Database compatibility confirmed -- zero schema changes

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup across all stories

- [ ] T036 [P] Run dotnet list package --vulnerable on solution and confirm no new vulnerabilities
- [ ] T037 [P] Verify Swagger UI loads at /swagger on running API container
- [ ] T038 Run full docker compose up and verify all services healthy
- [ ] T039 Run quickstart.md validation steps end-to-end

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies -- start immediately
- **Foundational (Phase 2)**: Depends on Setup (Phase 1) completion -- BLOCKS all user stories
- **US1 Framework Upgrade (Phase 3)**: Depends on Foundational (Phase 2) -- BLOCKS US2, US3, US4
- **US2 Containers (Phase 4)**: Depends on US1 completion (needs updated csproj and code)
- **US3 Infrastructure (Phase 5)**: Depends on US1 completion (runtime version must match)
- **US4 DB Verification (Phase 6)**: Depends on US1 completion (needs EF Core 10.0.7 installed)
- **Polish (Phase 7)**: Depends on US1-US4 completion

### User Story Dependencies

- **US1 (P1)**: Blocks all other stories -- must complete first
- **US2 (P2)**: Can start after US1; independent of US3, US4
- **US3 (P3)**: Can start after US1; independent of US2, US4
- **US4 (P4)**: Can start after US1; independent of US2, US3

### Within User Story 1

- Package upgrades (T007-T014) before code changes (T015)
- API packages before WebApp packages (dependency order)
- WebApp packages (T016-T020) can run in parallel with API code modernization
- Telemetry replacement (T021-T022) sequential (package then code)
- Test project upgrades (T024-T026) can run in parallel with WebApp work
- Build verification (T027-T028) must be last in Phase 3

### Parallel Opportunities

```text
# After Phase 2, within US1:
Parallel: T016, T017, T018, T019, T020 (WebApp packages -- different lines in same file)
Parallel: T024, T025 (Test project packages -- different files)

# After US1 completion:
Parallel: T029, T030 (Dockerfiles -- different files)
Parallel: T032, T033 (Infrastructure -- different files)
Parallel: T034 with T029/T030 (DB check vs Docker builds)
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T002)
2. Complete Phase 2: Foundational TFM changes (T003-T006)
3. Complete Phase 3: User Story 1 -- Framework and Package Upgrade (T007-T028)
4. **STOP and VALIDATE**: `dotnet build && dotnet test` passes
5. Deploy/demo if ready -- app runs on .NET 10

### Incremental Delivery After MVP

6. Phase 4: User Story 2 -- Container images (T029-T031)
7. Phase 5: User Story 3 -- Infrastructure alignment (T032-T033)
8. Phase 6: User Story 4 -- Database verification (T034-T035)
9. Phase 7: Polish and cross-cutting validation (T036-T039)

### Suggested MVP Scope

User Story 1 alone delivers a fully functional .NET 10 application that builds and passes all tests. Stories 2-4 are deployment/verification concerns that can follow in a second pass.
