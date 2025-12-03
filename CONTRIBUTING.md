# Contributing Guide

## Setup

After cloning the repository, install the commit linting tools:

```bash
npm install
```

This will install Husky git hooks that automatically enforce Conventional Commits on every commit

## Workflow

1. Pick a task from [Kanban Board](https://github.com/orgs/project-konto/projects/1/views/2)
2. Create a new branch from `development` (name it `feature/<FEATURE-NAME>` or `bug/<BUG-NAME>`)
3. Commit your changes using Conventional Commits format:
    - Format: `<type>(<scope>): <subject> #<issue>`
    - Example: `feat(api): add transaction import #23`
    - Allowed types: `feat`, `fix`, `docs`, `chore`, `refactor`, `test`, `perf`, `build`, `ci`, `revert`, `style`
4. Push your branch and open a Pull Request against `development`
5. Wait for workflows and code review and make any requested changes
6. Sync with `development`

## Commit Message Format

All commits must follow the [Conventional Commits](https://www.conventionalcommits.org/) specification. The commit message hook will prevent invalid commits locally, and CI will verify all commits in pull requests
