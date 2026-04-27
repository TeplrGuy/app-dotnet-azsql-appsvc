<!-- markdownlint-disable-file -->
# Release Changes: .NET 6 to .NET 10 Modernization Demo Guide

**Related Plan**: dotnet-modernization-demo-guide-plan.instructions.md
**Implementation Date**: 2026-04-27

## Summary

Create a 30-minute live demo guide for modernizing the Contoso University .NET 6 application to .NET 10 LTS using Spec Kit and the modernize-dotnet Copilot plugin. Update supporting project files for discoverability.

## Changes

### Added

* docs/demo-guide.md - Complete 30-minute demo guide with 5 phases (Discovery, Planning, Code Migration, Containerization, Deployment), presenter talking points, before/after code samples, time budget, and version reference inventory appendix

### Modified

* .github/copilot-instructions.md - Added "Modernization Demo Guide" section with link to docs/demo-guide.md
* AGENTS.md - Fixed test framework reference from "xUnit with FluentAssertions" to "MSTest"; added docs/demo-guide.md to Key Files table

### Removed

## Additional or Deviating Changes

* .NET 6 unit tests could not be executed during validation
  * .NET 6 runtime is not installed on this machine (only .NET 10.0.7 runtime)
  * Build compilation succeeded via .NET 10 SDK cross-compilation support
  * Non-blocking: demo guide purpose is to migrate TO .NET 10

## Release Summary

Created a comprehensive 30-minute demo guide for modernizing Contoso University from .NET 6 to .NET 10 LTS.

**Files affected**: 3 (1 added, 2 modified)

* Added: `docs/demo-guide.md` — 465+ line demo guide with 5 phases (Discovery and Assessment, Planning, Code Migration, Containerization and Cloud Readiness, Validation and Deployment), presenter talking points, 5 before/after code samples, time budget with priority markers, appendix with 16-location version reference inventory
* Modified: `.github/copilot-instructions.md` — Added demo guide reference section
* Modified: `AGENTS.md` — Fixed test framework reference (xUnit to MSTest), added demo guide to Key Files table

**Dependency/infrastructure changes**: None

**Deployment notes**: No deployment required. Demo guide is documentation only.
