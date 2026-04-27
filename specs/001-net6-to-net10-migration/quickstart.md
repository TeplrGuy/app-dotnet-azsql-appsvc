# Quickstart: Build, Run, and Test After Migration

**Date**: 2026-04-27
**Prerequisites**: .NET 10 SDK (10.0.203+), Docker Desktop, SQL Server (or Azure SQL Edge via Docker)

## 1. Restore and Build

```bash
cd src
dotnet restore ContosoUniversity.sln
dotnet build ContosoUniversity.sln --configuration Release
```

Expected: Build succeeds with 0 errors, 0 warnings (or pre-existing warnings only).

## 2. Run Unit Tests

```bash
dotnet test ContosoUniversity.Test/ContosoUniversity.Test.csproj --configuration Release --verbosity normal
```

Expected: All tests pass.

## 3. Start Local Database

```bash
cd ..
docker compose up -d azure-sql-edge
```

Wait 10 seconds for SQL to initialize, then verify:

```bash
docker compose ps
```

## 4. Run the API

```bash
dotnet run --project src/ContosoUniversity.API
```

Verify:

* Health: `curl https://localhost:5001/health` returns "Healthy"
* Swagger: Open `https://localhost:5001/swagger` in browser

## 5. Run the Web Application

```bash
dotnet run --project src/ContosoUniversity.WebApplication
```

Verify:

* Home: `https://localhost:5000` loads the Contoso University home page
* Students: `https://localhost:5000/Students` lists students

## 6. Run Full Stack via Docker Compose

```bash
docker compose build
docker compose up -d
```

Verify all three containers are running:

```bash
docker compose ps
```

## 7. Verify EF Core Schema Compatibility

```bash
cd src/ContosoUniversity.API
dotnet ef migrations has-pending-model-changes
```

Expected: "No pending model changes."

## 8. Run Load Tests (Smoke Profile)

```bash
cd loadtests
./run-local.sh  # or run-local.ps1 on Windows
```

Or use the Locust smoke profile:

```bash
cd loadtests/locust
locust -f locustfile.py --headless -u 5 -r 1 --run-time 30s
```

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| `global.json` SDK not found | Wrong SDK version | Verify `dotnet --list-sdks` shows 10.0.203+ |
| EF migration error | Package version mismatch | Ensure all EF packages are 10.0.7 |
| Docker build fails | Old base image cached | Run `docker compose build --no-cache` |
| SQL connection timeout | Database not ready | Wait 15s after `docker compose up` |
