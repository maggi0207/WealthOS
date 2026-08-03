# Hostinger VPS deployment — WealthOS (shared VPS)

WealthOS coexists with other sites (e.g. layaKPI-Tracker) on the same Hostinger VPS.
**Host Nginx** owns ports **80/443**. Docker publishes only:

| Service  | Host port | Container |
|----------|-----------|-----------|
| Frontend | `3000`    | nginx:80  |
| API      | `8080`    | aspnet:8080 |
| Postgres | *(none)*  | internal  |

```
Internet
  → Host Nginx (80/443 + Let's Encrypt)
      → wealthos.devenlight.com     → 127.0.0.1:3000  (frontend)
      → api.wealthos.devenlight.com → 127.0.0.1:8080  (API)
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
curl -fsS http://127.0.0.1:8080/health    # API health
```

## 5. Configure host Nginx

```bash
sudo mkdir -p /var/www/certbot
sudo cp ~/WealthOS/docs/nginx-hostinger.conf /etc/nginx/sites-available/wealthos
sudo ln -sf /etc/nginx/sites-available/wealthos /etc/nginx/sites-enabled/wealthos
sudo nginx -t
sudo systemctl reload nginx
```

The sample config is in [`docs/nginx-hostinger.conf`](./nginx-hostinger.conf).

## 6. Configure SSL (Let's Encrypt on the host)

Certificates stay on the **host** — never mounted into WealthOS containers.

```bash
sudo apt-get update
sudo apt-get install -y certbot python3-certbot-nginx

# After DNS propagates and HTTP vhosts exist:
sudo certbot --nginx -d wealthos.devenlight.com -d api.wealthos.devenlight.com
sudo certbot renew --dry-run
```

Certbot will adjust the site file SSL paths. Renewals are handled by the host timer.

## 7. Health checks

| URL | Expected |
|-----|----------|
| `http://127.0.0.1:8080/health` | API OK (container) |
| `https://api.wealthos.devenlight.com/health` | API OK (public) |
| `https://api.wealthos.devenlight.com/api/health` | Alias → `/health` via host Nginx |
| `https://wealthos.devenlight.com/` | SPA |

ASP.NET still maps health at `/health` (no app code change). Host Nginx aliases `/api/health`.

## 8. Restart / update

```bash
cd ~/WealthOS
git fetch --all
git reset --hard origin/main
docker compose -f docker/docker-compose.prod.yml --env-file docker/.env.production up -d --build
docker image prune -f
sudo nginx -t && sudo systemctl reload nginx
```

GitHub Actions (`deploy-hostinger.yml`) runs the same compose path via SSH (`VPS_HOST`, `VPS_USER`, `VPS_SSH_KEY`).

## 9. Rollback

```bash
cd ~/WealthOS
git log --oneline -5
git reset --hard <previous-good-sha>
docker compose -f docker/docker-compose.prod.yml --env-file docker/.env.production up -d --build
```

Database volume `wealthos_postgres_data` is preserved across rollbacks unless you explicitly `docker volume rm`.

## 10. Migration from the old deployment

| Old | New |
|-----|-----|
| `backend/docker/docker-compose.prod.yml` with **nginx** on 80/443 | `docker/docker-compose.prod.yml` — **no** container nginx, no 80/443 |
| TLS certs in `backend/docker/nginx/certs` | Host Let's Encrypt via Certbot |
| Env in `backend/docker/.env` | `docker/.env.production` |
| Frontend static host / separate deploy | Frontend **container** on host port **3000** |

Migration steps:

1. Stop old stack: `cd ~/WealthOS/backend/docker && docker compose -f docker-compose.prod.yml down` (keeps volume if not `-v`).
2. Copy secrets into `~/WealthOS/docker/.env.production`.
3. Start root compose (section 4).
4. Install host Nginx site + Certbot (sections 5–6).
5. Confirm layaKPI / other sites on 80/443 still work.

## Security checklist

- [ ] Strong `JWT_SECRET_KEY` and `POSTGRES_PASSWORD`
- [ ] CORS locked to `https://wealthos.devenlight.com`
- [ ] Docker does **not** publish 80/443
- [ ] Postgres not published to the host
- [ ] TLS only on host Nginx
- [ ] Firewall allows 80/443 (host); 3000/8080 can stay localhost-only if preferred via compose bind `127.0.0.1:3000:80`
