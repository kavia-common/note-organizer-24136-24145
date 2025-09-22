#!/usr/bin/env bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
pushd "$SCRIPT_DIR" >/dev/null
echo "Restoring..."
dotnet restore note-organizer.sln
echo "Building..."
dotnet build note-organizer.sln -c Release -warnaserror:false
popd >/dev/null
