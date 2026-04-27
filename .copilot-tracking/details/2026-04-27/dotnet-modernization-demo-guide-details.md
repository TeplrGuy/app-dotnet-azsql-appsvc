<!-- markdownlint-disable-file -->
# Implementation Details: .NET 6 to .NET 10 Modernization Demo Guide

## Context Reference

Sources: .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md, .copilot-tracking/research/subagents/2026-04-27/project-current-state-research.md, .copilot-tracking/research/subagents/2026-04-27/modernize-dotnet-plugin-research.md, .copilot-tracking/research/subagents/2026-04-27/infra-and-migration-targets-research.md

## Implementation Phase 1: Create Demo Guide Document

<!-- parallelizable: false -->

### Step 1.1: Create `docs/demo-guide.md` with frontmatter, session metadata, and time budget table

Create a new file at `docs/demo-guide.md`. Include YAML frontmatter with title, description, author, ms.date, and ms.topic fields. Add the session title "GitHub Copilot for Application Modernization: Migrating with Confidence" and session abstract. Include the time budget table showing all 5 phases with durations and priority (must-show vs. nice-to-have).

Files:
* docs/demo-guide.md - New file: complete demo guide document

Content structure for this step:

```markdown
---
title: ".NET Modernization Demo Guide"
description: "30-minute live demo guide for modernizing Contoso University from .NET 6 to .NET 10 LTS using Spec Kit and modernize-dotnet"
author: Microsoft
ms.date: 2026-04-27
ms.topic: tutorial
---

## Session: GitHub Copilot for Application Modernization

**Title**: GitHub Copilot for Application Modernization: Migrating with Confidence

**Abstract**: Accelerate legacy modernization with AI-assisted refactoring. This session explores how GitHub Copilot supports .NET migrations, framework upgrades, dependency updates, and architectural transformation. We'll walk through modernization patterns including containerization and cloud readiness aligned to Microsoft guidance.

**Duration**: 30 minutes

## Time Budget

| Phase | Duration | Priority |
|-------|----------|----------|
| Pre-Demo Setup | Before demo | Required |
| Phase 1: Discovery and Assessment | ~5 min | Must-show |
| Phase 2: Planning | ~5 min | Must-show |
| Phase 3: Code Migration | ~10 min | Must-show |
| Phase 4: Containerization and Cloud Readiness | ~5 min | Nice-to-have |
| Phase 5: Validation and Deployment | ~5 min | Nice-to-have |

> **Time pressure tip**: Phases 1-3 are the core demo (20 min). Phases 4-5 can be talked through quickly or skipped if running short.
```

Success criteria:
* File exists at docs/demo-guide.md
* YAML frontmatter passes schema validation
* Time budget table shows all 5 phases with priority annotations

Context references:
* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 82-102) - Time budget and session metadata

Dependencies:
* None (first step)

### Step 1.2: Write Pre-Demo Setup section with environment verification commands and repo setup

Add the Pre-Demo Setup section to docs/demo-guide.md covering 4 subsections: Environment Verification (dotnet --list-sdks, specify --version, modernize-dotnet plugin check), Repository Setup (clone repo, verify build), VS Code Configuration (Copilot active, chat panel open, terminal visible), and Branch Creation (git checkout -b modernize/net6-to-net10).

Files:
* docs/demo-guide.md - Append pre-demo setup section

Key commands to include:

```powershell
# 1. Environment verification
dotnet --list-sdks                    # Must show 10.0.x
specify --version                     # Expected: 0.8.1
# Check modernize-dotnet in VS Code Extensions sidebar

# 2. Repository setup
git clone https://github.com/TeplrGuy/app-dotnet-azsql-appsvc.git
cd app-dotnet-azsql-appsvc
dotnet build src/ContosoUniversity.sln  # Verify .NET 6 build works

# 3. VS Code config
# - GitHub Copilot active (status bar)
# - Copilot Chat panel open
# - Terminal panel visible
# - Azure portal tab open (optional)

# 4. Branch creation
git checkout -b modernize/net6-to-net10
```

Success criteria:
* Pre-demo section covers all 4 subsections
* All verification commands are copy-pasteable
* Expected output is documented for each command

Context references:
* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 104-138) - Pre-demo setup details

Dependencies:
* Step 1.1 completion

### Step 1.3: Write Phase 1 — Discovery and Assessment (~5 min) with Spec Kit /specify and modernize-dotnet assessment

Add Phase 1 to docs/demo-guide.md. This phase has two sub-steps:

**Step 1A: Spec Kit /specify** — Define the modernization specification. Include the exact /specify prompt that describes the 4-project solution, target .NET 10, and all update areas (frameworks, NuGet, Dockerfiles, Bicep, CI/CD). Note that this creates artifacts in `.specify/`.

**Step 1B: modernize-dotnet assessment** — Run the plugin assessment. Include the `@modernize-dotnet` chat prompt. Document what happens behind the scenes (get_state, get_scenarios, get_instructions, initialize_scenario, generate_dotnet_upgrade_assessment). List what to show the audience: assessment report, NuGet compatibility, breaking changes, "16+ version references across 12 files" key stat.

Talking points to include:
* "This is a real .NET 6 app running on an UNSUPPORTED runtime since Nov 2024"
* "Let's use AI to assess what needs to change — no manual file hunting"
* "In under a minute, Copilot identified every file, every package, and every breaking change"

Files:
* docs/demo-guide.md - Append Phase 1 section

Discrepancy references:
* DR-01: The Spec Kit /specify command may produce different artifacts depending on the Spec Kit version and configuration; the guide should note this variability

Success criteria:
* Phase 1 has both 1A (Spec Kit) and 1B (modernize-dotnet) sub-steps
* Exact copy-pasteable prompts for both tools
* Behind-the-scenes explanation for modernize-dotnet assessment
* Presenter talking points included

Context references:
* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 140-185) - Phase 1 details
* .copilot-tracking/research/subagents/2026-04-27/modernize-dotnet-plugin-research.md (Lines 120-155) - Assessment workflow

Dependencies:
* Step 1.2 completion

### Step 1.4: Write Phase 2 — Planning (~5 min) with Spec Kit /plan, /tasks, and modernize-dotnet upgrade plan

Add Phase 2 to docs/demo-guide.md with three sub-steps:

**Step 2A: Spec Kit /plan** — Create the migration plan. Include the /plan prompt covering dependency-order upgrade (API first, then WebApp, then tests), NuGet updates, endpoint routing modernization, Dockerfile/Bicep/CI updates.

**Step 2B: Spec Kit /tasks** — Generate the ordered task list. Show the `tasks.md` output as the "flight plan."

**Step 2C: modernize-dotnet plan review** — Show the plugin's generated plan in `.github/upgrades/{scenarioId}/tasks.md`. Highlight the topological ordering of project upgrades.

Talking points:
* "Now we know WHAT needs to change. Let's plan HOW to do it safely."
* "The modernize-dotnet plugin creates a task-by-task execution plan in dependency order"
* "We have two complementary plans — Spec Kit gives the architecture view, modernize-dotnet gives the file-level execution plan"

Files:
* docs/demo-guide.md - Append Phase 2 section

Success criteria:
* Phase 2 has all three sub-steps (2A, 2B, 2C)
* Exact copy-pasteable prompts
* Explains the complementary nature of both tools

Context references:
* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 187-222) - Phase 2 details

Dependencies:
* Step 1.3 completion

### Step 1.5: Write Phase 3 — Code Migration (~10 min) with modernize-dotnet task execution and before/after code samples

Add Phase 3 to docs/demo-guide.md. This is the main event (10 minutes). Include:

**Step 3A: Start Implementation** — Provide two options: Option A uses `@modernize-dotnet Start upgrading the projects. Use automatic mode.` (recommended for demo impact). Option B uses Spec Kit `/implement`. Document the behind-the-scenes flow (start_task, update framework, update packages, apply fixes, complete_task, next project).

**Step 3B: Key Changes to Highlight** — Include before/after code blocks for all 5 key migration patterns:

1. TargetFramework: `net6.0` → `net10.0` (all 4 .csproj files)
2. EF Core: `7.0.4` → `10.0.x` (API project — most impactful, spans 3 major versions)
3. API Endpoint Routing: `UseEndpoints(endpoints => { endpoints.MapControllers(); })` → `app.MapControllers()` (API Program.cs)
4. NSwag to built-in OpenAPI: `UseOpenApi()/UseSwaggerUi3()` → `MapOpenApi()` (optional pattern)
5. global.json: SDK `6.0.300` → `10.0.100`

**Step 3C: Build Verification** — Run `dotnet build` and `dotnet test`. Include the success talking point and the "build fails" recovery scenario (this is actually a GREAT demo moment).

Files:
* docs/demo-guide.md - Append Phase 3 section

Discrepancy references:
* DD-01: Research recommends showing NSwag→built-in OpenAPI switch, but this adds risk in a live demo; plan includes it as "optional" pattern
* DR-02: EF Core 7→10 spans 3 major versions with compounding breaking changes; demo should acknowledge this complexity

Success criteria:
* Phase 3 has all three sub-steps (3A, 3B, 3C)
* All 5 before/after code samples included with syntax highlighting
* Both Option A (modernize-dotnet) and Option B (Spec Kit) documented
* Build failure recovery guidance included

Context references:
* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 224-300) - Phase 3 details
* .copilot-tracking/research/subagents/2026-04-27/project-current-state-research.md (Lines 45-65) - EF Core version mismatch details

Dependencies:
* Step 1.4 completion

### Step 1.6: Write Phase 4 — Containerization and Cloud Readiness (~5 min) with Dockerfile, Bicep, and CI/CD updates

Add Phase 4 to docs/demo-guide.md covering three infrastructure layers:

**Step 4A: Dockerfile Updates** — Before/after showing aspnet:6.0→10.0, sdk:6.0→10.0, and EXPOSE 80→8080. Highlight the port change as a common deployment failure.

**Step 4B: Bicep Infrastructure** — Include the Copilot prompt to update all 5 occurrences of `netFrameworkVersion: 'v6.0'` to `'v10.0'` in infra/resources.bicep. Show the diff.

**Step 4C: CI/CD Pipeline** — Include the prompt to update `DOTNET_VERSION: '6.0.x'` to `'10.0.x'` in .github/workflows/resilience-pipeline.yml.

Talking point: "Three layers of infrastructure updated: containers, cloud resources, and CI/CD. All aligned to .NET 10."

Mark this phase as "nice-to-have" with note that it can be talked through if time is short.

Files:
* docs/demo-guide.md - Append Phase 4 section

Success criteria:
* Phase 4 covers all 3 infrastructure layers
* Before/after diffs for each layer
* Port change (80→8080) is explicitly highlighted
* Phase is marked as nice-to-have

Context references:
* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 302-345) - Phase 4 details
* .copilot-tracking/research/subagents/2026-04-27/infra-and-migration-targets-research.md (Lines 1-50) - Bicep version reference locations

Dependencies:
* Step 1.5 completion

### Step 1.7: Write Phase 5 — Validation and Deployment (~5 min) with build, test, commit, and Azure deployment

Add Phase 5 to docs/demo-guide.md covering:

**Step 5A: Full Build and Test** — `dotnet build --configuration Release` and `dotnet test --configuration Release --verbosity normal`.

**Step 5B: Docker Build Test** — `docker compose build` (if time permits).

**Step 5C: Commit and Push** — Include the full commit message following the project's `<type>(<scope>): <description>` format. Push to `modernize/net6-to-net10` branch.

**Step 5D: Deploy to Azure** — Two options: Option A via GitHub Actions PR trigger. Option B via direct `az webapp deploy`.

Mark this phase as "nice-to-have." Include the closing talking point: "In 30 minutes, we went from an unsupported .NET 6 app to a fully modernized .NET 10 application, ready for production on Azure App Service."

Files:
* docs/demo-guide.md - Append Phase 5 section

Success criteria:
* Phase 5 covers all 4 sub-steps
* Commit message follows project convention from .github/copilot-instructions.md
* Both deployment options documented
* Phase is marked as nice-to-have

Context references:
* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 347-395) - Phase 5 details

Dependencies:
* Step 1.6 completion

### Step 1.8: Write Closing section with talking points, audience takeaways table, and Q&A prompts

Add the closing section to docs/demo-guide.md with:

**5 Closing Talking Points**:
1. Speed — "What would take days was done in minutes with AI assistance"
2. Confidence — "Structured assess → plan → execute → validate approach reduces risk"
3. Completeness — "AI caught the EF Core mismatch, port change, and 16+ version references"
4. Tooling Integration — "Spec Kit provides methodology, modernize-dotnet provides the migration engine"
5. Microsoft Alignment — "Follows Microsoft's recommended modernization patterns"

**Key Audience Takeaways Table** — 6 rows mapping What/Tool/Value for: structured specification, AI-powered assessment, dependency-ordered planning, code migration execution, infrastructure updates, and validation.

**Suggested Q&A Topics** — List 3-5 common audience questions with brief answers.

Files:
* docs/demo-guide.md - Append closing section

Success criteria:
* All 5 talking points included
* Takeaways table has 6 rows
* Q&A section provides conversation starters

Context references:
* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 397-435) - Closing content

Dependencies:
* Step 1.7 completion

## Implementation Phase 2: Validate Demo Environment

<!-- parallelizable: true -->

### Step 2.1: Verify .NET 10 SDK is installed and `dotnet --list-sdks` returns 10.0.x

Run `dotnet --list-sdks` in the terminal and confirm a 10.0.x SDK version is listed. If not installed, guide the user to https://dotnet.microsoft.com/en-us/download/dotnet/10.0.

Files:
* No file changes — terminal verification only

Success criteria:
* `dotnet --list-sdks` output includes a 10.0.x entry
* If missing, clear instructions to install

Context references:
* .copilot-tracking/research/subagents/2026-04-27/infra-and-migration-targets-research.md (Lines 160-185) - .NET 10 version confirmation

Dependencies:
* None (parallelizable)

### Step 2.2: Verify Spec Kit CLI v0.8.1 is operational (`specify --version`)

Run `specify --version` and confirm output is 0.8.1. Verify that `.specify/` directory exists in the workspace root (already initialized from prior work).

Files:
* No file changes — terminal verification only

Success criteria:
* `specify --version` returns 0.8.1
* `.specify/` directory exists with init-options.json

Dependencies:
* None (parallelizable)

### Step 2.3: Verify modernize-dotnet plugin v1.0.1047-preview1 is loaded in VS Code

Check VS Code Extensions sidebar for the modernize-dotnet plugin. Alternatively, verify the plugin directory exists at `c:\Users\gappiah\.vscode\agent-plugins\github.com\dotnet\modernize-dotnet\`. Confirm version 1.0.1047-preview1 in plugin.json.

Files:
* No file changes — VS Code verification only

Success criteria:
* Plugin directory exists
* plugin.json shows version 1.0.1047-preview1

Context references:
* .copilot-tracking/research/subagents/2026-04-27/modernize-dotnet-plugin-research.md (Lines 1-30) - Plugin location

Dependencies:
* None (parallelizable)

### Step 2.4: Verify the solution builds on .NET 6 (`dotnet build src/ContosoUniversity.sln`)

Run `dotnet build src/ContosoUniversity.sln` and confirm a clean build with 0 errors. Run `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj` and confirm all tests pass. This establishes the pre-migration baseline.

Files:
* No file changes — build/test verification only

Success criteria:
* `dotnet build` completes with 0 errors
* `dotnet test` reports all tests passed

Dependencies:
* .NET 6 SDK (6.0.300+) must be installed

## Implementation Phase 3: Update Supporting Project Files

<!-- parallelizable: true -->

### Step 3.1: Update .github/copilot-instructions.md to note the demo guide location and modernization workflow

Add a brief section to .github/copilot-instructions.md referencing the demo guide at docs/demo-guide.md. Keep the addition minimal — a 2-3 line note under an appropriate existing section.

Files:
* .github/copilot-instructions.md - Add demo guide reference (small addition)

Discrepancy references:
* DD-02: Research does not explicitly recommend updating copilot-instructions.md; this is a derived objective for discoverability

Success criteria:
* .github/copilot-instructions.md references docs/demo-guide.md
* Addition is minimal and non-disruptive

Context references:
* .github/copilot-instructions.md - Current content for context on where to add the reference

Dependencies:
* Step 1.1 completion (demo guide file must exist)

### Step 3.2: Update AGENTS.md to reference the demo guide and correct the test framework reference (says xUnit, actually MSTest)

Fix the test framework reference in AGENTS.md that says "xUnit with FluentAssertions" — actual test projects use MSTest 2.2.10. Also add a reference to docs/demo-guide.md in an appropriate location.

Files:
* AGENTS.md - Fix test framework reference, add demo guide reference

Discrepancy references:
* DR-03: Research identified the xUnit/MSTest discrepancy in AGENTS.md; this step corrects it

Success criteria:
* AGENTS.md correctly references MSTest instead of xUnit
* AGENTS.md references docs/demo-guide.md

Context references:
* .copilot-tracking/research/subagents/2026-04-27/project-current-state-research.md (Lines 71-85) - Test project analysis showing MSTest
* .copilot-tracking/research/subagents/2026-04-27/infra-and-migration-targets-research.md (Lines 210-220) - AGENTS.md discrepancy

Dependencies:
* Step 1.1 completion (demo guide file must exist)

## Implementation Phase 4: Validation

<!-- parallelizable: false -->

### Step 4.1: Run full project validation

Execute all validation commands for the project:
* `dotnet build src/ContosoUniversity.sln` - Full solution build
* `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj` - Unit test suite
* VS Code markdown preview for docs/demo-guide.md - Visual rendering check

### Step 4.2: Fix minor validation issues

Iterate on lint errors, build warnings, and test failures. Apply fixes directly when corrections are straightforward and isolated. Focus on:
* Markdown formatting in demo guide
* Any broken file path references
* Code block language specifier completeness

### Step 4.3: Report blocking issues

When validation failures require changes beyond minor fixes:
* Document the issues and affected files
* Provide the user with next steps
* Recommend additional research and planning rather than inline fixes
* Avoid large-scale refactoring within this phase

## Dependencies

* .NET 10 SDK (10.0.x)
* .NET 6 SDK (6.0.300+) for baseline verification
* Spec Kit CLI v0.8.1
* modernize-dotnet VS Code plugin v1.0.1047-preview1
* VS Code with GitHub Copilot
* Git CLI

## Success Criteria

* docs/demo-guide.md exists with all 5 demo phases, talking points, and before/after code samples
* Demo environment verified (SDK, Spec Kit, plugin, baseline build)
* Supporting files updated (copilot-instructions.md, AGENTS.md)
* Solution builds and tests pass on pre-migration baseline
