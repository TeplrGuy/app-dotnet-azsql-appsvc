# API Contracts: Contoso University

**Date**: 2026-04-27
**Status**: Preserved (no endpoint changes during migration)

## Overview

The API contract is **frozen** for this migration. All existing endpoints, request/response shapes, and status codes remain unchanged. This document captures the contract for verification after the upgrade.

## Base URL

- Local: `https://localhost:5001`
- Deployed: `https://{environmentName}-api.azurewebsites.net`

## Endpoints

### Students

| Method | Path | Request | Response | Status Codes |
|--------|------|---------|----------|--------------|
| GET | /api/Students | - | Student[] | 200 |
| GET | /api/Students/{id} | - | Student | 200, 404 |
| POST | /api/Students | StudentCreateDTO | Student | 201, 400 |
| PUT | /api/Students/{id} | StudentUpdateDTO | - | 204, 400, 404 |
| DELETE | /api/Students/{id} | - | - | 204, 404 |

### Courses

| Method | Path | Request | Response | Status Codes |
|--------|------|---------|----------|--------------|
| GET | /api/Courses | - | Course[] | 200 |
| GET | /api/Courses/{id} | - | Course | 200, 404 |
| POST | /api/Courses | CourseCreateDTO | Course | 201, 400 |
| PUT | /api/Courses/{id} | CourseUpdateDTO | - | 204, 400, 404 |
| DELETE | /api/Courses/{id} | - | - | 204, 404 |

### Instructors

| Method | Path | Request | Response | Status Codes |
|--------|------|---------|----------|--------------|
| GET | /api/Instructors | - | Instructor[] | 200 |
| GET | /api/Instructors/{id} | - | Instructor | 200, 404 |

### Enrollments

| Method | Path | Request | Response | Status Codes |
|--------|------|---------|----------|--------------|
| GET | /api/Enrollments | - | Enrollment[] | 200 |

### Health

| Method | Path | Request | Response | Status Codes |
|--------|------|---------|----------|--------------|
| GET | /health | - | "Healthy" | 200 |

### OpenAPI / Swagger

| Method | Path | Description |
|--------|------|-------------|
| GET | /swagger/v1/swagger.json | OpenAPI spec (NSwag) |
| GET | /swagger | Swagger UI |

## Serialization

- Request/response bodies use JSON (`application/json`)
- API project uses `Microsoft.AspNetCore.Mvc.NewtonsoftJson` for serialization (upgraded to 10.0.7)
- Date format: ISO 8601 (`yyyy-MM-ddTHH:mm:ss`)

## Authentication

- No authentication on API endpoints (relies on network-level security in Azure)
- Managed identity used for Azure SQL connection (via Azure.Identity)

## Post-Migration Verification

After upgrade, confirm all endpoints return identical responses:

```bash
# Health check
curl https://localhost:5001/health
# Expected: 200 "Healthy"

# Students list
curl https://localhost:5001/api/Students
# Expected: 200 with JSON array

# Swagger
curl https://localhost:5001/swagger/v1/swagger.json
# Expected: 200 with OpenAPI JSON
```
