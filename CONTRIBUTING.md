# Contributing Guide

1. Pick a task from [Kanban Board](https://github.com/orgs/project-konto/projects/1/views/2)
2. Create a new branch from `development` (name it `feature/<FEATURE-NAME>` or `bug/<BUG-NAME>`)
3. Commit your changes (use Conventional Commits format, e.g. `feat(module): add new feature`)
   - See [Commit Message Validation](.github/hooks/README.md) for detailed format requirements
4. Push your branch and open a Pull Request against `development` (link the task in the PR description: `Closes #<ISSUE-NUMBER>`)
5. Wait for workflows and code review and make any requested changes
6. Sync with `development`

## Commit Message Format

This repository enforces Conventional Commits format using a server-side git hook. All commit messages must follow the format:

```
<type>(<scope>): <subject>
```

For detailed information about commit message requirements, valid types, and examples, see the [Git Hooks Documentation](.github/hooks/README.md).