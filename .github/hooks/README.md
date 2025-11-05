# Git Hooks for Commit Message Validation

This directory contains server-side Git hooks that enforce code quality and consistency standards for the repository.

## Pre-Receive Hook

The `pre-receive` hook validates commit messages against the [Conventional Commits](https://www.conventionalcommits.org/) format before allowing pushes to the server.

### What is Conventional Commits?

Conventional Commits is a specification for adding human and machine-readable meaning to commit messages. It provides an easy set of rules for creating an explicit commit history, which makes it easier to write automated tools on top of.

### Installation

#### Quick Installation (Recommended)

Use the provided installation script:

```bash
cd .github/hooks
./install.sh /path/to/repository.git
```

The script will:
- Verify the target repository exists
- Backup any existing pre-receive hook
- Copy and configure the hook
- Set proper permissions

#### Manual Installation

1. Copy the hook to your bare repository's hooks directory:
   ```bash
   cp .github/hooks/pre-receive /path/to/repository.git/hooks/pre-receive
   chmod +x /path/to/repository.git/hooks/pre-receive
   ```

2. The hook will now run automatically for all pushes to the repository.

#### For Local Testing

To test the hook locally before deploying to a server:

1. Create a test bare repository:
   ```bash
   git init --bare /tmp/test-repo.git
   ```

2. Copy the hook:
   ```bash
   cp .github/hooks/pre-receive /tmp/test-repo.git/hooks/pre-receive
   chmod +x /tmp/test-repo.git/hooks/pre-receive
   ```

3. Add the test repository as a remote and push:
   ```bash
   git remote add test-server /tmp/test-repo.git
   git push test-server your-branch
   ```

### Commit Message Format

All commit messages must follow this format:

```
<type>(<scope>): <subject>

[optional body]

[optional footer(s)]
```

#### Components

- **type** (required): The type of change being made
- **scope** (optional): The area of the codebase affected, in parentheses
- **subject** (required): A brief description of the change

#### Valid Types

| Type | Description |
|------|-------------|
| `feat` | A new feature |
| `fix` | A bug fix |
| `docs` | Documentation only changes |
| `style` | Changes that don't affect code meaning (formatting, whitespace, etc.) |
| `refactor` | Code change that neither fixes a bug nor adds a feature |
| `perf` | Performance improvement |
| `test` | Adding or updating tests |
| `build` | Changes to build system or dependencies |
| `ci` | Changes to CI configuration files and scripts |
| `chore` | Other changes that don't modify src or test files |
| `revert` | Reverts a previous commit |

#### Format Rules

1. **Type**: Must be lowercase and one of the valid types listed above
2. **Scope**: Optional, must be lowercase alphanumeric with hyphens, enclosed in parentheses
3. **Colon and Space**: Required after type/scope (`: `)
4. **Subject**: 
   - Required
   - Should start with a lowercase letter (after the `: `)
   - Should NOT end with a period
   - Should be a brief, imperative description of the change

### Examples

#### ✅ Valid Commit Messages

```
feat: add user authentication
fix(api): resolve null pointer exception
docs: update README with installation steps
style(formatting): apply consistent indentation
refactor(database): optimize query performance
perf(cache): improve response time with Redis
test: add unit tests for user service
build(deps): update dependencies to latest versions
ci: add automated deployment workflow
chore: update .gitignore
revert: revert previous commit
feat(auth): implement OAuth2 integration
fix(parser): handle edge case with empty input
```

#### ❌ Invalid Commit Messages

```
added new feature
# Missing type prefix

FIX: bug fix
# Type must be lowercase

feat:missing space after colon
# Missing space after colon

feat: Add feature.
# Subject ends with period (not allowed)

feat: Add Feature
# Subject starts with uppercase letter (should be lowercase)

invalid: commit message
# Invalid type (not in the allowed list)

feat(API): add endpoint
# Scope must be lowercase

fix : space before colon
# No space allowed before colon

feat(): empty scope
# Scope should not be empty if included
```

### Error Messages

When a commit message is invalid, the hook will:

1. **Reject the entire push** (all commits must be valid)
2. **Show which commit(s) failed validation** with the commit SHA
3. **Display the invalid commit message**
4. **Explain what's wrong** with specific error details
5. **Provide examples** of valid and invalid formats

Example error output:
```
INFO: Validating commit messages against Conventional Commits format...

ERROR: Invalid commit message format in commit a1b2c3d
Commit message:
Added new feature

ERROR:   → Missing commit type

ERROR: Push rejected: One or more commits have invalid commit messages

Valid commit message examples:
  feat: add user authentication
  fix(api): resolve null pointer exception
  ...
```

### Fixing Invalid Commits

If your push is rejected due to invalid commit messages:

#### Option 1: Amend the Last Commit

If only your most recent commit is invalid:
```bash
git commit --amend -m "feat: add new feature"
git push
```

#### Option 2: Interactive Rebase

If multiple commits need fixing:
```bash
# Rebase last N commits (replace N with the number of commits)
git rebase -i HEAD~N

# Mark commits as 'reword' in the editor
# Update commit messages in the subsequent editors
# Force push if already pushed to remote
git push --force-with-lease
```

#### Option 3: Reset and Recommit

For local changes not yet pushed:
```bash
# Soft reset to keep changes
git reset --soft HEAD~N

# Create new commit with proper message
git commit -m "feat: add new feature"
```

### Troubleshooting

#### Hook Doesn't Run

- Ensure the hook file is executable: `chmod +x hooks/pre-receive`
- Verify the hook is in the correct location: `/path/to/repo.git/hooks/pre-receive`
- Check that the shebang line is correct: `#!/bin/bash`

#### Hook Fails Unexpectedly

- Test the hook manually with a sample input:
  ```bash
  echo "old-sha new-sha refs/heads/main" | ./hooks/pre-receive
  ```
- Check Git version compatibility (requires Git 1.7.10+)
- Review server logs for error messages

#### Need to Bypass the Hook (Emergency)

Server administrators can temporarily disable the hook:
```bash
# Rename the hook to disable it
mv hooks/pre-receive hooks/pre-receive.disabled

# Re-enable later
mv hooks/pre-receive.disabled hooks/pre-receive
```

**Note**: Bypassing the hook should only be done in emergencies and with proper authorization.

### Best Practices

1. **Write clear, descriptive subjects**: The subject should clearly describe what the commit does
2. **Use appropriate types**: Choose the type that best describes your change
3. **Add scope when helpful**: Use scope to indicate which part of the codebase is affected
4. **Keep subjects concise**: Aim for 50 characters or less
5. **Use imperative mood**: "add feature" not "added feature" or "adds feature"
6. **Add body for complex changes**: Use the optional body to provide more context

### Additional Resources

- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- [How to Write a Git Commit Message](https://chris.beams.io/posts/git-commit/)
- [Contributing Guide](../../CONTRIBUTING.md)

### Support

If you encounter issues with the commit message validation hook:

1. Review the error messages carefully - they provide specific guidance
2. Check this documentation for examples and troubleshooting
3. Consult the [CONTRIBUTING.md](../../CONTRIBUTING.md) guide
4. Contact the repository maintainers for assistance
