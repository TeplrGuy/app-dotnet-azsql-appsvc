# Data Model: .NET 6 to .NET 10 LTS Migration

**Date**: 2026-04-27
**Status**: Schema preserved (no changes)

## Overview

The database schema is **frozen** for this migration. Entity Framework Core is upgraded from 7.0.4 to 10.0.7, but the data model mapping remains identical. This document captures the existing entities to confirm no schema drift occurs.

## Entities (Unchanged)

### Student

| Field | Type | Constraints |
|-------|------|-------------|
| ID | int | PK, auto-increment |
| LastName | string | Required, max 50 |
| FirstMidName | string | Required, max 50 |
| EnrollmentDate | DateTime | Required |

**Relationships**: One-to-many with Enrollment

### Course

| Field | Type | Constraints |
|-------|------|-------------|
| CourseID | int | PK (user-assigned) |
| Title | string | Required, max 50 |
| Credits | int | Required, range 1-5 |
| DepartmentID | int | FK to Department |

**Relationships**: Many-to-one with Department, One-to-many with Enrollment, Many-to-many with Instructor (via CourseAssignment)

### Instructor

| Field | Type | Constraints |
|-------|------|-------------|
| ID | int | PK, auto-increment |
| LastName | string | Required, max 50 |
| FirstMidName | string | Required, max 50 |
| HireDate | DateTime | Required |

**Relationships**: One-to-one with OfficeAssignment, Many-to-many with Course (via CourseAssignment)

### Enrollment

| Field | Type | Constraints |
|-------|------|-------------|
| EnrollmentID | int | PK, auto-increment |
| CourseID | int | FK to Course |
| StudentID | int | FK to Student |
| Grade | enum (nullable) | A, B, C, D, F |

**Relationships**: Many-to-one with Student, Many-to-one with Course

### Department

| Field | Type | Constraints |
|-------|------|-------------|
| DepartmentID | int | PK, auto-increment |
| Name | string | Required, max 50 |
| Budget | decimal | Required |
| StartDate | DateTime | Required |
| InstructorID | int (nullable) | FK to Instructor (administrator) |

**Relationships**: One-to-many with Course, One-to-one with Instructor (admin)

## EF Core Migration Validation

After the upgrade, run:

```bash
cd src/ContosoUniversity.API
dotnet ef migrations has-pending-model-changes
```

Expected output: "No pending model changes" — confirming schema parity.

## Convention Changes to Monitor

EF Core 10 introduces convention changes that could affect mapping:

1. **TimeOnly/DateOnly**: Not used in this schema (safe)
2. **Decimal precision**: Already configured via fluent API (safe)
3. **String length defaults**: Already specified via attributes (safe)
4. **Nullable reference types**: Project does not enable NRT (safe)

No explicit fluent overrides needed for this schema.
