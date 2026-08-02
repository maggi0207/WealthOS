# WealthOS AI Token Optimization Strategy

This document defines the permanent AI token optimization strategy for WealthOS.

## Goal

Reduce AI token usage, API costs, and unnecessary context while maintaining enterprise-quality code.

---

# Core Principle

Always solve the problem using the smallest possible context and the smallest possible code change.

Never perform expensive operations unless they provide significant value.

---

# Model Selection Strategy

Always choose the cheapest capable model for the task.

## LOW complexity

Examples:

- Create folders
- Create README files
- Markdown
- Documentation
- Rename files
- Format code
- Add comments
- Fix imports
- Update configuration
- Small CRUD changes
- Unit tests
- Simple refactoring

**Model:** Use the fastest / cheapest model.

## MEDIUM complexity

Examples:

- Add API endpoint
- Create Entity
- Create DTO
- Create Service
- Repository implementation
- Database migration
- Docker updates
- React component
- Bug fixes

**Model:** Use a balanced reasoning model.

## HIGH complexity

Examples:

- Architecture
- Security
- Authentication
- Authorization
- Performance optimization
- AI integration
- Large refactoring
- Cross-module changes
- Database redesign
- Distributed systems

**Model:** Use the strongest reasoning model.

---

# File Reading Rules

- Never scan the entire repository.
- Read only files required for the task.
- Prefer targeted context.
- Avoid unnecessary indexing.

---

# Editing Rules

- Never regenerate an entire file when one function changes.
- Never regenerate an entire module when one class changes.
- Modify only affected code.
- Preserve formatting.
- Preserve comments.
- Preserve architecture.

---

# Context Rules

- Reuse previous context.
- Avoid asking the AI to analyze unchanged files.
- Avoid repeatedly loading large files.
- Never include unrelated files.

---

# Code Generation

- Generate only requested files.
- Never scaffold the whole application.
- Implement one feature at a time.
- Avoid duplicate implementations.
- Reuse existing code whenever possible.

---

# Backend

- Generate only one module at a time.
- Never generate every CRUD endpoint unless requested.
- Prefer incremental development.

---

# Frontend

- Never regenerate complete pages.
- Modify individual components.
- Reuse shared components.
- Keep changes isolated.

---

# Documentation

- Update existing documentation.
- Avoid rewriting documents.
- Only change affected sections.

---

# Testing

- Generate tests only for the current feature.
- Avoid generating unnecessary test suites.

---

# Reviews

When reviewing code:

- Focus only on changed files.
- Do not review the entire repository.
- Provide concise feedback.

---

# Performance

- Avoid unnecessary tool calls.
- Avoid repeated searches.
- Avoid duplicate analysis.
- Prefer incremental edits.

---

# Git

- Never modify unrelated files.
- Keep commits focused.
- One feature per commit.

---

# AI Behavior

Before every task:

1. Estimate task complexity.
2. Select the cheapest suitable model.
3. Read only required files.
4. Modify only required code.
5. Produce the smallest correct solution.
6. Summarize changes briefly.
7. Suggest the next logical task.

---

# Absolute Rules

- Never rewrite the entire repository.
- Never regenerate working code.
- Never introduce breaking changes without explanation.

Always optimize for:

1. Correctness
2. Maintainability
3. Token efficiency
4. Development speed

---

This rule is mandatory for every future AI interaction in the WealthOS project.
