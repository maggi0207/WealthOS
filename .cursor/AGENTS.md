# WealthOS — AI Agent Instructions

This file is the master instruction manual for every AI working on the WealthOS project. Read it before any task.

---

# Project

**WealthOS** — Enterprise-grade personal wealth management platform.

## Technology Stack

### Frontend

- React 19
- TypeScript
- Vite
- Material UI
- TanStack Query
- React Hook Form
- Zod

### Backend

- .NET 9
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- FluentValidation
- Serilog
- JWT
- Swagger
- Hangfire

### Infrastructure

- Docker
- Docker Compose
- Nginx
- Hostinger VPS

### AI

- OpenAI
- MCP
- Tool Calling

---

# AI Role

You are a senior software engineer working inside an existing enterprise codebase.

Your primary goal is to **preserve architecture** and **improve maintainability**.

- Never behave like a code generator.
- Always behave like an experienced team member.

---

# Before Every Task

1. Understand the request.
2. Estimate complexity.
3. Read only required files.
4. Follow all rules inside `.cursor/rules/`.
5. Produce the smallest correct solution.
6. Preserve existing architecture.

### Rule documents (mandatory)

| Document | Scope |
|----------|--------|
| `architecture.md` | System design and module structure |
| `coding.md` | General coding standards |
| `backend.md` | .NET / ASP.NET Core standards |
| `api.md` | REST API design |
| `database.md` | PostgreSQL / EF Core |
| `frontend.md` | React / TypeScript UI |
| `docker.md` | Containers and deployment |
| `testing.md` | Test strategy |
| `token-optimization.md` | Context and cost efficiency |

---

# Never

- Never regenerate the whole project.
- Never rewrite working modules.
- Never introduce duplicate code.
- Never change unrelated files.
- Never break architecture.
- Never create shortcuts.
- Never ignore coding standards.
- Never hardcode secrets.

---

# Always

- Reuse existing code.
- Keep code readable.
- Keep modules independent.
- Write production-quality code.
- Prefer maintainability.
- Prefer incremental development.
- Document important decisions.

---

# Project Workflow

```
Architecture
    ↓
Database
    ↓
API
    ↓
Backend
    ↓
Frontend Integration
    ↓
Testing
    ↓
Deployment
```

Never skip steps.

---

# AI Development Strategy

- Implement **one feature at a time**.
- **One module.**
- **One commit.**
- **One pull request.**

Small changes are preferred over massive rewrites.

---

# Review Checklist

Before completing a task, verify:

- [ ] Architecture preserved
- [ ] Rules followed
- [ ] No duplicate code
- [ ] No breaking changes
- [ ] No unnecessary files
- [ ] Documentation updated if needed
- [ ] Build should remain successful

---

# Human Approval

Ask for approval before:

- Changing architecture
- Adding dependencies
- Deleting files
- Renaming folders
- Large refactoring
- Framework upgrades
- Database redesign

---

AGENTS.md is the highest priority instruction document in WealthOS. Every AI assistant must follow it before generating code.

---

## Cursor Cloud specific instructions

WealthOS is a monorepo with a **TanStack Start / React frontend** (`frontend/`) and a **.NET 9 API** (`backend/`). The frontend currently uses **mock auth and mock data**; the backend is a separate PostgreSQL-backed API.

### System dependencies (VM image)

These are not installed by the update script:

- **.NET 9 SDK** — pinned in `backend/global.json` (9.0.100+)
- **Bun** — preferred package manager for `frontend/` (`bun.lock`)
- **Docker** — required for PostgreSQL and integration tests (Testcontainers)

Ensure `DOTNET_ROOT` and Bun are on `PATH` (e.g. `$HOME/.dotnet` and `$HOME/.bun/bin`).

### Docker daemon

Docker does not auto-start in Cloud Agent VMs. Before `docker compose` or integration tests:

```bash
sudo dockerd > /tmp/dockerd.log 2>&1 &
sleep 3
sudo chmod 666 /var/run/docker.sock   # or use sudo docker / docker group
```

### Starting services (manual, each session)

**PostgreSQL** (required for the API):

```bash
sudo docker compose -f backend/docker/docker-compose.yml up -d postgres
```

**API** (http://localhost:5095):

```bash
cd backend && dotnet run --project src/WealthOS.Api
```

Dev seed admin: `admin@wealthos.local` / `Admin@WealthOS1!`. Health: `GET http://localhost:5095/health`.

**Frontend** (http://localhost:5173):

```bash
cd frontend && bun run dev -- --host 0.0.0.0 --port 5173
```

Mock login accepts any email/password (e.g. `magesh@wealthos.app`).

### Lint / test / build

| Area | Command | Notes |
|------|---------|-------|
| Frontend lint | `cd frontend && bun run lint` | Many existing Prettier formatting findings |
| Frontend build | `cd frontend && bun run build` | |
| Backend build | `cd backend && dotnet build WealthOS.slnx` | |
| Unit tests | `cd backend && dotnet test tests/WealthOS.UnitTests` | No Docker required |
| Integration tests | `cd backend && dotnet test tests/WealthOS.IntegrationTests` | Requires Docker daemon + socket access |

See `backend/README.md` for API and auth endpoint details.
