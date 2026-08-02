# WealthOS Frontend Development Standards

This document defines the permanent frontend development standards for WealthOS. All frontend work—human or AI-generated—must follow these rules.

## Frontend Technology

| Concern | Choice |
|---------|--------|
| UI library | React 19 |
| Language | TypeScript |
| Build tool | Vite |
| Component library | Material UI |
| Routing | React Router |
| Server state | TanStack Query |
| Forms | React Hook Form |
| Validation | Zod |
| Animation | Framer Motion |
| Charts | Recharts |

---

# Frontend Philosophy

- **Mobile-first development** — Design and implement for small screens first, then scale up.
- **Responsive by default** — Layouts and typography must adapt across breakpoints without separate one-off pages.
- **Component-driven architecture** — Build UI from small, composable, reusable pieces.
- **Accessibility first** — Usable by keyboard, screen readers, and assistive technologies from the start.
- **Performance first** — Lazy load, split code, and avoid unnecessary work on the main thread.
- **Reusable components** — Prefer shared primitives over duplicated UI patterns.
- **API-driven UI** — Render from server contracts; do not embed business rules in presentation.

---

# Folder Structure

Use feature-based organization.

```
src/
├── app/              # App shell, providers, router setup
├── components/       # Shared, reusable UI primitives
├── features/         # Feature modules (own components, hooks, types)
├── hooks/            # Shared custom hooks
├── layouts/          # Page and section layouts
├── pages/            # Route-level page components
├── services/         # API clients and data access
├── theme/            # MUI theme, tokens, typography
├── types/            # Shared TypeScript types
├── utils/            # Pure helpers
└── assets/           # Static images, icons, fonts
```

Each feature should own its components, hooks, and feature-specific types under `features/<FeatureName>/`. Shared UI belongs in `components/`, not duplicated inside features.

---

# Components

Components should:

- Be reusable
- Have one responsibility
- Avoid business logic
- Receive data through props
- Be strongly typed

Never create large monolithic components. Extract subcomponents when a file grows beyond a single clear concern. Presentational components receive data and callbacks; orchestration lives in pages, hooks, or services.

---

# State Management

| Concern | Approach |
|---------|----------|
| **Local state** | React state (`useState`, `useReducer`) |
| **Server state** | TanStack Query |
| **Forms** | React Hook Form |
| **Validation** | Zod |

Avoid unnecessary global state. Prefer colocated state and server cache over global stores unless multiple distant trees genuinely need shared mutable client state.

---

# API Calls

- Never call APIs directly inside UI components.
- Always use service files (`services/`).
- Always use TanStack Query for fetching, caching, invalidation, and loading/error states.

Components consume hooks that wrap TanStack Query; services encapsulate HTTP details and endpoint shapes.

---

# Styling

- Use Material UI.
- Follow design tokens defined in `theme/`.
- Maintain consistent spacing.
- **8-point spacing system** — use multiples of 8px for layout rhythm.
- **16px page gutters** — standard horizontal padding on mobile and up unless a layout explicitly differs.
- **Responsive typography** — scale type across breakpoints via theme.
- **Support dark mode** — theme must support light and dark palettes consistently.

Avoid inline style sprawl; prefer `sx`, theme overrides, and shared styled patterns.

---

# Mobile

Mobile-first by default.

Support:

| Breakpoint | Target |
|------------|--------|
| **360px** | Small phones |
| **390px** | Common phone width |
| **430px** | Large phones |
| **Tablet** | Medium layouts |
| **Desktop** | Full layouts |

- No horizontal scrolling on primary content.
- Touch targets minimum **44px**.

Test layouts at these widths before considering a screen complete.

---

# Routing

- Use React Router.
- Lazy load pages (`React.lazy` + `Suspense`).
- Protect authenticated routes with guards or layout wrappers.
- Keep route definitions centralized in `app/` or a dedicated router module.

---

# Performance

- Lazy loading for routes and heavy feature bundles.
- Memoization where appropriate (`memo`, `useMemo`, `useCallback`)—not by default on every component.
- Avoid unnecessary re-renders; lift state only when needed.
- Code splitting at route and large feature boundaries.
- Image optimization (appropriate formats, sizing, lazy loading).

---

# Accessibility

- Semantic HTML (`button`, `nav`, `main`, `heading` hierarchy).
- Keyboard navigation for all interactive flows.
- ARIA labels where necessary—not as a substitute for semantic markup.
- Sufficient color contrast per WCAG guidance.

Accessibility is required, not optional polish.

---

# Error Handling

- Use reusable error components for consistent failure UI.
- Use loading skeletons for async content—not blank screens or spinners everywhere.
- Gracefully handle API failures with clear, actionable messages.
- Surface validation errors inline on forms.

Never fail silently; never expose raw stack traces or internal errors to users.

---

# Forms

- React Hook Form for form state and submission.
- Zod for schema validation.
- Never duplicate validation logic—define schemas once and reuse on client; authoritative rules remain on the backend.

Map API validation errors into form field errors when the server returns structured failures.

---

# AI Rules

When generating frontend code:

- Modify only affected components.
- Never regenerate complete pages.
- Reuse existing components.
- Maintain existing design language.
- Preserve responsiveness.
- Avoid duplicate UI.

Also align with `.cursor/rules/architecture.md`, `.cursor/rules/coding.md`, `.cursor/rules/api.md`, and `.cursor/rules/token-optimization.md`.

---

All WealthOS frontend implementations must follow these standards to ensure a consistent, responsive, and maintainable user experience.
