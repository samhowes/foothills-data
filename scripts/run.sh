#!/usr/bin/env bash

set -euo pipefail

if [[ ! -d ".git" ]]; then echo "refusing to run outside of repo root directory: $(pwd)"; fi

echo "===================================================="
echo "starting sync"
echo "===================================================="

pushd Orbit/Sync

dotnet run --no-build -- "$1"

popd

summary="$(cat summary.txt)"

# https://docs.github.com/en/actions/learn-github-actions/workflow-commands-for-github-actions
echo "SYNC_SUMMARY<<EOF" >> $GITHUB_ENV
echo "$summary" >> $GITHUB_ENV
echo "EOF" >> $GITHUB_ENV

