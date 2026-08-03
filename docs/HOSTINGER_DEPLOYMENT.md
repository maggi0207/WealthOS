# Hostinger VPS deployment — WealthOS

This guide deploys WealthOS on the **same Hostinger VPS** already running
[layaKPI-Tracker](https://github.com/maggi0207/layaKPI-Tracker) on the main domain.

| App | Path on VPS | Domain role |
|-----|-------------|-------------|
| layaKPI-Tracker | `~/layaKPI-Tracker` | Main domain (`/` landing, `/app` dashboard, `/api`) |
| WealthOS | `~/WealthOS` | **Subdomain** (recommended): `api.` / `wealth.` — see § Shared VPS notes |

Deploy automation mirrors layaKPI: GitHub Actions → SSH → `git reset` → `docker compose`.

## Prerequisites

- Hostinger VPS with Ubuntu 22.04+ (already used by layaKPI)
- Docker Engine + Docker Compose plugin
- Subdomain DNS A records (do **not** bind WealthOS to port 80 if layaKPI already owns it)
- TLS certificates for the WealthOS hostnames

## Shared VPS notes (important)

layaKPI publishes **host port 80** (and often 8080 / 5432). WealthOS `docker-compose.prod.yml` also wants **80/443**.

Recommended layout:

1. Keep layaKPI on the main domain (port 80).
2. Point `api.yourdomain.com` (and optional `wealth.yourdomain.com`) at the VPS.
3. Either:
   - Change WealthOS nginx published ports in compose (e.g. `9080:80`, `9443:443`) and terminate TLS at Hostinger / Cloudflare proxy → those ports, **or**
   - Stop publishing layaKPI on 80 only if you intentionally move that stack.

WealthOS Postgres should stay on the **internal** Docker network (do not publish `5432` if layaKPI already uses it).

## 1. One-time VPS setup

```bash
# Clone beside layaKPI (same home directory pattern)
cd ~
git clone https://github.com/maggi0207/WealthOS.git
cd WealthOS/backend/docker
cp .env.example .env
nano .env   # POSTGRES_PASSWORD, JWT_SECRET_KEY, CORS_ORIGIN_0, domain values

mkdir -p nginx/certs
# copy fullchain.pem and privkey.pem into nginx/certs/
```

Update `nginx/conf.d/wealthos.conf` `server_name` to your API hostname (e.g. `api.yourdomain.com`).

First start:

```bash
docker compose -f docker-compose.prod.yml --env-file .env up -d --build
docker compose -f docker-compose.prod.yml ps
curl -fsS https://api.yourdomain.com/health
```

## 2. Frontend

Build with API mode pointing at the WealthOS API subdomain:

```bash
cd ~/WealthOS/frontend   # or build in CI / locally
cp .env.production.example .env.production
# VITE_API_BASE_URL=https://api.yourdomain.com
npm ci
npm run build
```

Deploy `.output/public` to Hostinger static hosting, Cloudflare Pages, or an Nginx root for `wealth.yourdomain.com`.
CORS origin must match `CORS_ORIGIN_0` in the VPS `.env`.

## 3. GitHub Actions (same secrets as layaKPI)

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| `ci.yml` | Push / PR to `main` or `develop` | Backend Release build + unit tests; frontend build |
| `deploy-hostinger.yml` | Push to `main` or **Run workflow** | SSH deploy (identical pattern to layaKPI) |

### Secrets (reuse layaKPI names)

Add these to **WealthOS → Settings → Secrets and variables → Actions**
(same values already used by layaKPI-Tracker):

| Secret | Used by | Notes |
|--------|---------|-------|
| `VPS_HOST` | Deploy | Hostinger VPS IP / hostname |
| `VPS_USER` | Deploy | SSH user (e.g. the account that owns `~/WealthOS`) |
| `VPS_SSH_KEY` | Deploy | Private key PEM |

Prefer **Organization secrets** so both repos share one set.

Deploy script (on the VPS):

```text
cd ~/WealthOS
git fetch --all
git reset --hard origin/main
cd backend/docker
sudo docker compose -f docker-compose.prod.yml --env-file .env down
sudo docker compose -f docker-compose.prod.yml --env-file .env build
sudo docker compose -f docker-compose.prod.yml --env-file .env up -d
sudo docker image prune -f
```

Optional repo variable for CI frontend builds: `VITE_API_BASE_URL`.

## 4. Operations

| Task | Command |
|------|---------|
| Logs | `cd ~/WealthOS/backend/docker && docker compose -f docker-compose.prod.yml logs -f api` |
| Restart | `docker compose -f docker-compose.prod.yml restart api` |
| Manual update | Same as the Actions script above |
| Hangfire | `https://api.yourdomain.com/hangfire` with Admin JWT |

App secrets (JWT, DB password, Angel One) stay in `~/WealthOS/backend/docker/.env` — never in git.

## 5. Angel One

Leave `ANGELONE_*` empty for stub sync. To enable read-only SmartAPI structure:

- Set `ANGELONE_API_KEY`, `ANGELONE_CLIENT_CODE`
- Set `ANGELONE_ENABLE_LIVE_SYNC=true` only after validating credentials
- Trading remains disabled in code

## Security checklist

- [ ] Strong `JWT_SECRET_KEY` (≥ 32 chars)
- [ ] Strong Postgres password (WealthOS DB, not shared with layaKPI)
- [ ] TLS certificates valid for WealthOS hostnames
- [ ] CORS locked to WealthOS app origin
- [ ] No port 80/443 clash with layaKPI
- [ ] Hangfire restricted (Admin JWT)
- [ ] Firewall: only intended public ports open
- [ ] Regular backups of `wealthos_postgres_data`
