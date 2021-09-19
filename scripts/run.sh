#!/usr/bin/env bash

set -euo pipefail

if [[ ! -d ".git" ]]; then echo "refusing to run outside of repo root directory: $(pwd)"; fi

echo "===================================================="
echo "starting sync"
echo "===================================================="

pushd Orbit/Sync

dotnet run --no-build -- "$1"
