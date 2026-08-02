# WealthOS Docker & Deployment Standards

This document defines the permanent Docker, containerization, and deployment standards for WealthOS. All services must be packaged and deployed according to these rules.

## Technology Stack

| Concern | Choice |
|---------|--------|
| Containers | Docker |
| Orchestration (local/VPS) | Docker Compose |
| Reverse proxy | Nginx |
| Database | PostgreSQL |
| API | ASP.NET Core |
| Web | React (Vite) |

## Deployment Target

**Hostinger VPS running Ubuntu**

---

# Docker Philosophy

- **Every service must be containerized** — No manual runtime installs on the VPS for application tiers.
- **Containers must be stateless** — Persist data in volumes or external stores, not container filesystems.
- **Configuration through environment variables** — No secrets or environment-specific values baked into images.
- **Immutable deployments** — Deploy new images; do not mutate running containers in place.
- **One responsibility per container** — Each container runs a single primary process or role.

---

# Planned Containers

| Container | Purpose |
|-----------|---------|
| **nginx** | Reverse proxy, TLS termination, static asset serving |
| **wealth-web** | React (Vite) production build |
| **wealth-api** | ASP.NET Core Web API |
| **postgres** | PostgreSQL database |
| **hangfire** | Background job processing (may share API image with alternate entrypoint) |
| **pgadmin** | Database admin UI — **development only** |
| **notification-service** | Future: outbound notifications |
| **ai-service** | Future: AI integrations |

Add new services as separate containers with explicit compose definitions and network membership.

---

# Docker Images

- Use official images whenever possible.
- Use multi-stage builds for application images (`wealth-api`, `wealth-web`).
- Keep images as small as possible.
- Avoid unnecessary packages, shells, and build tools in final runtime stages.

Pin image tags or digests for reproducible production builds.

---

# Docker Compose

Separate files for:

| Environment | File pattern |
|-------------|--------------|
| **Development** | `docker-compose.yml`, `docker-compose.dev.yml` (or equivalent) |
| **Production** | `docker-compose.prod.yml` |

Never mix production configuration with development. Development-only services (e.g., **pgadmin**) must not appear in production compose files.

---

# Environment Variables

Never hardcode:

- Passwords
- Secrets
- JWT keys
- Connection strings
- API keys

Use `.env` files for local development. Use secure environment injection on the VPS for production. Provide `.env.example` with keys only—never real values in source control.

---

# Networking

- Use Docker bridge networks.
- Containers communicate by **service name** (e.g., `wealth-api`, `postgres`).
- Avoid exposing unnecessary ports publicly; only **nginx** (and dev tools when required) should face the host edge in production.

Internal services (`postgres`, `hangfire`, future workers) remain on the internal network.

---

# Volumes

Persist:

- **PostgreSQL data**
- **Uploaded documents**
- **Logs** (if required for retention or compliance)

Never store important data inside containers. Use named volumes or bind mounts with documented backup procedures.

---

# Logging

- Log to **stdout/stderr** — containers must not rely on local file logging inside the image.
- Support centralized logging aggregation on the VPS or external stack in production.

Application logging (Serilog, etc.) should forward to stdout for Docker capture.

---

# Health Checks

Every container must expose a health check.

| Service | Check |
|---------|-------|
| **API** | HTTP health endpoint |
| **Database** | `pg_isready` or equivalent |
| **Nginx** | HTTP probe on upstream or `/health` |
| **Background workers** | Process or job heartbeat endpoint |

Compose and deployment scripts should use health status before routing traffic or marking deploys successful.

---

# Security

- Run containers as **non-root** whenever possible.
- Use minimal base images (e.g., Alpine or distroless where appropriate).
- Do not expose internal services publicly.
- Keep images updated; scan for known vulnerabilities in CI or release process.
- Restrict VPS firewall to required ports (typically 80, 443).

---

# Deployment

- Support **zero-downtime deployments** where possible (rolling nginx upstream updates, new API containers before draining old).
- Support **rolling updates** in the future as service count grows.
- Keep deployment **repeatable** — scripted, documented, and version-controlled.

Document deployment steps in `docs/operations/`; compose files in `docker/` are the source of truth for service topology.

---

# Development

Support one-command startup:

```bash
docker compose up -d
```

Support one-command shutdown:

```bash
docker compose down
```

Developers should reach API, web, and database locally without manual service installation. Use override files for local-only settings.

---

# Nginx

- Terminate TLS in production.
- Proxy API traffic to `wealth-api`.
- Serve static assets from `wealth-web` build output.
- Configure timeouts, body size limits, and security headers appropriately.
- Keep nginx config in `docker/` under version control.

---

# AI Rules

When generating Docker files:

- Modify only affected services.
- Never regenerate the entire compose file unnecessarily.
- Preserve existing environment variables.
- Never overwrite production configuration.
- Keep images optimized.

Also align with `.cursor/rules/backend.md`, `.cursor/rules/frontend.md`, and `.cursor/rules/architecture.md`.

---

All WealthOS services must be containerized and deployed according to these Docker standards.
