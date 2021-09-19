#!/usr/bin/env bash

set -euo pipefail

if [[ ! -d ".git" ]]; then echo "refusing to run outside of repo root directory: $(pwd)"; fi

echo "===================================================="
echo "starting build and test"
echo "===================================================="

dotnet build

dotnet test --no-build

