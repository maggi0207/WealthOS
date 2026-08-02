# WealthOS Testing Strategy

This document defines the permanent testing strategy for WealthOS. All tests—human or AI-generated—must follow these rules.

## Technology

| Layer | Tools |
|-------|-------|
| Backend unit/integration | xUnit, FluentAssertions |
| Database integration | Testcontainers |
| E2E (future) | Playwright |
| Frontend | React Testing Library |

---

# Testing Philosophy

- **Test business logic first** — Prioritize rules that protect correctness and revenue-critical flows.
- **Avoid testing framework internals** — Test behavior and outcomes, not implementation details of ASP.NET Core, React, or EF Core.
- **Fast tests** — Unit tests run in milliseconds; keep slow tests isolated and intentional.
- **Deterministic tests** — Same input, same result, every run.
- **Repeatable tests** — No order dependency, shared mutable state, or flaky timing.

Tests are documentation of expected behavior. They must be easy to read and trust.

---

# Backend Tests

## Unit Tests

- Target application services, validators, domain logic, and mappers.
- No database, HTTP, or filesystem unless the unit under test truly requires it.
- Use FluentAssertions for readable assertions.

## Integration Tests

- Verify wiring across layers (e.g., service + repository with real EF configuration).
- Use Testcontainers for PostgreSQL when persistence behavior matters.
- Clean up or isolate data per test run.

## API Tests

- Exercise controllers through the HTTP pipeline (WebApplicationFactory or equivalent).
- Assert status codes, response envelope shape, and contract fields.
- Cover auth-required and anonymous endpoints explicitly.

## Database Tests

- Validate migrations, constraints, and query behavior against a real PostgreSQL instance via Testcontainers.
- Do not rely on in-memory providers for schema or SQL-specific behavior.

---

# Frontend Tests

## Component Tests

- Use React Testing Library.
- Test what users see and do—not component state internals.
- Prefer role- and label-based queries.

## Hook Tests

- Test custom hooks in isolation with `renderHook`.
- Cover loading, success, and error states for data hooks.

## Page Tests

- Verify route-level composition, critical user flows, and integration with mocked services.
- Keep page tests focused; avoid duplicating every component test at page level.

## Accessibility Tests

- Include basic a11y checks (roles, labels, keyboard focus) in component and page tests where practical.
- Expand with dedicated a11y tooling as the suite matures.

---

# Test Naming

Use:

```
MethodName_ShouldExpectedBehavior_WhenCondition
```

Example:

```
CreateProperty_ShouldReturnSuccess_WhenDataIsValid
```

Names must describe behavior without reading the test body.

---

# Mocking

- Mock only **external dependencies** (HTTP clients, message buses, third-party APIs, clock).
- Avoid mocking business logic under test.
- Prefer fakes or in-memory implementations over brittle mocks when behavior matters.

---

# Coverage

- Focus on **critical business rules** and high-risk paths.
- Do not chase 100% coverage.
- **Quality over quantity** — one meaningful test beats ten shallow assertions.

Coverage metrics may inform gaps; they do not define success.

---

# Test Organization

Align tests with features and layers:

```
Wealth.Tests/
├── Unit/
├── Integration/
├── Api/
└── Database/

frontend/
└── src/
    └── features/<Feature>/
        └── __tests__/
```

Mirror production structure so tests are discoverable next to the code they protect.

---

# CI Expectations

- All tests must pass before merge.
- Flaky tests are treated as defects—fix or quarantine with a tracked issue.
- Keep the default suite fast enough for frequent local and CI runs.

---

# AI Rules

When generating tests:

- Generate tests only for changed features.
- Never regenerate the complete test suite.
- Keep tests readable.
- Keep tests independent.

Also align with `.cursor/rules/coding.md`, `.cursor/rules/backend.md`, `.cursor/rules/frontend.md`, and `.cursor/rules/token-optimization.md`.

---

Every WealthOS feature should be testable, maintainable, and independently verifiable.
