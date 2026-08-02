# WealthOS Architecture Rules

This document defines the permanent architecture rules for the WealthOS project. All contributors and AI agents must treat these rules as authoritative when designing, implementing, or modifying the system.

---

# Project Overview

- WealthOS is an enterprise-grade personal wealth management platform.
- The product follows a mobile-first architecture.
- Frontend and backend are developed independently and communicate through well-defined contracts.
- WealthOS is an AI-first product with a modular architecture that supports independent evolution of features and services.

---

# Architecture Principles

All design and implementation decisions must align with the following principles:

- **Clean Architecture** — Depend on abstractions; keep domain logic independent of frameworks, UI, and infrastructure.
- **SOLID Principles** — Apply Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion consistently.
- **Separation of Concerns** — Isolate presentation, application, domain, and infrastructure responsibilities.
- **Dependency Injection** — Resolve dependencies through composition roots and DI containers; avoid hard-wired concrete couplings.
- **Domain Driven Design where appropriate** — Use DDD concepts (entities, value objects, aggregates, bounded contexts) when domain complexity warrants them.
- **Feature-first organization** — Structure code around business features and capabilities, not only technical layers.
- **Composition over inheritance** — Prefer composing behaviors from small units over deep inheritance hierarchies.
- **Async-first programming** — Design I/O-bound and cross-service operations as asynchronous by default.
- **API-first development** — Define and agree on API contracts before implementing consumers and producers.

---

# Folder Organization

## Frontend

```
frontend/
├── README.md
├── apps/                 # Deployable client applications (e.g., mobile, web)
├── packages/             # Shared UI kits, utilities, and client libraries
├── features/             # Feature modules organized by business capability
└── assets/               # Static assets and shared visual resources
```

Purpose: host client applications and UI concerns only. No server-side business rules belong here.

## Backend

```
backend/
├── README.md
├── src/                  # Application and domain source
├── tests/                # Automated tests aligned to modules
├── contracts/            # Shared API contracts and DTOs
└── host/                 # Composition root, DI, and runtime hosting
```

Purpose: APIs, domain services, validation, persistence orchestration, and integration logic.

## Database

```
database/
├── README.md
├── schemas/              # Canonical schema definitions
├── migrations/           # Versioned schema changes
├── seeds/                # Reference and bootstrap data
└── scripts/              # Maintenance and operational SQL/scripts
```

Purpose: durable data structure, evolution, and data operations—separate from application code.

## Docker

```
docker/
├── README.md
├── images/               # Dockerfiles and image build context
├── compose/              # Compose stacks for local and shared environments
└── config/               # Container-oriented configuration templates
```

Purpose: packaging, local orchestration, and environment-consistent runtime definitions.

## Documentation

```
docs/
├── README.md
├── architecture/         # System design and technical overviews
├── decisions/            # Architecture Decision Records (ADRs)
├── guides/               # Onboarding and how-to documentation
└── operations/           # Runbooks and operational procedures
```

Purpose: the shared source of truth for design, decisions, and operational knowledge.

## Scripts

```
scripts/
├── README.md
├── setup/                # Environment and developer setup helpers
├── build/                # Build and packaging utilities
└── ops/                  # Operational and maintenance scripts
```

Purpose: automation that supports development and operations without embedding business logic.

## Cursor

```
.cursor/
├── rules/                # Permanent project and architecture rules
├── workflows/            # Reusable multi-step agent workflows
├── prompts/              # Vetted prompt library
└── templates/            # Standard artifact templates
```

Purpose: AI workspace conventions that reinforce this architecture and project standards.

---

# Development Rules

- Never mix UI with business logic.
- Keep business logic inside the backend.
- Shared models must use contracts.
- Use DTOs between layers.
- Keep modules independent.
- Build reusable components.

Additional expectations:

- Frontend may validate for UX, but authoritative validation and rules live in the backend.
- Cross-module communication must go through explicit interfaces or contracts—not private internals.
- Infrastructure concerns (persistence, messaging, external APIs) must not leak into domain models.

---

# Module Structure

Every feature should follow the same structure. Name folders and types after the business capability they represent.

## Example: Property

A Property feature should typically include:

| Artifact | Responsibility |
|----------|----------------|
| **Property Entity** | Core domain model and invariants |
| **DTO** | Data transfer shapes across boundaries |
| **Validator** | Input and rule validation |
| **Service** | Application/domain orchestration |
| **Repository** | Persistence abstraction and data access |
| **Controller** | API entry point / transport adapter |
| **Tests** | Unit, integration, and contract coverage |

Recommended layout pattern:

```
Property/
├── Property.Entity
├── Property.Dto
├── Property.Validator
├── Property.Service
├── Property.Repository
├── Property.Controller
└── Property.Tests
```

New features must mirror this shape unless an ADR documents a justified exception.

---

# Naming Standards

- Use **PascalCase** for classes.
- Use **camelCase** for variables.
- Use meaningful names only.
- Avoid abbreviations.

Additional guidance:

- Prefer complete business terms (`PortfolioAllocation` over `PortAlloc`).
- Interfaces should express capability clearly (`IPropertyRepository`, not `IRepo`).
- Files, folders, and public types should be discoverable from their names alone.

---

# Coding Philosophy

- Readable over clever.
- Simple over complex.
- Maintainability over shortcuts.
- Never duplicate business logic.

When choosing between approaches, prefer the option that is easier to test, easier to change, and easier for the next engineer (or AI agent) to understand.

---

# AI Rules

- Never regenerate an entire project.
- Modify only requested modules.
- Preserve existing architecture.
- Respect project conventions.
- Never overwrite working code without reason.

AI agents must:

- Read this document before making structural changes.
- Prefer incremental, reviewable edits over broad rewrites.
- Ask for clarification when a request conflicts with these rules.
- Leave unrelated modules untouched.

---

This document is the primary architectural authority for WealthOS. Every future implementation must follow these principles.
