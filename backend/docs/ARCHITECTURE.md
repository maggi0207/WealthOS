# WealthOS Backend Architecture — Phase 1

## Layer responsibilities

- **Domain** — entities, domain exceptions, repository contracts. No framework dependencies.
- **Application** — result pattern, DTO envelopes, service interfaces, options, validation, AutoMapper.
- **Infrastructure** — EF Core, PostgreSQL, JWT services, repositories, auditing interceptor.
- **Api** — composition root, middleware, Swagger, health, versioning.

## Dependency direction

```
Api → Application → Domain
Api → Infrastructure → Application → Domain
```

## Authentication foundation

- `User` and `RefreshToken` entities with EF configurations
- `IJwtTokenService`, `IPasswordHasher`, `ICurrentUserService` abstractions
- JWT bearer authentication configured; login endpoints deferred to Phase 2

## Database

- PostgreSQL via Npgsql
- Code-first migrations in `WealthOS.Infrastructure`
- Soft-delete and audit fields via `AuditableEntityInterceptor`

## Next phases

1. **Phase 2 — Authentication API** — register, login, refresh, revoke
2. **Phase 3 — Core modules** — Properties, Loans, Investments (read models first)
3. **Phase 4 — Remaining modules** — Goals, Documents, Income, Dashboard aggregation
