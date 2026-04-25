<#
.SYNOPSIS
    Dashboard de estado del Sistema Sat Hospitalario Docker.
.DESCRIPTION
    Muestra el estado de todos los contenedores, health checks y conectividad MySQL.
.EXAMPLE
    .\status.ps1
#>

$ErrorActionPreference = "SilentlyContinue"
$projectRoot = Join-Path $PSScriptRoot "..\..\.."

Write-Host ""
Write-Host "  ╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "  ║   Hospital System — Estado del Sistema                   ║" -ForegroundColor Cyan
Write-Host "  ╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Docker containers
Write-Host "  📦 Contenedores Docker:" -ForegroundColor White
Write-Host "  ──────────────────────────────────────────" -ForegroundColor DarkGray

Push-Location $projectRoot
$containers = @("sat-api", "sat-frontend", "sat-redis")

foreach ($c in $containers) {
    $status = docker inspect --format '{{.State.Status}}' $c 2>$null
    $health = docker inspect --format '{{.State.Health.Status}}' $c 2>$null

    $icon = switch ($status) {
        "running" { "🟢" }
        "exited"  { "🔴" }
        default   { "⚪" }
    }

    $healthIcon = switch ($health) {
        "healthy"   { "(healthy)" }
        "unhealthy" { "(unhealthy!)" }
        "starting"  { "(starting...)" }
        default     { "" }
    }

    $statusColor = if ($status -eq "running") { "Green" } else { "Red" }
    Write-Host "    $icon $($c.PadRight(16))" -NoNewline -ForegroundColor White
    Write-Host "$status " -NoNewline -ForegroundColor $statusColor
    Write-Host $healthIcon -ForegroundColor DarkCyan
}

# MySQL Host
Write-Host ""
Write-Host "  🗄️  MySQL (Host Windows):" -ForegroundColor White
Write-Host "  ──────────────────────────────────────────" -ForegroundColor DarkGray

try {
    $tcp = New-Object System.Net.Sockets.TcpClient
    $tcp.Connect("127.0.0.1", 3306)
    $tcp.Close()
    Write-Host "    🟢 MySQL Server       running (puerto 3306)" -ForegroundColor Green
} catch {
    Write-Host "    🔴 MySQL Server       no accesible (puerto 3306)" -ForegroundColor Red
}

# Health endpoint
Write-Host ""
Write-Host "  ❤️  Health Checks:" -ForegroundColor White
Write-Host "  ──────────────────────────────────────────" -ForegroundColor DarkGray

$endpoints = @(
    @{ Name = "API Health"; Url = "http://localhost:80/health" },
    @{ Name = "Frontend";   Url = "http://localhost:80/" }
)

foreach ($ep in $endpoints) {
    try {
        $resp = Invoke-WebRequest -Uri $ep.Url -TimeoutSec 5 -UseBasicParsing
        if ($resp.StatusCode -eq 200) {
            Write-Host "    🟢 $($ep.Name.PadRight(16)) OK (HTTP $($resp.StatusCode))" -ForegroundColor Green
        } else {
            Write-Host "    🟡 $($ep.Name.PadRight(16)) HTTP $($resp.StatusCode)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "    🔴 $($ep.Name.PadRight(16)) No responde" -ForegroundColor Red
    }
}

Pop-Location

Write-Host ""
Write-Host "  ──────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor DarkGray
Write-Host ""
