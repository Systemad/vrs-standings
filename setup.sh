#!/usr/bin/env bash
set -e

repos=(
    "https://github.com/ValveSoftware/counter-strike_rules_and_regs.git"
    "https://github.com/ValveSoftware/counter-strike_regional_standings.git"
)

base_path="$(dirname "$(realpath "$0")")/workdir/repos"
mkdir -p "$base_path"

for repo_url in "${repos[@]}"; do
    repo_name=$(basename "$repo_url" .git)
    repo_path="$base_path/$repo_name"

    if [[ ! -d "$repo_path/.git" ]]; then
        echo "Cloning $repo_name..."
        git clone "$repo_url" "$repo_path"
    else
        echo "Pulling latest in $repo_name..."
        git -C "$repo_path" pull
    fi
done
