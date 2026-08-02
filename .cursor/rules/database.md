# WealthOS Database Design Standards

This document defines the permanent database design standards for WealthOS. All schema design, EF Core models, and migrations must follow these rules.

## Database Technology

| Concern | Choice |
|---------|--------|
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Schema evolution | Code First Migrations |

---

# Database Philosophy

- Normalize data appropriately (typically up to 3NF).
- Maintain data integrity.
- Prefer simplicity over premature optimization.
- Design for long-term scalability.
- Business rules belong in the application layer, not the database.

The database enforces structure, constraints, and persistence—not domain workflows or authorization logic.

---

# Naming Conventions

## Tables

Use **plural** names.

Examples:

```
Users
Properties
Assets
Loans
Investments
Documents
Activities
Notifications
```

## Columns

Use **PascalCase**.

Examples:

```
PropertyId
CreatedAt
UpdatedAt
UserId
```

## Foreign Keys

Pattern: `<EntityName>Id`

Examples:

```
PropertyId
LoanId
UserId
```

## Primary Keys

- Column name: `Id`
- **GUID preferred** for distributed-friendly identifiers unless a documented exception applies.

---

# Auditing

Every table must include:

| Column | Purpose |
|--------|---------|
| `Id` | Primary key |
| `CreatedAt` | Record creation timestamp |
| `CreatedBy` | Creator identity |
| `UpdatedAt` | Last update timestamp |
| `UpdatedBy` | Last updater identity |
| `IsDeleted` | Soft-delete flag |
| `DeletedAt` | Soft-delete timestamp |

Use soft delete where appropriate. Queries must exclude deleted records by default unless explicitly including them for admin or audit scenarios.

---

# Relationships

- Use explicit foreign keys.
- Configure relationships using Fluent API.
- Avoid implicit relationships.
- Never use cascade delete unless explicitly required and documented.

Prefer clear ownership and delete behavior defined in `IEntityTypeConfiguration` classes, not ad hoc conventions.

---

# Entity Framework

- Use Code First.
- Use `IEntityTypeConfiguration<T>` for every entity.
- Keep entities clean.
- Avoid business logic inside entities.
- Use navigation properties appropriately.
- Always use async database operations (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`, etc.).

EF types and DbContext belong in Infrastructure. Domain entities must not depend on EF Core attributes or APIs unless the project explicitly documents a shared persistence model pattern.

---

# Migrations

- Every schema change must use EF Core migrations.
- Never modify production tables manually.
- Migration names should be descriptive.

Examples:

```
AddPropertyTable
AddLoanIndexes
CreateAssetTransactions
```

Review generated SQL for production impact before applying. Plan rollback paths for risky changes.

---

# Indexing

Create indexes only when necessary.

Index:

- Foreign keys
- Frequently searched columns
- Unique values

Avoid excessive indexing. Each index adds write cost and storage overhead—add them for measured query needs, not speculation.

---

# Constraints

Use database constraints:

- **NOT NULL** where appropriate
- **Unique** constraints where required
- **Foreign key** constraints for relationships
- **Check** constraints where useful

Constraints complement application validation; they do not replace it.

---

# Data Types

Choose appropriate PostgreSQL types:

| Type | Use |
|------|-----|
| `UUID` | Identifiers |
| `TEXT` | Unbounded text |
| `VARCHAR` | Bounded text |
| `BOOLEAN` | Flags |
| `TIMESTAMP WITH TIME ZONE` | Timestamps |
| `NUMERIC` | Money and precise decimals |

Never use floating point for financial values. Always use `decimal` / `NUMERIC` for currency.

---

# Financial Data

- Store money using `NUMERIC(18,2)`.
- Never use `float` or `double`.
- Support multiple currencies in the future.
- Store `CurrencyCode` separately when needed.

Financial amounts must remain exact at persistence and calculation layers.

---

# Soft Delete

Prefer soft delete.

Use:

- `IsDeleted`
- `DeletedAt`

Exclude deleted records by default in application queries and global filters where appropriate.

---

# Performance

- Avoid N+1 queries.
- Use projections (`Select`) to load only required shapes.
- Use pagination for large result sets.
- Avoid `SELECT *` patterns—load only required columns.
- Prefer `IQueryable` composition until execution; materialize deliberately.

---

# Security

- Never store plaintext passwords.
- Never store secrets in the database.
- Encrypt sensitive data where appropriate.
- Restrict database credentials and connection strings to secure configuration.

---

# Seed Data

- Use EF Core seed data only for development.
- Never seed production-sensitive information.
- Keep seed scripts idempotent and environment-aware.

---

# Backup Strategy

The database environment must support:

- Daily backups
- Point-in-time recovery
- Migration rollback

Operational runbooks belong in `docs/`; schema changes must remain reversible where feasible.

---

# AI Rules

When generating database code:

- Create one entity at a time.
- Create matching configuration classes.
- Create migrations separately.
- Do not regenerate existing entities.
- Preserve existing relationships.
- Never rename production tables automatically.

Also align with `.cursor/rules/backend.md`, `.cursor/rules/architecture.md`, and `.cursor/rules/coding.md`.

---

# Financial Precision

- All monetary calculations must use `decimal` in C#.
- All monetary columns must use `NUMERIC(18,2)` in PostgreSQL.
- Never use `float` or `double` for financial values.
- Round only at the presentation layer.

---

These database standards are mandatory for every Entity Framework model and PostgreSQL schema in WealthOS.
