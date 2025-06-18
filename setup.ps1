$repos = @(
    "https://github.com/ValveSoftware/counter-strike_rules_and_regs.git",
    "https://github.com/ValveSoftware/counter-strike_regional_standings.git"
)

$basePath = "$PSScriptRoot/workdir/repos"
New-Item -ItemType Directory -Path $basePath -Force | Out-Null

foreach ($repoUrl in $repos) {
    $repoName = ($repoUrl -split '/' | Select-Object -Last 1) -replace '\.git$'
    $repoPath = Join-Path $basePath $repoName

    if (-Not (Test-Path $repoPath)) {
        Write-Host "Cloning $repoName..."
        git clone $repoUrl $repoPath
    } else {
        Write-Host "Pulling latest in $repoName..."
        git -C $repoPath pull
    }
}