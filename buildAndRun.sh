#!/usr/bin/env bash

set -euo pipefail

pushd Orbit/Sync

echo "starting dotnet build..."
dotnet build

echo "===================================================="
echo "starting sync"
echo "===================================================="

dotnet run --no-build
