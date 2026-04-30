# Quickstart: Validating the .NET 10 Migration

**Feature**: 001-net6-to-net10-migration
**Date**: 2026-04-27

## Prerequisites

- .NET 10 SDK installed (`dotnet --version` returns `10.0.x`)
- Docker Desktop (for container validation)
- Azure CLI with Bicep (for infrastructure validation)
- Access to Azure SQL Database (for EF Core schema verification)

## Step 1: Verify SDK Installation

```bash
dotnet --version
# Expected: 10.0.100 or higher
```

## Step 2: Build the Solution

```bash
cd src
dotnet restore ContosoUniversity.sln
dotnet build ContosoUniversity.sln --configuration Release
```

**Expected**: Zero errors, zero package-compatibility warnings.

## Step 3: Run Unit Tests

```bash
dotnet test ContosoUniversity.Test/ContosoUniversity.Test.csproj --configuration Release --verbosity normal
```

**Expected**: All tests pass. No test removals from the original suite.

## Step 4: Run the API Locally

```bash
dotnet run --project ContosoUniversity.API
```

**Verify**:
- Navigate to `https://localhost:5001/openapi/v1.json` -- should return OpenAPI document
- Navigate to `https://localhost:5001/swagger` -- should show Swagger UI
- Test a GET endpoint (e.g., `/api/Students`) -- should return data

## Step 5: Run the WebApplication Locally

```bash
dotnet run --project ContosoUniversity.WebApplication
```

**Verify**:
- Navigate to `https://localhost:5001/` -- should redirect to Index/Students page
- Navigate to `/Students`, `/Courses`, `/health` -- all should respond

## Step 6: Verify EF Core Schema Compatibility

```bash
cd ContosoUniversity.API
dotnet ef migrations add VerifyNoChanges --context ContosoUniversityAPIContext
```

**Expected**: The generated migration should have empty `Up()` and `Down()` methods,
confirming no schema drift. Delete the migration file after verification:

```bash
dotnet ef migrations remove
```

## Step 7: Build Docker Images

```bash
cd src
docker build -f ContosoUniversity.API/Dockerfile -t contoso-api:net10 .
docker build -f ContosoUniversity.WebApplication/Dockerfile -t contoso-web:net10 .
```

**Expected**: Both images build successfully.

## Step 8: Run Containers

```bash
cd ..
docker-compose up -d
```

**Verify**: Both services respond on their configured ports.

## Step 9: Validate Bicep

```bash
az bicep build --file infra/main.bicep
```

**Expected**: Zero errors. Inspect the output JSON for `netFrameworkVersion: 'v10.0'`.

## Step 10: Validate CI/CD (Manual)

Push the branch and open a PR. Verify:
- GitHub Actions installs .NET 10 SDK
- Build step succeeds
- Test step succeeds
- All quality gates pass

## Troubleshooting

| Symptom | Likely Cause | Fix |
|---------|--------------|-----|
| `NETSDK1045: The current .NET SDK does not support targeting .NET 10.0` | Wrong SDK installed | Install .NET 10 SDK; check `global.json` |
| EF Core migration has non-empty Up() | Convention drift | Pin conventions in DbContext `OnModelCreating` |
| Newtonsoft.Json serialization errors | Missing `[JsonPropertyName]` attributes | Replace `[JsonProperty]` with STJ equivalent |
| Docker build fails on restore | Package source issue | Ensure NuGet.config points to nuget.org |
| Tests fail with MSTest errors | MSTest v3 API changes | Update `[TestMethod]` usage if needed (usually compatible) |
| Swagger UI 404 | NSwag middleware still referenced | Ensure `UseOpenApi`/`UseSwaggerUi3` are replaced |
