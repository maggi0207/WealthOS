# WealthOS Backend — Phase 1 Foundation

Clean Architecture backend for the WealthOS platform.

## Projects

| Project | Responsibility |
|---------|----------------|
| `WealthOS.Api` | HTTP host, middleware, Swagger, health |
| `WealthOS.Application` | Use cases, DTOs, validation, mapping |
| `WealthOS.Domain` | Entities, domain rules, repository contracts |
| `WealthOS.Infrastructure` | EF Core, PostgreSQL, JWT, repositories |

## Local development

```bash
# Start PostgreSQL
docker compose -f docker/docker-compose.yml up -d

# Apply migrations
dotnet ef database update --project src/WealthOS.Infrastructure --startup-project src/WealthOS.Api

# Run API
dotnet run --project src/WealthOS.Api
```

## Endpoints

- Swagger: `https://localhost:5001/swagger` (Development)
- Health: `GET /api/v1/health`
- Probe: `GET /health`

## Module boundaries

Authentication, Dashboard, IncomeBusiness, Investments, Properties, Loans, Goals, Documents, Reports, Notifications, AIAdvisor, Settings, Shared.

Phase 1 establishes shared infrastructure only. Feature modules are implemented in subsequent phases.
