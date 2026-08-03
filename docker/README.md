# docker

Production container orchestration for WealthOS on a **shared** Hostinger VPS.

## Files

| File | Purpose |
|------|---------|
| `docker-compose.prod.yml` | Postgres + API + frontend (ports **8080** / **3000** only) |
| `.env.production.example` | Template → copy to `.env.production` on the VPS |

## Quick start

From repository root:

```bash
cp docker/.env.production.example docker/.env.production
nano docker/.env.production
docker compose -f docker/docker-compose.prod.yml --env-file docker/.env.production up -d --build
```

Validate:

```bash
curl -fsS http://127.0.0.1:3000/
curl -fsS http://127.0.0.1:8080/health
```

Host TLS and public hostnames are configured outside Docker — see [`docs/HOSTINGER_DEPLOYMENT.md`](../docs/HOSTINGER_DEPLOYMENT.md).

## Notes

- Do **not** publish 80 or 443 from Compose.
- Postgres has no host port mapping.
- Dev compose remains under `backend/docker/docker-compose.yml`.
