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
