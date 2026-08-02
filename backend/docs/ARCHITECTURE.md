# WealthOS Backend Architecture — Phase 2

## Layer responsibilities

- **Domain** — entities (including Identity-based `User`/`Role`), domain exceptions, repository contracts.
- **Application** — result pattern, DTO envelopes, auth contracts/validators/mapping, options.
- **Infrastructure** — EF Core Identity stores, PostgreSQL, JWT, refresh tokens, repositories, seeding.
- **Api** — composition root, middleware, Swagger, health, versioned auth controllers.

## Dependency direction

```
Api → Application → Domain
Api → Infrastructure → Application → Domain
```

## Authentication (Phase 2)

- ASP.NET Core Identity with `IdentityDbContext<User, Role, Guid>`
- JWT bearer access tokens + rotating refresh tokens
- Endpoints under `/api/v1/auth/*`
- Admin role/user seeded on startup (`AdminSeed` configuration)
- `Permission` / `RolePermission` entities reserved for future RBAC
- Forgot/reset password stubs (no email delivery)

## Database

- PostgreSQL via Npgsql
- Code-first migrations in `WealthOS.Infrastructure`
- Soft-delete and audit fields via `AuditableEntityInterceptor` (User soft-delete filter enabled)
- Startup applies migrations + Identity seed

## Next phases

1. **Phase 3 — Core modules** — Properties, Loans, Investments (read models first)
2. **Phase 4 — Remaining modules** — Goals, Documents, Income, Dashboard aggregation
