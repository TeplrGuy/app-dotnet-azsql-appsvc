# App Modernization Research

## Research Topics

1. Azure app modernization guidance overview
2. GitHub Copilot app modernization extension for .NET
3. .NET modernize platform page
4. spec-kit official page
5. Key phases/steps of .NET application modernization
6. GitHub Copilot modernize-dotnet extension capabilities
7. Key features of spec-kit for structured development
8. Breaking changes between .NET 6 and .NET 10

---

## 1. Azure App Modernization Guidance

**Source:** https://learn.microsoft.com/en-us/azure/app-modernization-guidance/

### Key Concepts

Microsoft App Modernization Guidance for Azure provides proven guidance and best practices for confidently modernizing applications and data. It treats application modernization as a **continuous cycle** of assessment, planning, execution, and maintenance.

### Structure and Phases

The guidance is organized into 7 major phases:

| Phase | Purpose |
|-------|---------|
| **Get Started** | Overview, life cycle understanding, maturity model, roadmap |
| **Assess** | Evaluate IT assets, identify legacy systems, security vulnerabilities, optimization areas |
| **Plan** | Evaluate modernization strategies with ROI focus, prioritize financial objectives |
| **Launch** | Execute proof of concept (PoC), validate strategy, Center of Excellence |
| **Foundation** | End-to-end modernization with cloud-first mindset, PaaS adoption |
| **Expand** | Growth and optimization using managed application services, API-first approach |
| **Innovate** | AI and advanced technologies, new business models and revenue streams |
| **Optimize** | Continuous optimization, lower costs, improve performance/security/compliance |

### The 6 Rs of Application Modernization

Core decision framework for choosing modernization approach:

1. **Rehost** (Lift and Shift) - Move to cloud without modifying code
2. **Replatform** (Lift, Tinker, and Shift) - Move to new runtime with minimal code changes
3. **Refactor** - Change existing code without major external behavior changes
4. **Rebuild** - Start over when replatforming/refactoring costs outweigh benefits
5. **Retire** - Decommission or shut down applications
6. **Retain** - Keep as-is when not ready to modernize

### Architecture Concepts

- Modernization is a continuum building cloud-native readiness
- Each strategy suits the organization's level of technological and operational maturity
- Foundation targets include: VMware/Azure VMware Solution, Oracle workloads, SAP applications, .NET applications, Java applications
- Expansion includes: microservices, serverless platforms, API wrappers, data replatforming
- Innovation includes: AI services, best practices for automation

### Value Propositions

- Shift CapEx to OpEx (pay for what you use)
- Improve scalability, agility, security, compliance
- Faster time-to-market
- Better customer experiences
- Higher revenue and competitive edge

---

## 2. GitHub Copilot App Modernization Extension for .NET

**Source:** https://learn.microsoft.com/en-us/dotnet/core/porting/github-copilot-app-modernization/overview

### Definition

GitHub Copilot modernization is a **GitHub Copilot agent** that helps upgrade projects to newer .NET versions and migrate .NET applications to Azure quickly and confidently. It guides through assessment, solution recommendations, code fixes, and validation.

### Key Capabilities

- Upgrade to newer versions of .NET
- Migrate technologies and deploy to Azure
- Modernize .NET apps (especially from .NET Framework)
- Assess application code, configuration, and dependencies
- Plan and set up Azure resources
- Fix issues and apply best practices for cloud migration
- Validate builds and tests

### Supported Scenarios (6 End-to-End Workflows)

| Scenario | Description |
|----------|-------------|
| .NET version upgrade | Upgrades from older .NET versions to .NET 8, 9, 10, or later |
| SDK-style conversion | Converts legacy project format to SDK-style |
| Newtonsoft.Json upgrade | Replaces Newtonsoft.Json with System.Text.Json |
| SqlClient upgrade | Upgrades System.Data.SqlClient to Microsoft.Data.SqlClient |
| Azure Functions upgrade | Upgrades from in-process to isolated worker model |
| Semantic Kernel to Agents | Upgrades SK Agents to Microsoft Agent Framework |

### Supported Upgrade Paths

| From | To |
|------|-----|
| .NET Framework (any version) | .NET 8 or later |
| .NET Core 1.x-3.x | .NET 8 or later |
| .NET 5 or later | .NET 8 or later |

### Supported Project Types

- ASP.NET Core (MVC, Razor Pages, Web API)
- Blazor
- Azure Functions
- WPF, Windows Forms, WinUI
- .NET MAUI and Xamarin
- Class libraries
- Console apps
- Test projects (MSTest, NUnit, xUnit)

### Three-Stage Workflow

1. **Assessment** - Examines project structure, dependencies, code patterns. Presents strategy decisions (upgrade strategy, project approach, technology options). Saves to `assessment.md` and `upgrade-options.md`
2. **Planning** - Converts assessment into detailed specification. Documents upgrade strategies, refactoring approaches, dependency paths, risk mitigations. Saves to `plan.md`
3. **Execution** - Breaks plan into sequential tasks with validation criteria in `tasks.md`. Each task describes a single change and how success is confirmed.

### Upgrade Strategies

| Strategy | Best For | Approach |
|----------|----------|----------|
| Bottom-up | Large solutions with deep dependency graphs | Upgrades leaf projects first, then works upward |
| Top-down | Quick feedback on main application | Upgrades application project first, then fixes dependencies |
| All-at-once | Small, simple solutions | Upgrades all projects in one pass |

### Flow Modes

- **Automatic** - Works through all stages without pausing; best for experienced users
- **Guided** - Pauses at each stage boundary for review; best for first-time users

### State Management

All upgrade state stored in `.github/upgrades/{scenarioId}/`:
- `assessment.md` - Analysis of solution
- `upgrade-options.md` - Confirmed upgrade decisions
- `plan.md` - Ordered task plan
- `tasks.md` - Live progress dashboard
- `scenario-instructions.md` - Agent's persistent memory
- `execution-log.md` - Detailed audit trail
- `tasks/{taskId}/task.md` - Per-task scope
- `tasks/{taskId}/progress-details.md` - Per-task execution notes

### Azure Migration Features

Predefined tasks for migration:
- Migrate to Managed Identity-based Database (Azure SQL DB, Azure SQL MI, Azure PostgreSQL)
- Migrate to Azure File Storage
- Migrate to Azure Blob Storage
- Migrate to Microsoft Entra ID
- Migrate to secured credentials (Managed Identity + Azure Key Vault)
- Migrate to Azure Service Bus
- Migrate to Azure Communication Service email
- Migrate to Confluent Cloud/Azure Event Hub for Kafka
- Migrate to OpenTelemetry on Azure
- Migrate to Azure Cache for Redis using Managed Identity

### Available Environments

- Visual Studio: Right-click solution > Modernize, or `@Modernize` in chat
- Visual Studio Code: `@modernize-dotnet` in chat
- GitHub Copilot CLI: `@modernize-dotnet` followed by request
- GitHub.com: `modernize-dotnet` coding agent

### Learning Agent Feature

The agent learns from user edits and applies patterns to similar upgrade cases. Creates Git commits for every change enabling rollback.

---

## 3. .NET Modernize Platform Page

**Source:** https://dotnet.microsoft.com/en-us/platform/modernize

### Headline

"Modernize .NET apps effortlessly with GitHub Copilot" - Let agent mode automate upgrades, reduce risk, and get to modern .NET faster and easier.

### Key Value Propositions (7 Pillars)

1. **Intelligent Assessment** - Analyze application to identify upgrade scope, breaking changes, compatibility gaps, risk areas
2. **AI Plan Generation** - Generate upgrade plans via natural language chat using AI with GitHub Copilot agent mode
3. **Code Transformations** - Apply automatic code changes and validate results
4. **Personalized Workflow** - Decide which projects to upgrade, address security vulnerabilities, etc.
5. **Git Commit Tracking** - Naturally tracks commits for incremental adoption and testing
6. **Learning Agent** - Automates fixes as agent mode learns from edits and applies patterns
7. **Accelerate Modernization** - More personalized and reliable experience; faster path to modern .NET or cloud

### Supported Environments

- Visual Studio 2026
- Visual Studio Code (GitHub Copilot modernization extension)
- Windows OS
- C# language only
- Git repository required
- Copilot plan: Free, Pro, Pro+, Business, or Enterprise

### Supported Project Types

ASP.NET Core, Windows Forms, WPF, WinForms, Libraries, Console

### Upgrade Paths

.NET Framework and .NET Core to the latest supported .NET

### Customer Testimonials

- **Exact** (Principal Software Architect): "The tool handled complex project and package changes with ease, giving me confidence that we can move to .NET 9 much faster."
- **FMG** (Staff Platform Engineer): "Once installed, it just worked - upgrading more than a dozen applications seamlessly in just a matter of hours."
- **Microsoft Teams**: "Instead of spending week(s) upgrading and testing several projects from .NET 6 to .NET 8, we got it done within a few hours with GitHub Copilot."
- **Xbox** (Presence Service): "GitHub Copilot upgrades for .NET turned a manual process into something almost-automatic."

---

## 4. Spec Kit Official Page

**Source:** https://speckit.org/

### Definition

Spec Kit is a tool that revolutionizes software development by making specifications executable. It introduces "Spec-Driven Development" - a methodology where specifications become executable, directly generating working implementations.

### Core Philosophy

- Focus on **what** you want to build and **why**, not technical implementation details
- AI-powered specification execution
- Eliminate gap between specification and implementation
- Reduce development time

### Key Statistics

- 28K+ GitHub Stars
- 11+ AI Agents supported
- 2.3K+ Forks
- MIT Open Source License

### Installation

```bash
# Install with uv (recommended)
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git

# Initialize with specific AI agent
specify init my-project --ai copilot
specify init my-project --ai claude
specify init my-project --ai cursor
```

### Slash Commands (Structured Development)

| Command | Purpose | Notes |
|---------|---------|-------|
| `/constitution` | Create project governing principles and guidelines | Run first |
| `/specify` | Define what to build (requirements and user stories) | Focus on what and why |
| `/clarify` | Clarify underspecified areas through structured questioning | Must run before /plan |
| `/plan` | Create technical implementation plans with tech stack | Architecture and frameworks |
| `/tasks` | Generate actionable task lists for implementation | Breaks plan into steps |
| `/analyze` | Cross-artifact consistency and coverage analysis | Run after /tasks, before /implement |
| `/implement` | Execute all tasks to build the feature | Generates working code |

### Development Workflow

**Phase 1: Foundation**
1. Constitution - Establish project principles
2. Specification - Define requirements clearly
3. Clarification - Resolve ambiguities

**Phase 2: Implementation**
1. Planning - Choose tech stack and architecture
2. Tasks - Break down into actionable items
3. Implementation - Generate working code

### Supported AI Agents

- **Fully Supported:** Claude Code, GitHub Copilot, Cursor, Gemini CLI, Windsurf
- **Limited Support:** Codex CLI (no custom arguments for slash commands)
- **Plus 5 more:** Qwen Code, opencode, Kilo Code, Auggie CLI, Roo Code

### System Requirements

- **Required:** Python 3.8+, Git 2.20+, Internet connection
- **Recommended:** VS Code or Cursor, AI Agent subscription, uv package manager
- **Optional:** Docker, GitHub CLI, Node.js (for web projects)

### Maintainers

- Den Delimarsky (@localden) - GitHub
- John Lam (@jflam) - GitHub

---

## 5. Key Phases of .NET Application Modernization (Microsoft)

### Application Modernization Life Cycle

Microsoft defines app modernization as a continuous cycle:

1. **Assess** - Evaluate IT assets, identify legacy systems, security vulnerabilities, technical debt, optimization areas
2. **Plan** - Evaluate modernization strategies with ROI focus, use 6 Rs framework, prioritize applications
3. **Launch** - Build proof of concept (PoC), validate strategy, establish feedback loops
4. **Foundation** - End-to-end modernization with cloud-first PaaS mindset, CI/CD pipelines
5. **Expand** - Use managed services, API-first approach, enhance performance and reliability
6. **Innovate** - Adopt AI and advanced technologies, new business models
7. **Optimize** - Continuous improvement, right-sizing, DevOps/DevSecOps practices

### Maturity Model Stages

- Traditional Foundation
- Emerging Pioneers
- Agile Innovators
- Digital Champions

---

## 6. GitHub Copilot Modernize-Dotnet Extension - Detailed Capabilities

### Scenarios and Skills Reference

The extension has **6 scenarios** (top-level workflows) and **30+ built-in upgrade skills** organized into categories:

#### Common Skills
- Converting to SDK-style format
- Upgrading Autofac to .NET DI / Integrating Autofac with .NET
- Upgrading cryptography namespaces
- Upgrading Newtonsoft to System.Text.Json
- Upgrading Semantic Kernel to Agents
- Upgrading to MSMQ.Messaging
- Converting to Central Package Management
- Modernizing C# version (C# 7.0 through 15)
- Upgrading C# nullable references

#### Data Access Skills
- Upgrading EDMX to Code-First
- Upgrading EF DbContext
- Upgrading EF6 Code-First to EF Core
- Upgrading to Microsoft.Data.SqlClient

#### Web and ASP.NET Skills
- Upgrading ASP.NET Framework to Core (comprehensive)
- Upgrading ASP.NET Identity
- Upgrading Global.asax
- Upgrading OWIN to middleware / Cookie Auth / OAuth to JWT / OpenID Connect
- Upgrading MVC (authentication, bundling, configuration, content negotiation, controllers, DI, filters, HTTP pipeline, HttpContext, logging, model binding, Razor views, routing, session state, static files, System.Web adapters, validation)
- Upgrading WCF to CoreWCF

#### Cloud and Azure Skills
- Upgrading Azure Functions Startup / to v2
- Upgrading Azure Key Vault / Service Bus / Storage SDKs

#### Library Skills
- Upgrading ADAL to MSAL
- Upgrading ASP.NET SignalR
- Upgrading Bond interfaces
- Upgrading Data EDM/OData/Data Services
- Upgrading PowerShell SDK
- Upgrading SPA Services to SPA Proxy
- Upgrading System.Spatial
- Upgrading WebAPI CORS / OData

### Skill Activation

- **Session start** - Loads matching scenario and immediately relevant skills
- **During execution** - Loads specialized skills on demand (e.g., EDMX, WCF, OWIN)
- **On request** - User can ask for any skill at any time

### Custom Skills

Users can create custom skills in:
- Repository: `.github/skills/`
- User profile: `%UserProfile%/.copilot/skills/`

---

## 7. Key Features of Spec Kit for Structured Development

### Spec-Driven Development Methodology

1. **Specifications First** - Specifications are no longer scaffolding discarded after coding; they become executable artifacts
2. **AI-Powered Execution** - AI agents transform specs into working code
3. **Structured Workflow** - Constitution > Specify > Clarify > Plan > Tasks > Analyze > Implement
4. **Cross-Artifact Analysis** - `/analyze` command validates consistency and coverage across all artifacts
5. **Multi-Agent Support** - Works with 11+ AI coding assistants
6. **CLI-Based Initialization** - `specify init` bootstraps projects with agent-specific configurations
7. **Slash Command System** - Structured commands that guide the development process
8. **Phase-Based Development** - Clear separation between foundation (what to build) and implementation (how to build)

### Benefits for Structured Development

- Eliminates gap between specification and implementation
- Reduces miscommunication and technical debt
- Focus on product scenarios rather than undifferentiated code
- Faster delivery through AI-powered specification execution
- Reproducible workflows across different AI agents

---

## 8. Breaking Changes Between .NET 6 and .NET 10

### Overview

When migrating from .NET 6 to .NET 10, you encounter breaking changes introduced across .NET 7, 8, 9, and 10. Changes are categorized as:
- **Binary incompatible** - Existing binaries may fail to load or execute
- **Source incompatible** - Source code requires changes to compile
- **Behavioral change** - Code behaves differently at runtime

### .NET 10 Breaking Changes by Area

#### Containers
- Default .NET images use Ubuntu (behavioral change)

#### Core .NET Libraries
- API obsoletions (source incompatible)
- ActivitySource behavior changes
- BufferedStream.WriteByte no longer performs implicit flush
- C# 14 overload resolution with span parameters
- Consistent shift behavior in generic math
- Default trace context propagator updated to W3C standard
- .NET runtime no longer provides default termination signal handlers
- System.Linq.AsyncEnumerable included in core libraries (source incompatible)

#### Cryptography
- OpenSSL 1.1.1 or later required on Unix
- OpenSSL primitives not supported on macOS
- X500DistinguishedName validation is stricter
- Post-quantum cryptography updates (CompositeMLDsa, MLDsa, SlhDsa)
- Environment variable renamed to DOTNET_OPENSSL_VERSION_OVERRIDE

#### Extensions
- BackgroundService runs all of ExecuteAsync as a Task
- Null values preserved in configuration
- Message no longer duplicated in Console log output
- Fix issues in GetKeyedService() with AnyKey

#### SDK and MSBuild (Major Changes)
- dotnet new sln defaults to SLNX file format
- dotnet restore audits transitive packages
- PackageReference without a version raises an error
- Double quotes in file-level directives disallowed
- .NET tool packaging creates RuntimeIdentifier-specific packages
- Default workload configuration changed to 'workload sets' mode
- project.json not supported in dotnet restore
- NuGet packages with no runtime assets not included in deps.json

#### Networking
- Streaming HTTP responses enabled by default in browser clients
- Uri length limits removed
- MailAddress enforces validation for consecutive dots

#### Serialization
- System.Text.Json checks for property name conflicts
- XmlSerializer no longer ignores ObsoleteAttribute properties

#### Windows Forms
- StatusStrip uses System RenderMode by default
- System.Drawing OutOfMemoryException changed to ExternalException
- Applications referencing WPF and WinForms must disambiguate types

#### WPF
- Empty ColumnDefinitions and RowDefinitions disallowed
- Incorrect DynamicResource usage causes crash

### What's New in .NET 10

- **Runtime:** JIT inlining improvements, method devirtualization, AVX10.2, NativeAOT enhancements
- **Libraries:** Post-quantum cryptography, JSON strict serialization, PipeReader support, WebSocketStream
- **SDK:** Microsoft.Testing.Platform in dotnet test, native container images for console apps, file-based apps with publish support
- **C# 14:** Field-backed properties, extension blocks, null-conditional assignment, partial constructors/events, user-defined compound assignment operators
- **ASP.NET Core 10:** Blazor WebAssembly preloading, OpenAPI enhancements, passkey support
- **EF Core 10:** LINQ enhancements, named query filters, performance optimizations
- **LTS Release:** .NET 10 is supported for 3 years as a Long-Term Support release

---

## References

- https://learn.microsoft.com/en-us/azure/app-modernization-guidance/
- https://learn.microsoft.com/en-us/azure/app-modernization-guidance/get-started/application-modernization-life-cycle
- https://learn.microsoft.com/en-us/azure/app-modernization-guidance/plan/the-6-rs-of-application-modernization
- https://learn.microsoft.com/en-us/dotnet/core/porting/github-copilot-app-modernization/overview
- https://learn.microsoft.com/en-us/dotnet/core/porting/github-copilot-app-modernization/scenarios-and-skills
- https://dotnet.microsoft.com/en-us/platform/modernize
- https://speckit.org/
- https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0
- https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview

---

## Follow-On Questions (Directly Relevant)

- What are the specific ASP.NET Core 10 breaking changes for MVC apps?
- What is the detailed architecture diagram for the GitHub Copilot modernization agent workflow?
- What specific Bicep/ARM templates are recommended for Azure migration targets?
- What are the pricing implications of different Copilot plans for modernization?

## Clarifying Questions

- What level of depth is needed for the HTML presentation (executive summary vs. deep technical)?
- Should the presentation include interactive elements or is it a static informational page?
- Should customer testimonials be included in the presentation?
- Is there a specific visual theme or branding guideline to follow?
