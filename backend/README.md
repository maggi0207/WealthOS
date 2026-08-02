# WealthOS Backend — Phase 2 Authentication

Clean Architecture backend for the WealthOS platform.

## Projects

| Project | Responsibility |
|---------|----------------|
| `WealthOS.Api` | HTTP host, middleware, Swagger, health, auth endpoints |
| `WealthOS.Application` | Use cases, DTOs, validation, mapping |
| `WealthOS.Domain` | Entities, domain rules, repository contracts |
| `WealthOS.Infrastructure` | EF Core, PostgreSQL, Identity, JWT, repositories |

## Local development

```bash
# Start PostgreSQL
docker compose -f docker/docker-compose.yml up -d

# Apply migrations (also applied automatically on API startup)
dotnet ef database update --project src/WealthOS.Infrastructure --startup-project src/WealthOS.Api

# Run API
dotnet run --project src/WealthOS.Api
```

Migrations and Identity seeding run on startup via `InitializeDatabaseAsync`.

## Authentication endpoints (`/api/v1/auth/...`)

Preferred versioned routes (Phase 1 convention):

| Method | Path | Auth |
|--------|------|------|
| POST | `/api/v1/auth/register` | Anonymous |
| POST | `/api/v1/auth/login` | Anonymous |
| POST | `/api/v1/auth/refresh` | Anonymous |
| POST | `/api/v1/auth/logout` | Anonymous |
| GET | `/api/v1/auth/me` | Bearer JWT |
| POST | `/api/v1/auth/forgot-password` | Anonymous (placeholder, no email) |
| POST | `/api/v1/auth/reset-password` | Anonymous (placeholder, no email) |

Note: Routes use `/api/v1/auth/...` to match Phase 1 versioning (not unversioned `/api/auth/...`).

Other endpoints:

- Swagger: `https://localhost:5001/swagger` (Development)
- Health: `GET /api/v1/health`
- Probe: `GET /health`

## Admin seed credentials

Configured under `AdminSeed` (override via environment variables in non-dev):

| Setting | Development default |
|---------|---------------------|
| Email | `admin@wealthos.local` |
| Password | `Admin@WealthOS1!` |

Environment overrides:

```bash
AdminSeed__Email=admin@wealthos.local
AdminSeed__Password=YourStrongPasswordHere!
```

If `AdminSeed:Password` is empty, admin user seeding is skipped (roles are still ensured).

## JWT configuration

```bash
Jwt__Issuer=https://api.wealthos.local
Jwt__Audience=wealthos-api
Jwt__SecretKey=YOUR_SECRET_AT_LEAST_32_CHARS_LONG
Jwt__AccessTokenExpirationMinutes=15
Jwt__RefreshTokenExpirationDays=7
```

## Testing

```bash
# Unit tests (validators + result flows)
dotnet test tests/WealthOS.UnitTests/WealthOS.UnitTests.csproj

# Integration tests (requires Docker for Testcontainers)
dotnet test tests/WealthOS.IntegrationTests/WealthOS.IntegrationTests.csproj

# Full solution build
dotnet build WealthOS.slnx
```

## Module boundaries

Authentication (Phase 2 complete), Dashboard, IncomeBusiness, Investments, Properties, Loans, Goals, Documents, Reports, Notifications, AIAdvisor, Settings, Shared.
