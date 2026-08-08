# Hostinger VPS deployment — WealthOS (shared VPS)

WealthOS coexists with other sites (e.g. layaKPI-Tracker) on the same Hostinger VPS.
**Host Nginx** owns ports **80/443**. Docker publishes only:

| Service  | Host port | Container |
|----------|-----------|-----------|
| Frontend | `3000`    | node:3000 (TanStack Start) |
| API      | `8081`    | aspnet:8080 (host **8080** is used by layaKPI) |
| Postgres | *(none)*  | internal  |

```
Internet
  → Host Nginx (80/443 + Let's Encrypt)
      → wealthos.devenlight.com     → 127.0.0.1:3000  (frontend)
      → api.wealthos.devenlight.com → 127.0.0.1:8081  (API)
          → postgres (Docker network)
```

## 1. Server preparation

- Ubuntu 22.04+ / 24.04 with host Nginx already serving other sites
- DNS A records:
  - `wealthos.devenlight.com` → VPS IP
  - `api.wealthos.devenlight.com` → VPS IP
- Docker Engine + Compose plugin

```bash
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER
# re-login, then:
docker --version
docker compose version
```

## 2. Clone repository

```bash
cd ~
git clone https://github.com/maggi0207/WealthOS.git
cd WealthOS
```

## 3. Configure environment

```bash
cp docker/.env.production.example docker/.env.production
nano docker/.env.production
```

Required values:

| Variable | Example |
|----------|---------|
| `POSTGRES_PASSWORD` | strong random password |
| `JWT_SECRET_KEY` | ≥ 32 characters |
| `CORS_ORIGIN_0` | `https://wealthos.devenlight.com` |
| `JWT_ISSUER` | `https://api.wealthos.devenlight.com` |
| `VITE_API_BASE_URL` | `https://api.wealthos.devenlight.com` |

Optional: `ADMIN_SEED_*`, `ANGELONE_*`.

## 4. Start containers

From repository root (**do not** bind 80/443):

```bash
docker compose -f docker/docker-compose.prod.yml --env-file docker/.env.production up -d --build
docker compose -f docker/docker-compose.prod.yml ps
```

Local validation on the VPS:

```bash
curl -fsS http://127.0.0.1:3000/          # frontend
curl -fsS http://127.0.0.1:8081/health    # API health
```

## 5. Free host ports 80/443 (required on shared VPS)

**Host Nginx** must own **80/443**. If another Docker stack publishes them (e.g. layaKPI), host Nginx and Certbot will fail with `bind() ... Address already in use`.

Check:

```bash
sudo ss -tlnp | grep -E ':80|:443'
sudo docker ps --format 'table {{.Names}}\t{{.Ports}}'
```

If you see `kpi_tracker_frontend … 0.0.0.0:80->80/tcp`, remap it off 80 (example: host **3001**):

```bash
# Locate compose (typical labels):
docker inspect kpi_tracker_frontend --format '{{index .Config.Labels "com.docker.compose.project.working_dir"}}'
docker inspect kpi_tracker_frontend --format '{{index .Config.Labels "com.docker.compose.project.config_files"}}'
```

In that compose file, change the frontend publish from `80:80` to `127.0.0.1:3001:80`, then recreate:

```bash
cd <kpi-compose-dir>
# edit ports for the frontend service, then:
docker compose up -d --force-recreate
# confirm Docker no longer holds :80
sudo ss -tlnp | grep ':80' || echo "port 80 is free"
```

Add a **host** Nginx site for the KPI domain(s) that proxies to `127.0.0.1:3001` (and API to `127.0.0.1:8080`) so the existing site keeps working. Prefer binding KPI ports to `127.0.0.1` only.

## 6. Configure host Nginx (HTTP first)

Use the HTTP-only sample so Nginx starts **before** certificates exist:

```bash
sudo mkdir -p /var/www/certbot
sudo cp ~/WealthOS/docs/nginx-hostinger.conf /etc/nginx/sites-available/wealthos
sudo ln -sf /etc/nginx/sites-available/wealthos /etc/nginx/sites-enabled/wealthos
sudo nginx -t
sudo systemctl start nginx   # use start if inactive; reload if already running
sudo systemctl enable nginx
```

If a previous broken SSL site is enabled, remove or fix it first:

```bash
sudo rm -f /etc/nginx/sites-enabled/wealthos
# then re-copy the HTTP-only file as above
```

The sample config is in [`docs/nginx-hostinger.conf`](./nginx-hostinger.conf).

## 7. Configure SSL (Let's Encrypt on the host)

Install Certbot if missing. Prefer **webroot** on a shared VPS (avoids Certbot restarting Nginx while diagnosing port conflicts):

```bash
sudo apt-get update
sudo apt-get install -y certbot python3-certbot-nginx

# DNS A records for both hosts must already point at this VPS:
sudo certbot certonly --webroot -w /var/www/certbot \
  -d wealthos.devenlight.com \
  -d api.wealthos.devenlight.com

# Then install HTTPS into the Nginx site (or edit listen 443 blocks manually):
sudo certbot --nginx -d wealthos.devenlight.com -d api.wealthos.devenlight.com
sudo certbot renew --dry-run
```

Certificates stay on the **host** — never mounted into WealthOS containers.

## 8. Health checks

| URL | Expected |
|-----|----------|
| `http://127.0.0.1:8081/health` | API OK (container host port) |
| `https://api.wealthos.devenlight.com/health` | API OK (public) |
| `https://api.wealthos.devenlight.com/api/health` | Alias → `/health` via host Nginx |
| `https://wealthos.devenlight.com/` | SPA |

ASP.NET still maps health at `/health` (no app code change). Host Nginx aliases `/api/health`.

## 9. Restart / update

GitHub Actions (`.github/workflows/deploy-hostinger.yml`) mirrors layaKPI-Tracker:

- Trigger: push to `main` or **Actions → Deploy to VPS → Run workflow**
- Secrets (same as KPI): `VPS_HOST`, `VPS_USER`, `VPS_SSH_KEY`
- Remote path: `~/WealthOS`
- Compose: `docker compose -f docker/docker-compose.prod.yml --env-file docker/.env.production up -d --build`
- Smoke: `127.0.0.1:8081/health` and `127.0.0.1:3000/`

Manual update (same commands as CI):

```bash
cd ~/WealthOS
git fetch --all
git reset --hard origin/main
docker compose -f docker/docker-compose.prod.yml --env-file docker/.env.production up -d --build
docker image prune -f
sudo nginx -t && sudo systemctl reload nginx
```

## 10. Rollback

```bash
cd ~/WealthOS
git log --oneline -5
git reset --hard <previous-good-sha>
docker compose -f docker/docker-compose.prod.yml --env-file docker/.env.production up -d --build
```

Database volume `wealthos_postgres_data` is preserved across rollbacks unless you explicitly `docker volume rm`.

## 11. Migration from the old deployment

| Old | New |
|-----|-----|
| `backend/docker/docker-compose.prod.yml` with **nginx** on 80/443 | `docker/docker-compose.prod.yml` — **no** container nginx, no 80/443 |
| TLS certs in `backend/docker/nginx/certs` | Host Let's Encrypt via Certbot |
| Env in `backend/docker/.env` | `docker/.env.production` |
| Frontend static nginx image | Frontend **node-server** container on host port **3000** |

Migration steps:

1. Stop old stack: `cd ~/WealthOS/backend/docker && docker compose -f docker-compose.prod.yml down` (keeps volume if not `-v`).
2. Copy secrets into `~/WealthOS/docker/.env.production`.
3. Start root compose (section 4).
4. Free Docker from 80/443, install host Nginx for KPI + WealthOS, then Certbot (sections 5–7).
5. Confirm layaKPI / other sites still work via host Nginx.

## Security checklist

- [ ] Strong `JWT_SECRET_KEY` and `POSTGRES_PASSWORD`
- [ ] CORS locked to `https://wealthos.devenlight.com`
- [ ] Docker does **not** publish 80/443
- [ ] Postgres not published to the host
- [ ] TLS only on host Nginx
- [ ] Firewall allows 80/443 (host); 3000/8081 can stay localhost-only if preferred via compose bind `127.0.0.1:3000:3000`
