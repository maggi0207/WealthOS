# WealthOS Release Notes — v1.0.0

**Release date:** 2026-08-02  
**Codename:** Production readiness baseline

## Completed Features

### Platform
- ASP.NET Core 9 API with JWT auth, refresh tokens, FluentValidation, ProblemDetails, API versioning (`/api/v1`)
- PostgreSQL + EF Core migrations, soft-delete filters, audit interceptor, Identity roles (Admin / User)
- Hangfire background jobs (stubs + Angel One sync job), Serilog logging, health checks
- Docker Compose production stack (Postgres + API + Nginx TLS reverse proxy)
- Hostinger VPS deployment guide (`docs/HOSTINGER_DEPLOYMENT.md`)

### Frontend
- React / Vite SPA with shell navigation, theme, responsive Tailwind breakpoints
- Auth UI wired to real API when `VITE_API_MODE=api` (mock mode retained for local UI work)
- Module services with transparent mock ↔ API switching
- Dashboard, Properties, Loans, Income & Business, Investments, Goals, Documents, Notifications, AI Advisor, Reports

### Backend modules
- Auth (register, login, refresh, logout, me, forgot/reset stubs)
- Dashboard aggregation APIs
- Properties, Loans, Income/Business, Investments, Goals, Documents
- Notifications + reminders scaffolding
- AI Advisor (provider-backed; tool execution configurable)
- Reports / analytics export scaffolding
- Angel One SmartAPI **read-only** provider (trading explicitly disabled)

### Security / ops hardening (v1.0.0 QA)
- Production frontend defaults to `VITE_API_MODE=api`
- Auth provider obtains and refreshes JWTs in API mode
- Auth endpoint fixed-window rate limiting
- Hangfire dashboard restricted to Admin JWT in Production
- CORS empty-origin safe fail; JWT `RequireHttpsMetadata` outside Development
- Hangfire worker count respects `Hangfire:WorkerCount` configuration

## Known Limitations

- **Expenses** and **Settings** UI routes are placeholders (not feature-complete)
- **Angel One** live HTTP sync is stubbed until credentials + token exchange are provisioned; structure is read-only
- **AI Advisor** depends on configured AI provider keys; without them, behavior degrades to configured fallbacks
- **Forgot / reset password** prepare tokens but do not send email
- **Hangfire browser UX**: Production dashboard expects Bearer Admin JWT (not cookie login)
- **Integration tests** require Docker (Testcontainers); not runnable without Docker Engine
- **Frontend automated tests**: no dedicated unit/e2e suite in this release
- Module seed data may populate demo entities for development/admin accounts — review before public multi-tenant use
- Personal expenses module (separate from business expenses) is not shipped

## Deployment Notes

1. Copy `backend/docker/.env.example` → `.env` on the VPS; set strong `POSTGRES_PASSWORD`, `JWT_SECRET_KEY` (≥ 32 chars), and `CORS_ORIGIN_0`.
2. Mount TLS certs into `backend/docker/nginx/certs` (`fullchain.pem`, `privkey.pem`); update `server_name` in `nginx/conf.d/wealthos.conf`.
3. Start stack:
   ```bash
   cd backend/docker
   docker compose -f docker-compose.prod.yml --env-file .env up -d --build
   curl -fsS https://api.yourdomain.com/health
   ```
4. Build frontend with production env (`VITE_API_MODE=api`, `VITE_API_BASE_URL=https://api.yourdomain.com`):
   ```bash
   cd frontend && npm ci && npm run build
   ```
   Deploy `.output/public` (or Nitro host artifact) to the app origin matching CORS.
5. Optionally set `AdminSeed__Email` / `AdminSeed__Password` for the first Admin user.
6. Leave `ANGELONE_ENABLE_LIVE_SYNC=false` unless Angel One credentials are validated. Trading remains disabled in code.
7. Full steps: [`docs/HOSTINGER_DEPLOYMENT.md`](docs/HOSTINGER_DEPLOYMENT.md) and [`RELEASE_CHECKLIST.md`](RELEASE_CHECKLIST.md).
