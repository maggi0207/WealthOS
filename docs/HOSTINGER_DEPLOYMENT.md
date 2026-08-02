# Hostinger VPS deployment — WealthOS

This guide deploys the WealthOS ASP.NET Core API behind Nginx with TLS on a Hostinger VPS using Docker Compose.

## Prerequisites

- Hostinger VPS with Ubuntu 22.04+ and root/SSH access
- Domain DNS A records for `api.yourdomain.com` (and optionally app)
- Docker Engine + Docker Compose plugin installed
- TLS certificates (Hostinger SSL or Certbot / Let's Encrypt)

## 1. Install Docker

```bash
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER
```

Log out and back in, then verify:

```bash
docker --version
docker compose version
```

## 2. Clone and configure

```bash
git clone https://github.com/maggi0207/WealthOS.git
cd WealthOS/backend/docker
cp .env.example .env
nano .env   # set POSTGRES_PASSWORD, JWT_SECRET_KEY, CORS_ORIGIN_0, domain values
```

Place TLS files:

```bash
mkdir -p nginx/certs
# copy fullchain.pem and privkey.pem into nginx/certs/
```

Update `nginx/conf.d/wealthos.conf` `server_name` to your API hostname.

## 3. Start production stack

```bash
docker compose -f docker-compose.prod.yml --env-file .env up -d --build
docker compose -f docker-compose.prod.yml ps
curl -fsS https://api.yourdomain.com/health
```

## 4. Frontend

Build the SPA with API mode:

```bash
cd ../../frontend
# set VITE_API_MODE=api and VITE_API_BASE_URL=https://api.yourdomain.com in .env.production
npm ci
npm run build
```

Deploy `.output/public` (or your host's static artifact) to Hostinger static hosting, Cloudflare Pages, or an Nginx `root` for the app subdomain. Ensure CORS origin matches `CORS_ORIGIN_0`.

## 5. Operations

| Task | Command |
|------|---------|
| Logs | `docker compose -f docker-compose.prod.yml logs -f api` |
| Restart | `docker compose -f docker-compose.prod.yml restart api` |
| Update | `git pull && docker compose -f docker-compose.prod.yml up -d --build` |
| Hangfire | Browse `https://api.yourdomain.com/hangfire` (protect in prod) |

## 6. CI/CD readiness

Suggested GitHub Actions flow:

1. `dotnet test` / `dotnet build` on backend
2. `npm run build` on frontend
3. SSH deploy: pull + `docker compose -f docker-compose.prod.yml up -d --build`
4. Smoke `GET /health`

Secrets (JWT, DB password, Angel One) must live in the VPS `.env` or a vault — never in the repo.

## 7. Angel One

Leave `ANGELONE_*` empty for stub sync. To enable read-only SmartAPI structure:

- Set `ANGELONE_API_KEY`, `ANGELONE_CLIENT_CODE`
- Set `ANGELONE_ENABLE_LIVE_SYNC=true` only after validating credentials
- Trading remains disabled in code

## Security checklist

- [ ] Strong `JWT_SECRET_KEY` (≥ 32 chars)
- [ ] Strong Postgres password
- [ ] TLS certificates valid
- [ ] CORS locked to app origin
- [ ] Hangfire dashboard restricted
- [ ] Firewall: only 80/443 public
- [ ] Regular backups of `wealthos_postgres_data`
