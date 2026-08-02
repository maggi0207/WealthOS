# WealthOS API Design Standards

This document defines the permanent API design standards for WealthOS. All API work—human or AI-generated—must follow these rules.

## Technology Context

| Concern | Choice |
|---------|--------|
| Framework | ASP.NET Core Web API |
| Style | REST |
| Auth | JWT Authentication |
| Docs | Swagger / OpenAPI |
| Data store | PostgreSQL |

---

# API Philosophy

- **API-first development** — Define and agree on contracts before implementing consumers and producers.
- **RESTful design** — Model resources and use HTTP semantics correctly.
- **Predictable endpoints** — Clients should infer behavior from naming and verbs alone.
- **Consistent naming** — Use the same conventions across all modules.
- **Backward compatibility** — Prefer additive changes; avoid breaking existing clients without a versioned path.
- **Version-ready architecture** — Design routes, contracts, and hosts so versioning can be introduced without redesign.

---

# Endpoint Naming

Use plural resource names.

Examples:

```
/api/properties
/api/assets
/api/loans
/api/investments
/api/documents
```

Avoid verbs in endpoint names. Express actions with HTTP methods.

**Good**

```
GET    /api/properties
POST   /api/properties
PUT    /api/properties/{id}
DELETE /api/properties/{id}
```

**Bad**

```
/createProperty
/updateLoan
/getAllAssets
```

Nested resources are allowed when the relationship is clear and ownership is strong (for example, `/api/properties/{id}/documents`). Prefer flat resources when nesting adds noise without clarity.

---

# HTTP Methods

| Method | Purpose |
|--------|---------|
| **GET** | Retrieve data |
| **POST** | Create |
| **PUT** | Replace |
| **PATCH** | Partial update |
| **DELETE** | Delete |

Never misuse HTTP verbs. Do not use `GET` for mutations, or `POST` for simple reads when a query resource is appropriate.

---

# API Versioning

Prepare APIs for versioning.

Example:

```
/api/v1/properties
```

Although `v1` may be omitted initially, the architecture must support versioning (route templates, contract packages, and Swagger documents). Breaking changes require a new version or an explicit migration plan.

---

# Response Format

Every API response must use the same envelope:

```json
{
  "success": true,
  "message": "",
  "data": {},
  "errors": []
}
```

Rules:

- `success` indicates overall outcome.
- `message` carries a human-readable summary when useful.
- `data` holds the payload (object, array, or `null`).
- `errors` holds structured error details (empty on success).

Never return inconsistent response formats across endpoints or error paths.

---

# Error Responses

Use standard HTTP status codes.

| Status | Meaning |
|--------|---------|
| **200 OK** | Successful read or update |
| **201 Created** | Successful create |
| **204 No Content** | Successful delete or empty success body when envelope policy allows |
| **400 Bad Request** | Malformed request |
| **401 Unauthorized** | Missing or invalid authentication |
| **403 Forbidden** | Authenticated but not permitted |
| **404 Not Found** | Resource does not exist |
| **409 Conflict** | State conflict (duplicates, concurrency) |
| **422 Validation Error** | Semantic / validation failures |
| **500 Internal Server Error** | Unexpected server failure |

Return meaningful error messages inside the standard envelope. Never expose internal exceptions, stack traces, SQL, or infrastructure details to clients.

---

# Validation

- Validate every request.
- Use FluentValidation.
- Return validation details in a consistent format within `errors`.
- Fail fast; do not partially apply invalid writes.

Client-side checks do not replace authoritative server validation.

---

# Pagination

Large collections must support pagination.

Standard query parameters:

| Parameter | Purpose |
|-----------|---------|
| `page` | Page number |
| `pageSize` | Items per page |
| `sort` | Sort field / direction |
| `search` | Free-text search |

Never return extremely large collections unbounded. Paginated list responses should include enough metadata in `data` (or an agreed paging object) for clients to navigate results.

---

# Filtering

Support filtering using query parameters.

Examples:

```
?status=Active
?search=gold
?sort=name
```

Avoid creating multiple endpoints for filtering variants. Prefer one list endpoint with composable query parameters.

---

# Authentication

- JWT-protected by default.
- Anonymous access only when explicitly allowed.
- Document auth requirements in Swagger for every endpoint.

---

# Authorization

- Use role-based authorization.
- Prefer policies over hardcoded role checks in controllers.
- Enforce authorization in the API layer and reinforce with application rules where needed.

---

# Documentation

Every endpoint must appear in Swagger / OpenAPI and include:

- Summary
- Description
- Request example
- Response example
- Error responses

Keep docs synchronized with contract and envelope changes.

---

# DTO Rules

- Never expose database entities through the API.
- Always use **Request DTOs** and **Response DTOs**.
- Keep API contracts stable and explicit; map at the application boundary.

---

# Performance

- Support async endpoints.
- Support cancellation tokens.
- Avoid N+1 queries.
- Avoid unnecessary payloads (select fields deliberately; do not over-fetch).
- Prefer pagination and filtering over full-table responses.

---

# Security

- Validate input.
- Sanitize output where necessary.
- Never expose secrets, tokens, or credentials in responses or docs examples.
- Never trust client data—including IDs, roles, and ownership claims—without server-side verification.

---

# AI Rules

When generating APIs:

- Follow the existing response format.
- Preserve endpoint consistency.
- Modify only requested endpoints.
- Do not regenerate unrelated controllers.
- Keep controllers thin.
- Delegate business logic to services.

Also align with `.cursor/rules/backend.md`, `.cursor/rules/architecture.md`, and `.cursor/rules/coding.md`.

---

All WealthOS APIs must follow these standards to ensure consistency, maintainability, and long-term scalability.
