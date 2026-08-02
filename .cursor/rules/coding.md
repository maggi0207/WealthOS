# WealthOS Coding Standards

This document defines the permanent coding standard for the WealthOS project. All human and AI-generated code must comply with these rules.

---

# Goal

- Produce enterprise-grade, production-ready code.
- Readable code is more important than clever code.
- Always optimize for maintainability.

---

# General Rules

- Never create duplicate logic.
- Prefer reuse over rewriting.
- Keep methods small.
- Keep classes focused.
- One responsibility per class.
- Avoid deeply nested code.
- Prefer composition over inheritance.
- Remove dead code.
- Avoid magic strings.
- Avoid magic numbers.

---

# Clean Code

- Follow SOLID principles.
- Follow DRY.
- Follow KISS.
- Follow YAGNI.
- Write self-documenting code.
- Use meaningful names.

---

# File Size

- Class maximum: **300 lines**.
- Method maximum: **40 lines**.
- Prefer multiple small files over large monolithic ones.

When a class or method approaches these limits, extract cohesive units with clear names and responsibilities.

---

# Naming

| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `PropertyService` |
| Interfaces | `I` + PascalCase | `IExampleService` |
| Methods | PascalCase | `CalculateNetWorth` |
| Variables | camelCase | `portfolioValue` |
| Constants | UPPER_CASE | `MAX_RETRY_COUNT` |
| Private fields | `_` prefix + camelCase | `_propertyRepository` |

Additional expectations:

- Names must convey intent without requiring comments.
- Avoid abbreviations unless they are universal domain terms.
- Prefer domain language that matches WealthOS features and bounded contexts.

---

# Comments

- Write comments only when explaining business decisions.
- Never explain obvious code.
- Prefer expressive code over comments.

Comments are appropriate for non-obvious trade-offs, regulatory constraints, and intentional deviations documented elsewhere. They are not a substitute for clear naming or structure.

---

# Error Handling

- Never swallow exceptions.
- Log unexpected errors.
- Return meaningful error messages.
- Never expose stack traces.

Errors returned to clients must be safe, actionable, and free of sensitive internals. Unexpected failures must be observable through logging without leaking implementation details.

---

# Async

- Use `async`/`await` everywhere appropriate.
- Never block async code.
- Avoid `Task.Result`.
- Avoid `Wait()`.

Async flows must remain asynchronous end-to-end. Blocking calls invite deadlocks, thread-pool starvation, and unpredictable latency.

---

# Logging

- Use structured logging.
- Never log passwords.
- Never log tokens.
- Never log secrets.
- Log business events.

Prefer key-value structured fields over free-form concatenated strings. Log outcomes that matter to operations and auditing; never log credentials or secrets in any environment.

---

# Validation

- Validate all external input.
- Fail fast.
- Never trust client input.

Treat UI, API, file, and third-party payloads as untrusted. Enforce authoritative validation on the backend even when the client performs UX checks.

---

# Performance

- Avoid unnecessary allocations.
- Avoid unnecessary database calls.
- Avoid loading entire collections.
- Prefer pagination.

Design data access and processing paths for realistic production volumes. Fetch only what is needed, and page large result sets by default.

---

# Security

- Never hardcode secrets.
- Never hardcode passwords.
- Never hardcode API keys.
- Use environment variables.

Secrets belong in secure configuration or secret stores, never in source control, logs, or client bundles.

---

# Testing

- Write testable code.
- Avoid static dependencies.
- Use dependency injection.

Design modules so behavior can be verified in isolation. Prefer injected abstractions over hard-wired static calls that block unit testing.

---

# AI Rules

When generating code:

- Modify only requested files.
- Never regenerate complete modules.
- Preserve formatting.
- Follow existing architecture.
- Follow existing naming conventions.
- Never introduce breaking changes without explanation.

AI agents must make the smallest correct change that satisfies the request, preserve working behavior unless change is explicitly required, and align with `.cursor/rules/architecture.md` and these standards.

---

These coding standards are mandatory for every AI-generated implementation in WealthOS.
