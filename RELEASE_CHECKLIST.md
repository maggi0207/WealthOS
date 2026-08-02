# WealthOS v1.0.0 — Release Checklist

Use this checklist before deploying WealthOS to production (Hostinger VPS + static frontend host).

## Pre-deploy verification

### Build & tests
- [ ] Frontend `npm run build` succeeds with `VITE_API_MODE=api`
- [ ] Backend `dotnet build` (Release) succeeds
- [ ] Backend unit tests pass (`dotnet test` on `WealthOS.UnitTests`)
- [ ] Integration tests pass when Docker is available (`WealthOS.IntegrationTests` / Testcontainers)
- [ ] No secrets committed (`.env`, certs, JWT keys, Angel One credentials)

### Frontend
- [ ] Auth: login / register / logout / refresh work against live API
- [ ] Shell route guard redirects unauthenticated users to `/login`
- [ ] Modules load with loading / empty / error states
- [ ] Mock/API switch: production uses `api`; local UI work may use `mock`
- [ ] Responsive patterns reviewed for ~360 / 390 / 430 / tablet / desktop (Tailwind `sm`/`md`/`lg`)
- [ ] Expenses & Settings remain intentional placeholders (documented limitations)

### Backend / API
- [ ] JWT configured via env (`JWT_SECRET_KEY` ≥ 32 chars)
- [ ] CORS locked to app origin (`CORS_ORIGIN_0`)
- [ ] Auth endpoints rate-limited
- [ ] Controllers require auth (fallback policy); health is anonymous
- [ ] ProblemDetails / FluentValidation errors return correctly
- [ ] Swagger UI disabled outside Development
- [ ] Hangfire dashboard requires Admin JWT in Production
- [ ] Serilog console logging active

### Database
- [ ] Migrations apply on startup (`MigrateAsync`)
- [ ] Soft-delete query filters present
- [ ] Audit interceptor active
- [ ] Indexes / FKs from module migrations present
- [ ] Admin seed password set via env when an admin account is required
- [ ] Postgres volume backed up (`wealthos_postgres_data`)

### Modules (smoke)
- [ ] Auth
- [ ] Dashboard
- [ ] Properties
- [ ] Loans
- [ ] Income & Business
- [ ] Investments
- [ ] Goals
- [ ] Documents
- [ ] Notifications
- [ ] AI Advisor
- [ ] Reports
- [ ] Angel One — read-only only (`EnableLiveSync` optional; trading never enabled)

### Security
- [ ] HTTPS / TLS certs mounted for Nginx
- [ ] Security headers in Nginx (`HSTS`, `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`)
- [ ] Firewall: only 80/443 public
- [ ] No trading endpoints callable for Angel One
- [ ] Rate limiting on `/api/v*/auth/*`
- [ ] Secrets only in VPS `.env` / vault

### Docker / Nginx / Hostinger
- [ ] `docker compose -f docker-compose.prod.yml --env-file .env up -d --build`
- [ ] `GET /health` returns healthy
- [ ] Nginx HTTP→HTTPS redirect works
- [ ] Follow [`docs/HOSTINGER_DEPLOYMENT.md`](docs/HOSTINGER_DEPLOYMENT.md)
- [ ] Frontend artifact (`.output/public` or host preset) deployed with correct `VITE_API_BASE_URL`

### Angel One
- [ ] `ANGELONE_ENABLE_LIVE_SYNC=false` unless credentials validated
- [ ] Confirm `PlaceOrder` remains disabled in code
- [ ] No trading UI or APIs exposed

### Release artifacts
- [ ] [`RELEASE_NOTES_v1.0.0.md`](RELEASE_NOTES_v1.0.0.md) reviewed
- [ ] This checklist completed and signed off by Release Manager
