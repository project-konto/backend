#!/bin/bash
#
# Installation script for pre-receive hook
#
# Usage:
#   ./install.sh /path/to/repository.git
#

set -e

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Check if target path is provided
if [ $# -ne 1 ]; then
    echo -e "${RED}Error:${NC} Please provide the path to the bare repository"
    echo "Usage: $0 /path/to/repository.git"
    exit 1
fi

TARGET_REPO="$1"
HOOKS_DIR="$TARGET_REPO/hooks"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Validate target repository
if [ ! -d "$TARGET_REPO" ]; then
    echo -e "${RED}Error:${NC} Directory '$TARGET_REPO' does not exist"
    exit 1
fi

if [ ! -d "$HOOKS_DIR" ]; then
    echo -e "${RED}Error:${NC} '$TARGET_REPO' does not appear to be a Git repository (missing hooks directory)"
    exit 1
fi

# Check if pre-receive hook already exists
if [ -f "$HOOKS_DIR/pre-receive" ]; then
    echo -e "${YELLOW}Warning:${NC} pre-receive hook already exists at $HOOKS_DIR/pre-receive"
    read -p "Do you want to overwrite it? (y/N) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Installation cancelled."
        exit 0
    fi
    # Backup existing hook
    cp "$HOOKS_DIR/pre-receive" "$HOOKS_DIR/pre-receive.backup.$(date +%Y%m%d_%H%M%S)"
    echo -e "${GREEN}✓${NC} Backed up existing hook"
fi

# Copy the hook
echo "Installing pre-receive hook to $HOOKS_DIR..."
cp "$SCRIPT_DIR/pre-receive" "$HOOKS_DIR/pre-receive"
chmod +x "$HOOKS_DIR/pre-receive"

# Verify installation
if [ -x "$HOOKS_DIR/pre-receive" ]; then
    echo -e "${GREEN}✓${NC} Hook installed successfully!"
    echo ""
    echo "The pre-receive hook is now active and will validate all incoming commits."
    echo "Commits must follow the Conventional Commits format:"
    echo "  <type>(<scope>): <subject>"
    echo ""
    echo "See README.md for more information and examples."
else
    echo -e "${RED}✗${NC} Installation failed - hook is not executable"
    exit 1
fi
