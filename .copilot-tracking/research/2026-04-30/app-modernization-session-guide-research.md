<!-- markdownlint-disable-file -->
# Task Research: App Modernization Session Guide HTML Page

Create an HTML presentation page for a 60-minute developer session on "GitHub Copilot for Application Modernization: Migrating with Confidence"

## Task Implementation Requests

* Create HTML page following the same Primer CSS template as the existing Playwright guide
* Content focused on app modernization with .NET 6 to .NET 10 migration story
* Visually easy to grasp in 10 minutes of showing
* Frame the story: UI upgrade requires .NET 10 first, which itself is phased

## Scope and Success Criteria

* Scope: Single HTML file, same visual template, tailored content for app modernization session
* Assumptions: 60-minute session split between two presenters; user has first 30 min
* Success Criteria:
  * HTML page renders correctly with Primer CSS
  * Content covers: overview, spec-kit workflow, .NET 6 to 10 migration phases, colleague's modernize-dotnet section
  * Visually scannable in 10 minutes

## Research Executed

### File Analysis

* docs/GHCP_Speckit_Playwright_Guide - Copy.html (full template reference)
* specs/001-net6-to-net10-migration/spec.md (project context for the migration)

### External Research

* Azure App Modernization Guidance: 7-phase lifecycle, 6 Rs framework
* GitHub Copilot Modernize-Dotnet Extension: 6 scenarios, 30+ skills, 3-stage workflow
* Spec-Kit (speckit.org): slash commands, two-phase workflow, 28K+ stars
* .NET 10 breaking changes: containers, cryptography, SDK, serialization

## Selected Approach

Create the HTML page with these sections:
1. Hero + Navigation
2. Session Agenda (60 min, two presenters)
3. App Modernization Overview (Microsoft guidance, 6 Rs)
4. The Migration Story (why .NET 10 before UI, phased approach)
5. Spec-Kit for Phased Modernization (workflow, slash commands)
6. Live Demo: .NET 6 to .NET 10 (what the demo covers)
7. Colleague Section: modernize-dotnet extension (overview + handoff)
8. Next Steps

Design decisions:
- Keep content concise and visual (cards, tables, diagrams)
- Use color-coded agenda steps matching the original template
- Emphasize the phased story with a visual timeline/flow
