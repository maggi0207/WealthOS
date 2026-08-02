# Database

PostgreSQL is the system of record for WealthOS.

## Schema management

- Migrations live in `backend/src/WealthOS.Infrastructure/Persistence/Migrations/`
- Apply migrations: `dotnet ef database update --project backend/src/WealthOS.Infrastructure --startup-project backend/src/WealthOS.Api`

## Local PostgreSQL

Start via Docker:

```bash
docker compose -f backend/docker/docker-compose.yml up -d postgres
```

Connection string (development): see `backend/src/WealthOS.Api/appsettings.Development.json`

## Phase 1 tables

- `Users` — identity foundation
- `RefreshTokens` — refresh token rotation support

Feature tables are added in subsequent phases.
