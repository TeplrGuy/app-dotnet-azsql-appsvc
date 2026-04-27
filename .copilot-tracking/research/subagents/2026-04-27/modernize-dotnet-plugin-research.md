# Modernize-Dotnet Plugin Research

## Status: Complete

## Research Topics

1. Local filesystem location of the modernize-dotnet plugin
2. Plugin manifest, skills, and instruction files
3. MCP tools exposed (names, descriptions, parameters)
4. Recommended workflow for .NET upgrades
5. Skills for assessment, project discovery, test running, etc.

---

## 1. Local Filesystem Location

The plugin is installed at:

```
c:\Users\gappiah\.vscode\agent-plugins\github.com\dotnet\modernize-dotnet\
```

Confirmed in `c:\Users\gappiah\.vscode\agent-plugins\installed.json` with entry:

```json
{
  "pluginUri": "file:///c%3A/Users/gappiah/.vscode/agent-plugins/github.com/dotnet/modernize-dotnet/plugins/modernize-dotnet",
  "marketplace": "github/awesome-copilot"
}
```

### Directory Structure

```
modernize-dotnet/                         # repo root
├── .github/
│   ├── plugin/
│   │   └── marketplace.json              # marketplace metadata
│   ├── ISSUE_TEMPLATE/
│   └── workflows/
├── coding-agent/
│   ├── modernize-dotnet.agent.md         # CCA variant (non-interactive)
│   ├── README.md
│   ├── linux/                            # copilot-setup-steps.yml for Linux
│   └── windows/                          # copilot-setup-steps.yml for Windows
├── plugins/
│   └── modernize-dotnet/                 # the actual plugin
│       ├── .github/
│       │   └── plugin/
│       │       └── plugin.json           # plugin manifest
│       ├── agents/
│       │   └── modernize-dotnet.agent.md # main agent definition
│       └── README.md
├── CHANGELOG.md
├── VS-CHANGELOG.md
├── CODE-OF-CONDUCT.md
├── LICENSE.md
└── README.md
```

---

## 2. Plugin Manifest

File: `plugins/modernize-dotnet/.github/plugin/plugin.json`

```json
{
  "name": "modernize-dotnet",
  "version": "1.0.1047-preview1",
  "description": "AI-powered .NET modernization and upgrade assistant. Helps upgrade .NET Framework and .NET applications to the latest versions of .NET.",
  "author": { "name": "Microsoft" },
  "keywords": ["modernization", "upgrade", "migration", "dotnet"],
  "repository": "https://github.com/dotnet/modernize-dotnet",
  "homepage": "https://dotnet.microsoft.com/en-us/platform/modernize",
  "license": "MIT"
}
```

---

## 3. MCP Server Configuration

Defined in the agent.md frontmatter:

```yaml
mcp-servers:
  Modernization:
    type: 'local'
    command: 'dnx'
    args: [
      'Microsoft.GitHubCopilot.Modernization.Mcp',
      '--prerelease',
      '--yes',
      '--ignore-failed-sources'
    ]
    cwd: '~'
    tools: ['*']
    env:
      APPMOD_CALLER_TYPE: copilot-cli
```

The MCP server is a .NET tool (`Microsoft.GitHubCopilot.Modernization.Mcp`) launched via `dnx`. It starts automatically when the modernize-dotnet agent is invoked.

### MCP Tools Exposed

The agent.md documents these core MCP tools:

#### Workflow Management Tools

| Tool | Description |
|------|-------------|
| `get_state` | Get current workflow state — active scenario, task progress, stale warnings, existing scenarios on disk |
| `initialize_scenario` | Initialize a new scenario workflow (creates `.github/upgrades/{scenarioId}/` folder structure) |
| `resume_scenario` | Resume an existing scenario from a previous session |
| `start_task` | Start a task — returns task content, related skills, stale task warnings |
| `complete_task` | Mark a task as complete (or failed with `failed=true`) |
| `break_down_task` | Register subtasks for a parent task (declarative reconciliation) |

#### Scenario and Instructions Tools

| Tool | Description |
|------|-------------|
| `get_scenarios` | List available modernization scenarios |
| `get_instructions(kind='scenario', query='...')` | Load full scenario instructions (MANDATORY before any upgrade work) |
| `get_instructions(kind='skill', query='...')` | Load skill-specific guidance |

**Note**: These map to the `mcp_copilotmod_*` prefixed tools available in the current VS Code session:

| Agent Tool Name | VS Code MCP Tool Name |
|---|---|
| `get_state` | `mcp_copilotmod_get_state` |
| `initialize_scenario` | `mcp_copilotmod_initialize_scenario` |
| `resume_scenario` | `mcp_copilotmod_resume_scenario` |
| `start_task` | `mcp_copilotmod_start_task` |
| `complete_task` | `mcp_copilotmod_complete_task` |
| `break_down_task` | `mcp_copilotmod_break_down_task` |
| `get_scenarios` | `mcp_copilotmod_get_scenarios` |
| `get_instructions` | `mcp_copilotmod_get_instructions` |

### Additional VS Code Extension MCP Tools

The VS Code extension (`vscjava.migrate-java-to-azure`) provides additional MCP tools with the `mcp_copilotmod_` prefix:

| Tool | Description |
|------|-------------|
| `get_solution_path` | Get path of currently loaded solution |
| `get_projects_in_topological_order` | Retrieve projects in dependency order |
| `get_project_dependencies` | Get detailed dependency info for a project |
| `get_dotnet_upgrade_options` | Get upgrade target options and parameters |
| `generate_dotnet_upgrade_assessment` | Generate comprehensive assessment for .NET version upgrade |
| `query_dotnet_assessment` | Query assessment data (hierarchical scope-based) |
| `convert_project_to_sdk_style` | Convert legacy project to SDK-style |
| `discover_upgrade_scenarios` | Discover potential upgrade scenarios for a solution/project |
| `discover_test_projects` | Discover test projects in solution |
| `get_code_dependencies` | Build dependency graph for a C# class/controller |
| `get_type_info` | Get detailed info about a .NET type |
| `get_member_info` | Get detailed info about a .NET type member |
| `get_namespace_info` | Get detailed info about a .NET namespace |
| `get_supported_package_version` | Get latest supported package version for a target framework |
| `validate_dotnet_sdk_in_globaljson` | Validate global.json SDK compatibility |
| `validate_dotnet_sdk_installation` | Validate .NET SDK is installed |
| `authenticate_nuget_feed` | Resolve NuGet authentication failures |
| `open_dashboard` | Open modernization dashboard UI |

---

## 4. Recommended Workflow for .NET Upgrades

The agent follows a structured 3-phase workflow:

### Phase 1: Assessment

1. Call `get_state()` to check for existing scenarios
2. Call `get_scenarios()` to find available scenarios
3. Call `get_instructions(kind='scenario', query='<scenario_id>')` — **MANDATORY**
4. Load `scenario-initialization` skill
5. Gather parameters (source control defaults + scenario-specific + flow mode)
6. Present consolidated prompt to user
7. Handle source control (commit/stash, create branch)
8. Call `initialize_scenario` to create `.github/upgrades/{scenarioId}/` structure
9. Run assessment — creates `assessment.md`

### Phase 2: Planning

10. Generate upgrade plan — creates `plan.md` and `tasks.md`
11. Review plan (in Guided mode, pause for user approval)

### Phase 3: Execution

12. For each task in `availableTasks`:
    - `start_task(taskId)` — returns task content + related skills
    - Load relevant skills
    - Assess decomposition need
    - If decomposable: `break_down_task(taskId, subtasks)`
    - Research and enrich `task.md`
    - Execute code changes
    - Validate (build + tests)
    - Write `progress-details.md`
    - `complete_task(taskId, filesModified, executionLogSummary)`
    - Pick next task

### Flow Modes

| Mode | Behavior | Default |
|------|----------|---------|
| **Automatic** | Run end-to-end, only pause when blocked | Yes (default) |
| **Guided** | Pause after assessment, planning, complex breakdowns for user review | No |

### Workflow Files Structure

Created at `{RepoRoot}/.github/upgrades/{scenarioId}/`:

| File | Purpose |
|------|---------|
| `scenario-instructions.md` | Scenario spec, user preferences, persistent memory |
| `tasks.md` | Task hierarchy with status |
| `tasks/{taskId}/task.md` | Task plan and working memory |
| `tasks/{taskId}/progress-details.md` | Per-task change record |
| `execution-log.md` | Chronological progress log |

---

## 5. Available Scenarios

| Scenario | Description |
|----------|-------------|
| .NET Version Upgrade | Upgrade .NET projects to newer .NET versions |
| SDK-Style Project Conversion | Convert legacy .NET projects to SDK-style format |
| Azure Functions Upgrade | Upgrade Azure Functions from in-process to isolated worker model |
| Azure Migration | Migrate applications to Azure cloud services |
| Newtonsoft.Json Conversion | Migrate from Newtonsoft.Json to System.Text.Json |
| SqlClient Migration | Migrate from System.Data.SqlClient to Microsoft.Data.SqlClient |
| Semantic Kernel to Agents Framework | Migrate from Semantic Kernel to Microsoft Agents Framework |
| Aspire Integration | Add .NET Aspire support to existing applications (new in 1.0.1026) |
| Aspire Version Upgrade | Upgrade Aspire applications to newer versions (new in 1.0.1026) |

---

## 6. Available Skills (Built-in Migration Knowledge)

### Cloud Skills

- `migrating-azure-functions-startup` — In-process to isolated worker model
- `migrating-azure-functions-to-v2` — Azure Functions to v2 hosting pattern

### Common Skills

- `converting-to-sdk-style` — Legacy to SDK-style project files
- `integrating-autofac-with-dotnet` — Autofac DI to ASP.NET Core hosting
- `managing-package-references` — PackageReference/ProjectReference management
- `managing-target-frameworks` — Target framework management
- `migrating-autofac-to-dotnet-di` — Autofac to built-in DI
- `migrating-cryptography-namespaces` — System.Security.Cryptography modernization
- `migrating-newtonsoft-to-system-text-json` — Newtonsoft.Json to System.Text.Json
- `migrating-semantic-kernel-to-agents` — Semantic Kernel to Microsoft Agent Framework
- `migrating-to-msmq-messaging` — System.Messaging to MSMQ.Messaging
- `modifying-project-properties` — PropertyGroup modifications
- `migrating-nullable-references` — C# nullable reference types (new in 1.0.1017)
- `modernizing-csharp-code` — C# code modernization (new in 1.0.1017)

### Data Skills

- `migrating-edmx-to-code-first` — EF6 EDMX to EF Core Code-First
- `migrating-ef-dbcontext` — EF DbContext registration migration
- `migrating-ef6-code-first-to-ef-core` — EF6 Code-First to EF Core
- `migrating-linq-to-sql-to-ef-core` — LINQ to SQL to EF Core
- `migrating-to-microsoft-data-sqlclient` — System.Data.SqlClient to Microsoft.Data.SqlClient

### Libraries Skills

- `migrating-powershell-sdk` — Windows PowerShell 5.1 to PowerShell 7+

### Web (ASP.NET) Skills

- `migrating-aspnet-identity` — ASP.NET MVC Identity to ASP.NET Core Identity
- `migrating-global-asax` — Global.asax to middleware/Program.cs
- `migrating-owin-to-middleware` — OWIN to ASP.NET Core middleware

### Web (MVC) Skills

- `migrating-mvc-bundling` — System.Web.Optimization to modern bundling
- `migrating-mvc-filters` — Global filters to Core exception handling
- `migrating-mvc-routing` — RouteCollection to endpoint routing
- `incremental-mvc-webapi-updates` — Incremental MVC/WebApi migration (new in 1.0.1017)

### Web (WCF) Skills

- `migrating-wcf-to-corewcf` — WCF to CoreWCF for .NET 6+

### Workflow Skills (meta)

- `scenario-initialization` — Pre-initialization flow for any scenario
- `task-execution` — Task execution lifecycle
- `plan-generation` — Plan creation guidance
- `state-management` — Workflow state operations
- `tasks-consistency` — Task reconciliation when out of sync
- `user-interaction` — Communication patterns
- `sub-agent-delegation` — Delegating work to sub-agents

---

## 7. Key Implementation Details

### Prerequisites

- .NET SDK 10.0 or later required
- The MCP server runs as a .NET tool: `Microsoft.GitHubCopilot.Modernization.Mcp`
- VS Code requires the GitHub Copilot extension + subscription

### Environments Supported

1. **VS Code** — Via the `vscjava.migrate-java-to-azure` extension (includes modernize-dotnet)
2. **GitHub Copilot CLI** — Via the plugin marketplace
3. **GitHub Copilot Coding Agent** — Via agent definition in `.github/agents/`
4. **Visual Studio 2026 / 2022 17.14.17+** — Via built-in GitHub Copilot app modernization component

### Coding Agent Variant

The `coding-agent/modernize-dotnet.agent.md` is a non-interactive variant designed for GitHub Copilot Coding Agent (pull request automation):

- Runs all steps in one session without user input
- Uses the requested target framework and recommended branch name automatically
- Creates assessment, plan, tasks, then executes them
- Ensures no build warnings remain
- Updates `tasks.md` at the end with success/fails/skips
- Sets `APPMOD_CALLER_TYPE: copilot-coding-agent`

### Version History Highlights

| Version | Key Changes |
|---------|-------------|
| 1.0.1047-preview1 | Binding redirect analysis rules, improved working directory detection |
| 1.0.1037-preview1 | OpenTelemetry tracing, improved repo root detection |
| 1.0.1026-preview1 | Aspire integration + version upgrade scenarios |
| 1.0.1017-preview1 | Aspire support, nullable references skill, modernizing C# code skill, incremental MVC/WebApi |
| 1.0.956-preview1 | VS Code extension rebranding |
| 1.0.948-preview1 | Baseline with all existing skills and scenarios |

---

## 8. Relationship to VS Code Extension MCP Tools

The VS Code extension (`vscjava.migrate-java-to-azure` version 1.16.0) bundles additional MCP tools beyond what the plugin's MCP server provides. These are the `mcp_copilotmod_*` tools visible in the current session context. The plugin's MCP server provides the core workflow tools (get_state, initialize_scenario, etc.) while the VS Code extension adds .NET-specific analysis tools (get_solution_path, generate_dotnet_upgrade_assessment, etc.).

---

## References

- Local plugin path: `c:\Users\gappiah\.vscode\agent-plugins\github.com\dotnet\modernize-dotnet\`
- GitHub repository: https://github.com/dotnet/modernize-dotnet
- Plugin page: https://github.com/dotnet/modernize-dotnet/tree/main/plugins/modernize-dotnet
- Documentation: https://dotnet.microsoft.com/en-us/platform/modernize
- VS Code extension: `vscjava.migrate-java-to-azure` (1.16.0)

---

## Follow-On Questions (Not Investigated)

- What specific assessment rules does the MCP server apply for each scenario?
- How does the Aspire Integration scenario differ from the .NET Version Upgrade scenario in practice?
- What are the exact parameters accepted by `initialize_scenario` for each scenario type?
- How does the `query_dotnet_assessment` hierarchical scope system work in detail?

## Clarifying Questions

- None — all original research topics were answered through local file investigation and GitHub page content.
