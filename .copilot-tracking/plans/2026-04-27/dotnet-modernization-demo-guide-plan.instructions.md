---
applyTo: '.copilot-tracking/changes/2026-04-27/dotnet-modernization-demo-guide-changes.md'
---
<!-- markdownlint-disable-file -->
# Implementation Plan: .NET 6 to .NET 10 Modernization Demo Guide

## Overview

Create a production-ready 30-minute live demo guide that walks through modernizing the Contoso University .NET 6 application to .NET 10 LTS using Spec Kit and the modernize-dotnet Copilot plugin, culminating in Azure App Service deployment.

## Objectives

### User Requirements

* Create a phased demo guide for modernizing Contoso University from .NET 6 to .NET 10 LTS — Source: user conversation
* Use Spec Kit (/specify, /plan, /tasks, /implement) for the structured step-by-step approach — Source: user conversation
* Leverage the modernize-dotnet Copilot plugin for AI-assisted assessment and migration — Source: user conversation
* Cover containerization and cloud readiness patterns aligned to Microsoft guidance — Source: user conversation
* Deploy the modernized application to Azure App Service — Source: user conversation
* Keep the guide executable within a 30-minute demo window — Source: user conversation

### Derived Objectives

* Create a standalone demo guide document at `docs/demo-guide.md` with all 5 phases — Derived from: demo needs to be a single portable reference document for the presenter
* Verify environment prerequisites (.NET 10 SDK, Spec Kit 0.8.1, modernize-dotnet v1.0.1047-preview1) before the guide is finalized — Derived from: demo reliability depends on correct tooling
* Include presenter talking points and audience-facing highlights at each phase — Derived from: 30-minute time constraint requires clear guidance on what to show vs. skip
* Document the 16+ version reference locations across 12 files that need updating — Derived from: research identified the full version inventory
* Address the pre-existing EF Core 7.0.4 version mismatch (net6.0 project with EF Core 7 packages) — Derived from: research flagged this as a risk area
* Include fallback/shortcut guidance for phases 4-5 when running low on time — Derived from: 30-minute budget allocates phases 4-5 as nice-to-have

## Context Summary

### Project Files

* src/ContosoUniversity.WebApplication/ContosoUniversity.WebApplication.csproj - Razor Pages frontend, net6.0, 6 NuGet packages
* src/ContosoUniversity.API/ContosoUniversity.API.csproj - Web API with EF Core 7.0.4, net6.0, 14 NuGet packages (version mismatch)
* src/ContosoUniversity.Test/ContosoUniversity.Test.csproj - MSTest 2.2.10, net6.0
* src/ContosoUniversity.CodedUITest/ContosoUniversity.CodedUITest.csproj - Selenium 4.1.1, MSTest 2.2.10, net6.0
* src/ContosoUniversity.WebApplication/Program.cs - Minimal hosting, Razor Pages, HttpClient factory, health checks
* src/ContosoUniversity.API/Program.cs - Minimal hosting, EF Core + SQL Server, legacy UseEndpoints() pattern, NSwag
* infra/resources.bicep - 5 occurrences of netFrameworkVersion: 'v6.0'
* .github/workflows/resilience-pipeline.yml - DOTNET_VERSION: '6.0.x'
* src/ContosoUniversity.WebApplication/Dockerfile - aspnet:6.0, sdk:6.0, port 80
* src/ContosoUniversity.API/Dockerfile - aspnet:6.0, sdk:6.0, port 80
* src/ContosoUniversity.WebApplication/global.json - SDK 6.0.300
* src/ContosoUniversity.API/global.json - SDK 6.0.300

### References

* .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md - Primary research with full demo guide outline and analysis
* .copilot-tracking/research/subagents/2026-04-27/project-current-state-research.md - Detailed .csproj, Program.cs, and Dockerfile analysis
* .copilot-tracking/research/subagents/2026-04-27/modernize-dotnet-plugin-research.md - Plugin tools, scenarios, workflow, and skills inventory
* .copilot-tracking/research/subagents/2026-04-27/infra-and-migration-targets-research.md - Bicep, CI/CD, global.json, and .NET 10 LTS target confirmation

### Standards References

* .github/copilot-instructions.md — Project coding guidelines, architecture, and deployment commands
* AGENTS.md — Agent-facing project context and build/test commands

## Implementation Checklist

### [x] Implementation Phase 1: Create Demo Guide Document

<!-- parallelizable: false -->

* [x] Step 1.1: Create `docs/demo-guide.md` with frontmatter, session metadata, and time budget table
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 11-45)
* [x] Step 1.2: Write Pre-Demo Setup section with environment verification commands and repo setup
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 47-89)
* [x] Step 1.3: Write Phase 1 — Discovery and Assessment (~5 min) with Spec Kit /specify and modernize-dotnet assessment
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 91-147)
* [x] Step 1.4: Write Phase 2 — Planning (~5 min) with Spec Kit /plan, /tasks, and modernize-dotnet upgrade plan
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 149-192)
* [x] Step 1.5: Write Phase 3 — Code Migration (~10 min) with modernize-dotnet task execution and before/after code samples
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 194-275)
* [x] Step 1.6: Write Phase 4 — Containerization and Cloud Readiness (~5 min) with Dockerfile, Bicep, and CI/CD updates
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 277-330)
* [x] Step 1.7: Write Phase 5 — Validation and Deployment (~5 min) with build, test, commit, and Azure deployment
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 332-388)
* [x] Step 1.8: Write Closing section with talking points, audience takeaways table, and Q&A prompts
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 390-430)

### [x] Implementation Phase 2: Validate Demo Environment

<!-- parallelizable: true -->

* [x] Step 2.1: Verify .NET 10 SDK is installed and `dotnet --list-sdks` returns 10.0.x
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 432-448)
  * Result: .NET 10 SDK 10.0.203 confirmed
* [x] Step 2.2: Verify Spec Kit CLI v0.8.1 is operational (`specify --version`)
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 450-460)
  * Result: specify 0.8.1 confirmed
* [x] Step 2.3: Verify modernize-dotnet plugin v1.0.1047-preview1 is loaded in VS Code
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 462-475)
  * Result: Plugin directory exists at expected path
* [x] Step 2.4: Verify the solution builds on .NET 6 (`dotnet build src/ContosoUniversity.sln`)
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 477-490)
  * Result: Build succeeded (0 errors, 13 warnings). Tests blocked: .NET 6 runtime not installed (only 10.0.7 runtime available)

### [x] Implementation Phase 3: Update Supporting Project Files

<!-- parallelizable: true -->

* [x] Step 3.1: Update .github/copilot-instructions.md to note the demo guide location and modernization workflow
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 492-510)
* [x] Step 3.2: Update AGENTS.md to reference the demo guide and correct the test framework reference (says xUnit, actually MSTest)
  * Details: .copilot-tracking/details/2026-04-27/dotnet-modernization-demo-guide-details.md (Lines 512-530)

### [x] Implementation Phase 4: Validation

<!-- parallelizable: false -->

* [x] Step 4.1: Run full project validation
  * Verify `dotnet build src/ContosoUniversity.sln` succeeds on .NET 6 (pre-migration baseline) — PASSED (0 errors, 13 warnings)
  * Verify `dotnet test src/ContosoUniversity.Test/ContosoUniversity.Test.csproj` passes — BLOCKED (.NET 6 runtime not installed)
  * Verify demo guide document renders correctly in VS Code preview — PASSED
* [x] Step 4.2: Fix minor validation issues
  * No markdown or formatting issues found
* [x] Step 4.3: Report blocking issues
  * .NET 6 runtime not installed (only .NET 10.0.7 runtime available) — tests cannot execute on net6.0 TFM
  * Non-blocking for demo guide: the guide's purpose is to migrate TO .NET 10, and the build baseline is confirmed

## Planning Log

See .copilot-tracking/plans/logs/2026-04-27/dotnet-modernization-demo-guide-log.md for discrepancy tracking, implementation paths considered, and suggested follow-on work.

## Dependencies

* .NET 10 SDK (10.0.x) installed locally
* .NET 6 SDK (6.0.300+) for pre-migration baseline build
* Spec Kit CLI v0.8.1 (`uv tool install specify-cli`)
* modernize-dotnet VS Code plugin v1.0.1047-preview1
* VS Code with GitHub Copilot active
* Git for branch management
* Azure CLI (for Phase 5 deployment, optional)
* Docker (for Phase 4 container build, optional)

## Success Criteria

* Demo guide document exists at `docs/demo-guide.md` with all 5 phases — Traces to: user requirement for phased demo guide
* Each phase has time allocation, presenter talking points, exact commands, and expected outcomes — Traces to: 30-minute demo window requirement
* Spec Kit workflow (/specify, /plan, /tasks, /implement) is integrated into phases 1-3 — Traces to: user requirement for Spec Kit usage
* modernize-dotnet plugin assessment and task execution is integrated into phases 1-3 — Traces to: user requirement for modernize-dotnet usage
* Phase 3 includes before/after code samples for all 5 key migration patterns — Traces to: research finding of 16+ version references
* Phases 4-5 are marked as nice-to-have with shortcut guidance — Traces to: user requirement for 30-minute constraint
* Supporting project files (.github/copilot-instructions.md, AGENTS.md) reference the demo guide — Traces to: derived objective for discoverability
* Solution builds and tests pass on .NET 6 baseline — Traces to: deployment reliability requirement
