<!-- markdownlint-disable-file -->
# Planning Log: .NET 6 to .NET 10 Modernization Demo Guide

## Discrepancy Log

Gaps and differences identified between research findings and the implementation plan.

### Unaddressed Research Items

* DR-01: Spec Kit /specify command output variability
  * Source: .copilot-tracking/research/subagents/2026-04-27/modernize-dotnet-plugin-research.md (Lines 100-120)
  * Reason: Spec Kit artifact format depends on version and AI provider configuration; cannot fully predict output in a demo guide
  * Impact: low — presenter can adapt to actual output during live demo

* DR-02: EF Core 7→10 compounding breaking changes across 3 major versions
  * Source: .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 65-68)
  * Reason: Full breaking changes list is extensive; demo guide includes the pattern but defers detailed troubleshooting to Microsoft docs
  * Impact: medium — if EF Core migration fails during demo, presenter needs recovery knowledge

* DR-03: AGENTS.md test framework discrepancy (says xUnit, actually MSTest)
  * Source: .copilot-tracking/research/subagents/2026-04-27/infra-and-migration-targets-research.md (Lines 210-220)
  * Reason: Addressed in Implementation Phase 3, Step 3.2
  * Impact: low — corrected during implementation

* DR-04: Detailed .NET 10 breaking changes list not compiled
  * Source: .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 440-445) - Potential Next Research
  * Reason: Full breaking changes enumeration is out of scope for demo guide creation; modernize-dotnet plugin handles this during execution
  * Impact: low — plugin has built-in knowledge of breaking changes

* DR-05: Azure App Service .NET 10 runtime availability by region not verified
  * Source: .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 447-449) - Potential Next Research
  * Reason: Region availability is deployment-time concern; demo guide includes this as a pre-check note
  * Impact: medium — could block Phase 5 deployment if region doesn't support .NET 10

* DR-06: NSwag 14 vs. built-in OpenAPI comparison not completed
  * Source: .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 451-453) - Potential Next Research
  * Reason: Demo guide marks the NSwag→OpenAPI switch as optional; both paths are valid
  * Impact: low — presenter can choose either approach

* DR-07: CodedUITest project deprecation considerations
  * Source: .copilot-tracking/research/subagents/2026-04-27/project-current-state-research.md (Lines 80-90)
  * Reason: Coded UI Tests are deprecated by Microsoft; upgrading the framework is sufficient for demo purposes without addressing the deprecation
  * Impact: low — project still compiles after framework upgrade

* DR-08: Subagent research references .NET 9 package target versions instead of .NET 10
  * Source: .copilot-tracking/research/subagents/2026-04-27/project-current-state-research.md (NuGet migration notes throughout Sections 1-2)
  * Reason: The project-current-state subagent research was conducted with .NET 9 as the assumed target; the primary research and infra research correctly identify .NET 10 LTS as the target. The plan and details correctly use .NET 10 throughout. Implementers should follow plan/details package version guidance (10.0.x), not the subagent research migration notes (9.0.x).
  * Impact: low — plan and details override subagent research; only a risk if implementer reads subagent research in isolation

### Plan Deviations from Research

* DD-01: NSwag→built-in OpenAPI switch marked optional instead of recommended
  * Research recommends: Switching to .NET built-in OpenAPI (`MapOpenApi()`) as the modern pattern
  * Plan implements: Lists as "optional pattern" in Phase 3 Step 3B
  * Rationale: Live demo risk — NSwag→OpenAPI switch involves removing packages and changing middleware, which could introduce errors in a 10-minute window. Safer to show as a talking point.

* DD-02: Supporting file updates added (not in original research scope)
  * Research recommends: No mention of updating copilot-instructions.md or AGENTS.md
  * Plan implements: Phase 3 updates both files for demo guide discoverability
  * Rationale: These files are the primary context for Copilot agents; adding the demo guide reference improves future discoverability and fixes the known xUnit/MSTest error.

## Implementation Paths Considered

### Selected: Standalone Demo Guide with Dual-Tool Workflow

* Approach: Create a single `docs/demo-guide.md` that integrates both Spec Kit (/specify, /plan, /tasks, /implement) and modernize-dotnet plugin in a complementary workflow. Spec Kit handles the structured methodology; modernize-dotnet handles the actual code migration.
* Rationale: Showcases both tools without competition. Spec Kit demonstrates specification-driven development while modernize-dotnet demonstrates Microsoft's AI-powered migration engine. The 30-minute window is achievable with this split (5+5+10+5+5).
* Evidence: .copilot-tracking/research/2026-04-27/dotnet-modernization-demo-guide-research.md (Lines 70-80)

### IP-01: modernize-dotnet Only (Skip Spec Kit)

* Approach: Run the entire migration through the modernize-dotnet agent alone using its 3-phase workflow (Assessment, Planning, Execution)
* Trade-offs: Simpler to execute (one tool); misses the Spec Kit demo opportunity; doesn't showcase specification-driven development methodology
* Rejection rationale: User explicitly requested Spec Kit integration; the session title emphasizes "structured approach" which Spec Kit delivers

### IP-02: Spec Kit Only (Skip modernize-dotnet)

* Approach: Drive everything through /specify → /plan → /implement using general Copilot for code changes
* Trade-offs: Shows the full Spec Kit workflow; misses the specialized migration capabilities; general Copilot may not handle EF Core 7→10 breaking changes as reliably
* Rejection rationale: modernize-dotnet has 30+ built-in skills specifically for .NET migration scenarios; general Copilot would be less reliable for the complex package upgrades

### IP-03: Incremental Migration (.NET 6 → 8 → 10)

* Approach: Step through each major version individually to reduce risk
* Trade-offs: Lower risk per step; takes 3x longer; no business value in intermediate versions
* Rejection rationale: 30-minute constraint makes this impossible; modernize-dotnet supports direct jumps; .NET 8 and 9 have shorter remaining support windows

### IP-04: Target .NET 9 Instead of .NET 10

* Approach: Use .NET 9 (STS) as the migration target
* Trade-offs: Slightly smaller version jump; reaches end-of-support Nov 2026 (7 months away)
* Rejection rationale: .NET 10 LTS provides support until Nov 2028; no benefit to targeting a shorter-lived runtime

### Implementation Deviations

* DD-03: .NET 6 unit tests could not be executed during validation
  * Plan specifies: Run `dotnet test` to verify all tests pass on .NET 6 baseline
  * Implementation differs: Tests failed because .NET 6 runtime is not installed (only 10.0.7 runtime available)
  * Rationale: Build compilation succeeded via .NET 10 SDK cross-compilation. Non-blocking since the demo guide migrates TO .NET 10 and tests will run after migration.

## Suggested Follow-On Work

Items identified during planning that fall outside current scope.

* WI-01: Compile detailed .NET 10 breaking changes cheat sheet — Quick reference for presenter recovery during live demo when errors occur (medium priority)
  * Source: DR-04 unaddressed research item
  * Dependency: None

* WI-02: Install .NET 6 runtime for pre-migration baseline test execution — Required only if pre-migration test verification is needed (low priority)
  * Source: DD-03 implementation deviation
  * Dependency: Download from https://dotnet.microsoft.com/en-us/download/dotnet/6.0

* WI-02: Verify Azure App Service .NET 10 runtime in target Azure region — Ensure Phase 5 deployment will work (high priority)
  * Source: DR-05 unaddressed research item
  * Dependency: Azure subscription access

* WI-03: Create a "demo recovery" document — Step-by-step troubleshooting for common demo failures (EF Core errors, port mismatches, NuGet restore failures) (medium priority)
  * Source: Phase 3 build failure recovery scenario
  * Dependency: WI-01 completion

* WI-04: Record a dry-run video — Practice the full 30-minute demo to calibrate timing (low priority)
  * Source: Time budget constraints
  * Dependency: All implementation phases complete

* WI-05: Address CodedUITest deprecation — Evaluate converting Selenium-based Coded UI Tests to a modern test framework or removing the project (low priority)
  * Source: DR-07 CodedUITest deprecation
  * Dependency: Post-modernization, separate planning cycle

* WI-06: Migrate Newtonsoft.Json to System.Text.Json — Both WebApp and API reference Newtonsoft.Json; modernize to built-in System.Text.Json (low priority)
  * Source: .copilot-tracking/research/subagents/2026-04-27/project-current-state-research.md NuGet analysis
  * Dependency: Post-.NET 10 migration, separate modernize-dotnet scenario
