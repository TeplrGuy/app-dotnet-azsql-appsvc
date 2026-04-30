# Tasks: .NET 6 to .NET 10 LTS Migration

**Input**: Design documents from `/specs/001-net6-to-net10-migration/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Not explicitly requested in the spec. Test tasks are limited to running existing tests for validation.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: SDK version update that enables all subsequent project migrations

- [ ] T001 Update SDK version in `src/ContosoUniversity.API/global.json` to 10.0.100
- [ ] T002 [P] Update SDK version in `src/ContosoUniversity.WebApplication/global.json` to 10.0.100

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Migrate the API project first since it owns the data layer and EF Core -- all other projects depend on it being on .NET 10

**CRITICAL**: The solution will NOT build until both API and WebApplication are migrated (Steps 2-3 in dependency graph)

- [ ] T003 [US1] Update `TargetFramework` to `net10.0` and add `Nullable` + `ImplicitUsings` properties in `src/ContosoUniversity.API/ContosoUniversity.API.csproj`
- [ ] T004 [US2] Update NuGet packages to .NET 10 versions in `src/ContosoUniversity.API/ContosoUniversity.API.csproj` per contracts/package-versions.md
- [ ] T005 [US2] Remove obsolete packages (NSwag, Newtonsoft, Razor.Language, CodeGeneration.Design, System.Runtime.Extensions) from `src/ContosoUniversity.API/ContosoUniversity.API.csproj`
- [ ] T006 [US2] Add new packages (Swashbuckle.AspNetCore.SwaggerUI) to `src/ContosoUniversity.API/ContosoUniversity.API.csproj`
- [ ] T007 [US1] Replace `UseEndpoints(endpoints => { endpoints.MapControllers(); })` with `app.MapControllers()` in `src/ContosoUniversity.API/Program.cs`
- [ ] T008 [US2] Replace NSwag OpenAPI registration (`app.UseOpenApi(); app.UseSwaggerUi3()`) with built-in OpenAPI (`builder.Services.AddOpenApi(); app.MapOpenApi(); app.UseSwaggerUI(...)`) in `src/ContosoUniversity.API/Program.cs`
- [ ] T009 [US2] Replace any `Newtonsoft.Json` using directives and `[JsonProperty]` attributes with `System.Text.Json` equivalents across `src/ContosoUniversity.API/` source files
- [ ] T010 [US1] Update `TargetFramework` to `net10.0` and add `Nullable` + `ImplicitUsings` properties in `src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj`
- [ ] T011 [US2] Remove obsolete packages (Razor.Language, DependencyInjection, DependencyInjection.Abstractions, CodeGeneration.Design, Newtonsoft.Json) from `src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj`
- [ ] T012 [US2] Update Microsoft.ApplicationInsights.AspNetCore to 2.22.0 in `src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj`
- [ ] T013 [US2] Replace any `Newtonsoft.Json` using directives with `System.Text.Json` equivalents in `src/ContosoUniversity.WebApplication/` source files
- [ ] T014 [US1] Verify solution builds: run `dotnet build src/ContosoUniversity.sln` and resolve any compilation errors

**Checkpoint**: API and WebApplication projects compile on .NET 10. Solution builds successfully.

---

## Phase 3: User Story 1 - Update Target Frameworks and SDK (Priority: P1) MVP

**Goal**: All four projects compile and run on .NET 10

**Independent Test**: `dotnet build src/ContosoUniversity.sln` succeeds with zero errors

### Implementation for User Story 1

- [ ] T015 [US1] Update `TargetFramework` to `net10.0` and add `Nullable` + `ImplicitUsings` in `src/ContosoUniversity.Test/ContosoUniversity.Test.csproj`
- [ ] T016 [P] [US1] Update `TargetFramework` to `net10.0` and add `Nullable` + `ImplicitUsings` in `src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj`
- [ ] T017 [US1] Fix any nullable reference type warnings introduced by enabling `Nullable` across all four projects
- [ ] T018 [US1] Verify full solution build: `dotnet build src/ContosoUniversity.sln --configuration Release` passes with zero errors

**Checkpoint**: All four projects target net10.0 and build clean.

---

## Phase 4: User Story 2 - Upgrade NuGet Dependencies (Priority: P1)

**Goal**: All NuGet packages are .NET 10-compatible with no version conflicts

**Independent Test**: `dotnet restore src/ContosoUniversity.sln && dotnet build` with zero package warnings

### Implementation for User Story 2

- [ ] T019 [US2] Update MSTest packages (TestAdapter, TestFramework to 3.7.0, Test.Sdk to 17.12.0, coverlet to 6.0.2) in `src/ContosoUniversity.Test/ContosoUniversity.Test.csproj`
- [ ] T020 [P] [US2] Update MSTest packages and Selenium.WebDriver to 4.27.0, remove Selenium.WebDriver.ChromeDriver in `src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj`
- [ ] T021 [US2] Run `dotnet restore src/ContosoUniversity.sln` and verify zero package-compatibility warnings
- [ ] T022 [US2] Run `dotnet build src/ContosoUniversity.sln --configuration Release` and verify zero errors

**Checkpoint**: All NuGet packages resolved to .NET 10-compatible versions. Full solution builds.

---

## Phase 5: User Story 3 - Pass Existing Test Suite (Priority: P1)

**Goal**: All existing unit tests pass on .NET 10 confirming no regressions

**Independent Test**: `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj` -- all green

### Implementation for User Story 3

- [ ] T023 [US3] Run `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj --configuration Release` and identify any failures
- [ ] T024 [US3] Fix any test failures caused by MSTest v3 API changes or .NET 10 behavior differences in `src/ContosoUniversity.Test/` test files
- [ ] T025 [US3] Verify all tests pass: `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj --verbosity normal` reports 100% pass

**Checkpoint**: Test gate passes. All existing unit tests green on .NET 10.

---

## Phase 6: User Story 4 - Update Dockerfiles (Priority: P2)

**Goal**: Both Dockerfiles use .NET 10 official images and produce runnable containers

**Independent Test**: `docker build` succeeds for both images; containers respond on port 80

### Implementation for User Story 4

- [ ] T026 [P] [US4] Update base image to `mcr.microsoft.com/dotnet/aspnet:10.0`, SDK image to `mcr.microsoft.com/dotnet/sdk:10.0`, add `ENV ASPNETCORE_URLS=http://+:80` in `src/ContosoUniversity.API/Dockerfile`
- [ ] T027 [P] [US4] Update base image to `mcr.microsoft.com/dotnet/aspnet:10.0`, SDK image to `mcr.microsoft.com/dotnet/sdk:10.0`, add `ENV ASPNETCORE_URLS=http://+:80` in `src/ContosoUniversity.WebApplication/Dockerfile`
- [ ] T028 [US4] Verify Docker builds: `docker build -f src/ContosoUniversity.API/Dockerfile -t contoso-api:net10 src/` succeeds
- [ ] T029 [US4] Verify Docker builds: `docker build -f src/ContosoUniversity.WebApplication/Dockerfile -t contoso-web:net10 src/` succeeds

**Checkpoint**: Both Docker images build and run on .NET 10 runtime.

---

## Phase 7: User Story 5 - Update Bicep Infrastructure (Priority: P2)

**Goal**: Bicep templates reference .NET 10 runtime stack for App Service

**Independent Test**: `az bicep build --file infra/main.bicep` succeeds; output contains `v10.0`

### Implementation for User Story 5

- [ ] T030 [US5] Replace all occurrences of `netFrameworkVersion: 'v6.0'` with `netFrameworkVersion: 'v10.0'` in `infra/resources.bicep` (4 occurrences at lines 210, 284, 321, 358)
- [ ] T031 [US5] Validate Bicep: run `az bicep build --file infra/main.bicep` and confirm zero errors

**Checkpoint**: Infrastructure aligned with .NET 10 target framework.

---

## Phase 8: User Story 6 - Update CI/CD Pipeline (Priority: P2)

**Goal**: GitHub Actions pipeline uses .NET 10 SDK for build and test

**Independent Test**: Pipeline triggered on branch installs .NET 10 SDK and all steps succeed

### Implementation for User Story 6

- [ ] T032 [US6] Change `DOTNET_VERSION: '6.0.x'` to `DOTNET_VERSION: '10.0.x'` in `.github/workflows/resilience-pipeline.yml`
- [ ] T033 [US6] Review other workflow files for .NET version references and update if found in `.github/workflows/`

**Checkpoint**: CI/CD pipeline configured for .NET 10.

---

## Phase 9: User Story 7 - Update Selenium UI Test Project (Priority: P3)

**Goal**: CodedUITest project compiles on .NET 10 with updated Selenium packages

**Independent Test**: `dotnet build src/ContosoUniversity.CodedUITest/` succeeds; Selenium tests run

### Implementation for User Story 7

- [ ] T034 [US7] Verify CodedUITest project builds: `dotnet build src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj`
- [ ] T035 [US7] Fix any Selenium API changes due to WebDriver 4.27 in `src/ContosoUniversity.CodedUITest/` test files (if compilation errors exist)

**Checkpoint**: Selenium test project compiles and is ready for execution against running app.

---

## Phase 10: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup across all user stories

- [ ] T036 [P] Remove any remaining `using Newtonsoft.Json` statements across the entire `src/` directory
- [ ] T037 [P] Remove redundant `using` directives made unnecessary by `ImplicitUsings` across all projects
- [ ] T037b [FR-011] Scan for remaining deprecated .NET 6 APIs: run `dotnet build src/ContosoUniversity.sln /p:TreatWarningsAsErrors=true` and fix any `SYSLIB`/`NETSDK` obsolescence warnings in `src/ContosoUniversity.API/` and `src/ContosoUniversity.WebApplication/`
- [ ] T038 Run full solution build with warnings-as-errors: `dotnet build src/ContosoUniversity.sln -warnaserrors`
- [ ] T039 Run complete test suite: `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj --configuration Release`
- [ ] T040 Run quickstart.md validation steps 1-6 (SDK, build, test, API run, WebApp run, EF schema verify)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies -- can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 (SDK version must be set first) -- BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Phase 2 -- test projects need TFM update after API+WebApp build
- **User Story 2 (Phase 4)**: Depends on Phase 3 -- packages update for test projects after TFM set
- **User Story 3 (Phase 5)**: Depends on Phase 4 -- tests can only run after packages are compatible
- **User Stories 4-6 (Phases 6-8)**: Depend on Phase 5 (tests passing) -- can proceed in parallel with each other
- **User Story 7 (Phase 9)**: Depends on Phase 4 (CodedUITest already updated in Phase 4)
- **Polish (Phase 10)**: Depends on all previous phases

### User Story Dependencies

- **US1 (TFM+SDK)**: Foundation -- all other stories depend on this
- **US2 (NuGet)**: Depends on US1 (TFM must be set before packages resolve correctly)
- **US3 (Tests)**: Depends on US2 (packages must be updated for tests to compile)
- **US4 (Docker)**: Depends on US3 (app must build+test before containerizing)
- **US5 (Bicep)**: Independent of US4 but logically follows US3
- **US6 (CI/CD)**: Independent of US4/US5 but logically follows US3
- **US7 (Selenium)**: Depends on US2 (packages updated in Phase 4)

### Parallel Opportunities

- T001 and T002 (global.json files) -- different files, parallel
- T015 and T016 (Test and CodedUITest TFM) -- different files, parallel
- T019 and T020 (Test and CodedUITest packages) -- different files, parallel
- T026 and T027 (Dockerfiles) -- different files, parallel
- T036 and T037 (cleanup tasks) -- different concerns, parallel
- Phases 6, 7, 8 (Docker, Bicep, Pipeline) -- independent infrastructure, parallel

---

## Parallel Example: Phases 6-8

```bash
# These three phases can execute in parallel after Phase 5 passes:

# Worker A: Dockerfiles (Phase 6)
# T026: Update API Dockerfile
# T027: Update WebApp Dockerfile
# T028-T029: Verify builds

# Worker B: Bicep (Phase 7)
# T030: Update resources.bicep
# T031: Validate

# Worker C: Pipeline (Phase 8)
# T032: Update resilience-pipeline.yml
# T033: Review other workflows
```

---

## Implementation Strategy

### MVP Scope

Phase 1 + Phase 2 + Phase 3 = Minimum viable migration (all projects build and tests pass on .NET 10). This is the critical path and must be completed first.

### Incremental Delivery

1. **Increment 1** (P1 -- MVP): Phases 1-5 (SDK + TFM + NuGet + Tests passing)
2. **Increment 2** (P2 -- Deployment ready): Phases 6-8 (Docker + Bicep + CI/CD) -- parallelizable
3. **Increment 3** (P3 -- Full validation): Phase 9-10 (Selenium + Polish)
