# WealthOS Backend Development Standards

This document defines the permanent backend development standards for WealthOS. All backend work—human or AI-generated—must follow these rules.

## Technology Stack

| Concern | Choice |
|---------|--------|
| Runtime | .NET 9 |
| API | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | PostgreSQL |
| Auth | JWT Authentication |
| Validation | FluentValidation |
| Logging | Serilog |
| Background jobs | Hangfire |
| Packaging | Docker |
| API docs | Swagger / OpenAPI |

---

# Backend Philosophy

- **API-first development** — Define contracts before implementation; expose clear, versionable endpoints.
- **Clean Architecture** — Domain and application logic stay independent of frameworks and infrastructure.
- **Feature-based organization** — Group code by business capability, not only by technical layer.
- **Dependency Injection** — Compose dependencies at the host; avoid hard-wired concrete couplings.
- **SOLID principles** — Keep types focused, extensible, and dependent on abstractions.
- **Async-first programming** — Use `async`/`await` for I/O-bound work end-to-end.
- **Testability** — Design for isolated unit and integration tests via abstractions and DI.

---

# Project Structure

Organize the backend into these projects:

| Project | Responsibility |
|---------|----------------|
| **Wealth.Api** | ASP.NET Core host, controllers, middleware, auth wiring, Swagger, DI composition root, and HTTP concerns. |
| **Wealth.Application** | Application services, use cases, orchestration, FluentValidation validators, and application-level mappings. |
| **Wealth.Domain** | Domain entities, value objects, domain rules, and core abstractions (no EF, HTTP, or infrastructure references). |
| **Wealth.Infrastructure** | EF Core, PostgreSQL, repositories, external integrations, Hangfire wiring, and other infrastructure adapters. |
| **Wealth.Contracts** | Shared API contracts, request/response DTOs, and cross-boundary models used by API and consumers. |
| **Wealth.Tests** | Unit, integration, and contract tests aligned to features and layers. |

Dependency direction:

```
Wealth.Api → Wealth.Application → Wealth.Domain
Wealth.Api → Wealth.Infrastructure → Wealth.Domain
Wealth.Application / Wealth.Api → Wealth.Contracts
Wealth.Tests → (projects under test)
```

`Wealth.Domain` must not reference Application, Infrastructure, or Api. Infrastructure depends inward on Domain (and Application abstractions where required), never the reverse.

---

# Feature Structure

Every feature follows the same shape and remains independent of other features.

## Example: Property

```
Property/
├── PropertyController
├── PropertyService
├── PropertyRepository
├── PropertyValidator
├── PropertyEntity
├── PropertyDto
├── PropertyMapping
└── PropertyTests
```

Rules:

- Place feature artifacts in the correct project by responsibility (controller in Api, service/validator/mapping in Application, entity in Domain, repository in Infrastructure, DTOs in Contracts, tests in Tests).
- Do not create cross-feature shortcuts that bypass contracts or shared abstractions.
- New features must mirror this structure unless an ADR documents a justified exception.

---

# Controllers

Controllers must remain thin.

Controllers should only:

- Validate the request (or trigger validation).
- Call an application service.
- Return a response.

Never place business logic inside controllers. No persistence calls, domain calculations, or infrastructure configuration belong in controller actions.

---

# Services

Services contain business rules and use-case orchestration.

- No HTTP logic (`HttpContext`, status-code crafting, header parsing as core flow).
- No database configuration (DbContext setup, connection strings, migrations).
- Only business logic and application workflow.

Services depend on abstractions (repositories, gateways, clocks) via constructor injection.

---

# Repositories

Repositories should only access the database.

- No business rules.
- No HTTP concerns.
- Map persistence models to domain entities at the infrastructure boundary when needed.
- Expose async methods; keep queries focused and intentional.

---

# DTO Rules

- Never expose EF entities directly through the API.
- Always use DTOs for API contracts.
- Separate **Request DTOs** and **Response DTOs**.
- Keep DTO definitions in `Wealth.Contracts` (or an agreed contracts location) so boundaries stay explicit.
- Mapping lives in Application (or dedicated mapping types)—not in controllers or entities.

---

# Validation

- Use FluentValidation for all incoming requests.
- Validate every incoming request.
- Fail fast.
- Return meaningful validation errors.

Authoritative validation runs on the backend even when clients perform UX checks. Prefer validator classes per request type, registered and invoked consistently.

---

# Authentication

- Use JWT authentication.
- Protect all APIs by default.
- Allow anonymous access only where explicitly marked.
- Never hardcode secrets, signing keys, or credentials.

Configure token validation, issuer, audience, and lifetimes through configuration and environment variables—not source code literals.

---

# Entity Framework

- Use Code First.
- Use Fluent API configuration for entity mapping (prefer configurations over attribute-heavy models).
- No business logic inside entities beyond true domain invariants appropriate to the domain model.
- Always use async database operations (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`, etc.).
- Prefer `IQueryable` until execution; materialize deliberately.
- Keep DbContext and EF types inside Infrastructure; Domain must not depend on EF Core.

---

# API Responses

Use a consistent response envelope for all API results.

Include:

- `Success`
- `Message`
- `Data`
- `Errors`

Never return inconsistent response shapes. Controllers and middleware should produce the same envelope for success, validation failures, and handled errors so clients can rely on a stable contract. Document the envelope in OpenAPI/Swagger.

---

# Logging

- Use Serilog.
- Structured logging only (key-value properties, not opaque concatenated strings).
- Never log passwords.
- Never log tokens.
- Never log secrets.
- Log important business events (e.g., portfolio updates, successful auth outcomes without secrets, critical workflow milestones).

Correlate requests where possible (request IDs) to support production diagnostics.

---

# Error Handling

- Use centralized exception middleware.
- Never catch exceptions unless necessary (e.g., translating known domain failures or isolating third-party boundaries).
- Return friendly API errors through the standard response envelope.
- Never expose stack traces or internal exception details to clients.
- Log unexpected failures with enough context for operators—without leaking secrets.

---

# Configuration

- Use `appsettings` only for non-secret defaults.
- Use environment variables (or a secret store) for secrets.
- Never commit secrets, connection strings with credentials, or signing keys.
- Prefer options pattern / strongly typed configuration for runtime settings.

---

# Docker

- Backend must always be Docker-ready.
- No machine-specific paths, drive letters, or developer-only assumptions in runtime config.
- Prefer environment-based configuration for connection strings, JWT settings, and Hangfire storage.
- Ensure the API, dependencies, and migrations strategy work in containerized environments.

---

# Background Jobs

- Use Hangfire for background and scheduled work.
- Keep job handlers thin: enqueue from Application, execute durable work via services/repositories.
- Jobs must be idempotent where retries are possible.
- Do not put business-critical rules only inside Hangfire infrastructure wiring.

---

# API Documentation

- Maintain accurate Swagger/OpenAPI documentation.
- Reflect auth requirements, envelopes, and validation error shapes.
- Keep contract changes intentional and documented.

---

# AI Rules

When generating backend code:

- Modify only the requested module.
- Preserve architecture.
- Follow existing folder structure.
- Avoid duplicate services.
- Never regenerate unrelated files.
- Produce production-ready code.

Also:

- Respect Clean Architecture project boundaries.
- Prefer incremental, reviewable changes.
- Align with `.cursor/rules/architecture.md` and `.cursor/rules/coding.md`.
- Do not introduce breaking API changes without an explicit explanation.

---

All backend implementations in WealthOS must follow these rules.
