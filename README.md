# WealthOS

Enterprise-grade personal wealth management platform.

## Repository layout

| Path | Purpose |
|------|---------|
| `frontend/` | React 19 / Vite / TanStack UI |
| `backend/` | .NET 9 ASP.NET Core API |
| `docker/` | **Production** Compose + env templates (shared VPS) |
| `docs/` | Architecture and Hostinger deployment |
| `database/` | Schema notes / scripts |
| `scripts/` | Operational helpers |
| `.cursor/` | AI agent rules |

## Local development

- Frontend: `cd frontend && npm run dev` (default `http://localhost:8080`)
- API: `cd backend && dotnet run --project src/WealthOS.Api --launch-profile http`
- Optional API+Postgres: `backend/docker/docker-compose.yml`

## Production (Hostinger shared VPS)

Host Nginx owns **80/443**. WealthOS containers publish **3000** (SPA) and **8081** (API) only — host **8080** is reserved for layaKPI.

```bash
cp docker/.env.production.example docker/.env.production
# edit secrets
docker compose -f docker/docker-compose.prod.yml --env-file docker/.env.production up -d --build
```

Full guide: [`docs/HOSTINGER_DEPLOYMENT.md`](docs/HOSTINGER_DEPLOYMENT.md)  
Host Nginx sample: [`docs/nginx-hostinger.conf`](docs/nginx-hostinger.conf)

Public hosts:

- App: `https://wealthos.devenlight.com`
- API: `https://api.wealthos.devenlight.com`
- Health: `https://api.wealthos.devenlight.com/health` (alias `/api/health`)
