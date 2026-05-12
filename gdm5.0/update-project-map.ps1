# Manual Project Map Update Script
# Run this locally to update PROJECT_MAP.md

Write-Host "??? Updating PROJECT_MAP.md..." -ForegroundColor Cyan
Write-Host ""

$projectRoot = "D:\Dev\Ggm\gdm_v5"
$mapFile = Join-Path $projectRoot "PROJECT_MAP.md"

if (-not (Test-Path $mapFile)) {
    Write-Host "? PROJECT_MAP.md not found!" -ForegroundColor Red
    exit 1
}

# Analyze project
Write-Host "?? Analyzing project structure..." -ForegroundColor Yellow

# Get .NET version
$csprojPath = Join-Path $projectRoot "gdm5.0\gdm5.0.csproj"
$csprojContent = Get-Content $csprojPath -Raw
if ($csprojContent -match '<TargetFramework>net([0-9.]+)</TargetFramework>') {
    $dotnetVersion = $matches[1]
} else {
    $dotnetVersion = "Unknown"
}

# Count files
$csFiles = (Get-ChildItem -Path (Join-Path $projectRoot "gdm5.0") -Filter "*.cs" -Recurse | 
            Where-Object { $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\bin\\" }).Count
$tsFiles = (Get-ChildItem -Path (Join-Path $projectRoot "gdm5.0\ClientApp") -Filter "*.ts" -Recurse | 
            Where-Object { $_.FullName -notmatch "\\node_modules\\" }).Count

# Get EF version
if ($csprojContent -match 'Microsoft\.EntityFrameworkCore\.SqlServer.*Version="([^"]+)"') {
    $efVersion = $matches[1]
} else {
    $efVersion = "Unknown"
}

# Check Identity Server
$hasIdentityServer = $csprojContent -match "IdentityServer"

# Get Angular version
$packageJsonPath = Join-Path $projectRoot "gdm5.0\ClientApp\package.json"
if (Test-Path $packageJsonPath) {
    $packageJson = Get-Content $packageJsonPath | ConvertFrom-Json
    $angularVersion = $packageJson.dependencies.'@angular/core'
} else {
    $angularVersion = "Unknown"
}

Write-Host "? Analysis complete!" -ForegroundColor Green
Write-Host ""
Write-Host "?? Project Statistics:" -ForegroundColor Cyan
Write-Host "  • C# Files: $csFiles" -ForegroundColor White
Write-Host "  • TypeScript Files: $tsFiles" -ForegroundColor White
Write-Host "  • .NET Version: $dotnetVersion" -ForegroundColor White
Write-Host "  • EF Core Version: $efVersion" -ForegroundColor White
Write-Host "  • Angular Version: $angularVersion" -ForegroundColor White
Write-Host "  • Identity Server: $(if ($hasIdentityServer) { 'Yes' } else { 'No (JWT Bearer)' })" -ForegroundColor White
Write-Host ""

# Update PROJECT_MAP.md
Write-Host "?? Updating PROJECT_MAP.md..." -ForegroundColor Yellow

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$mapContent = Get-Content $mapFile -Raw

# Update last updated timestamp
$mapContent = $mapContent -replace '\*\*Last Updated:\*\*.*', "**Last Updated:** $timestamp - Updated manually"

# Update .NET version
$mapContent = $mapContent -replace 'ASP\.NET Core [0-9.]+ Backend', "ASP.NET Core $dotnetVersion Backend"
$mapContent = $mapContent -replace '\*\*Framework:\*\* ASP\.NET Core [0-9.]+', "**Framework:** ASP.NET Core $dotnetVersion"

# Update EF version
$mapContent = $mapContent -replace 'Entity Framework Core [0-9.]+', "Entity Framework Core $efVersion"

# Update Identity Server status
if (-not $hasIdentityServer) {
    $mapContent = $mapContent -replace '- IdentityServer4.*', '- ~~IdentityServer4~~ (Removed - using JWT Bearer only)'
}

# Update or add statistics section
if ($mapContent -match '## Statistics \(Auto-generated\)') {
    # Replace existing statistics
    $statsPattern = '(?s)## Statistics \(Auto-generated\).*?(?=\n##|\z)'
    $newStats = @"
## Statistics (Auto-generated)

**Last Analyzed:** $timestamp

### Codebase Metrics:
- **C# Files:** $csFiles
- **TypeScript Files:** $tsFiles
- **.NET Version:** $dotnetVersion
- **Entity Framework:** $efVersion
- **Angular Version:** $angularVersion
- **Identity Server:** $(if ($hasIdentityServer) { 'Yes' } else { 'No (JWT Bearer)' })

### Git Information:
"@

    # Get git info
    Push-Location $projectRoot
    try {
        $lastCommit = git log -1 --pretty=format:'%h - %s (%an, %ar)'
        $currentBranch = git branch --show-current
        $newStats += "`n- **Last Commit:** $lastCommit"
        $newStats += "`n- **Current Branch:** $currentBranch"
    } finally {
        Pop-Location
    }

    $newStats += "`n"

    $mapContent = $mapContent -replace $statsPattern, $newStats
} else {
    # Add statistics section at the end
    $newStats = @"

---

## Statistics (Auto-generated)

**Last Analyzed:** $timestamp

### Codebase Metrics:
- **C# Files:** $csFiles
- **TypeScript Files:** $tsFiles
- **.NET Version:** $dotnetVersion
- **Entity Framework:** $efVersion
- **Angular Version:** $angularVersion
- **Identity Server:** $(if ($hasIdentityServer) { 'Yes' } else { 'No (JWT Bearer)' })

"@
    $mapContent += $newStats
}

# Save updated content
Set-Content -Path $mapFile -Value $mapContent -NoNewline

Write-Host "? PROJECT_MAP.md updated successfully!" -ForegroundColor Green
Write-Host ""

# Show git diff
Write-Host "?? Changes made:" -ForegroundColor Cyan
Push-Location $projectRoot
try {
    git diff PROJECT_MAP.md
} finally {
    Pop-Location
}

Write-Host ""
Write-Host "?? Next steps:" -ForegroundColor Yellow
Write-Host "  1. Review the changes: git diff PROJECT_MAP.md" -ForegroundColor White
Write-Host "  2. Commit if satisfied: git add PROJECT_MAP.md && git commit -m '??? Update PROJECT_MAP.md'" -ForegroundColor White
Write-Host "  3. Push changes: git push" -ForegroundColor White
Write-Host ""

# Offer to commit
$commit = Read-Host "Do you want to commit these changes? (y/n)"
if ($commit -eq 'y' -or $commit -eq 'Y') {
    Push-Location $projectRoot
    try {
        git add PROJECT_MAP.md
        git commit -m "??? Update PROJECT_MAP.md

- Updated metadata and versions
- Refreshed statistics
- .NET $dotnetVersion
- EF Core $efVersion
- $csFiles C# files analyzed"

        Write-Host ""
        Write-Host "? Changes committed!" -ForegroundColor Green
        Write-Host ""

        $push = Read-Host "Push to remote? (y/n)"
        if ($push -eq 'y' -or $push -eq 'Y') {
            git push
            Write-Host "? Changes pushed to remote!" -ForegroundColor Green
        }
    } catch {
        Write-Host "? Error committing: $_" -ForegroundColor Red
    } finally {
        Pop-Location
    }
}

Write-Host ""
Write-Host "?? Done!" -ForegroundColor Green
