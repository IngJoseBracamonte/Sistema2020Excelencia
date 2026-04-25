#Requires -Version 5.1
<#
.SYNOPSIS
    Desinstalador del Sistema Sat Hospitalario Docker.
.DESCRIPTION
    Detiene contenedores, opcionalmente borra volúmenes y limpia imágenes.
    Remueve la entrada del archivo hosts si se ejecuta como Admin.
#>

param(
    [string]$DomainName = "hospital.local",
    [switch]$RemoveVolumes,
    [switch]$RemoveImages,
    [switch]$Force
)

$ErrorActionPreference = "Stop"
$ScriptRoot = $PSScriptRoot
if (-not $ScriptRoot) { $ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path }

Write-Host ""
Write-Host "  ╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Yellow
Write-Host "  ║   Hospital System - Docker Uninstaller                   ║" -ForegroundColor Yellow
Write-Host "  ╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Yellow
Write-Host ""

# Confirmación
if (-not $Force) {
    Write-Host "  Esta acción detendrá todos los contenedores del sistema." -ForegroundColor White
    if ($RemoveVolumes) {
        Write-Host "  ⚠️  También se borrarán los volúmenes (logs, caché Redis)." -ForegroundColor Yellow
    }
    $confirm = Read-Host "  ¿Continuar? (s/N)"
    if ($confirm -notin @('s','S','si','Si','SI','y','Y')) {
        Write-Host "  Abortado." -ForegroundColor Gray
        exit 0
    }
}

Push-Location $ScriptRoot

try {
    # Detener contenedores
    Write-Host ""
    Write-Host "  [1/3] Deteniendo contenedores..." -ForegroundColor Yellow

    if ($RemoveVolumes) {
        docker compose down -v 2>&1 | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
        Write-Host "    ✅ Contenedores detenidos y volúmenes eliminados" -ForegroundColor Green
    } else {
        docker compose down 2>&1 | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
        Write-Host "    ✅ Contenedores detenidos (volúmenes conservados)" -ForegroundColor Green
    }

    # Limpiar imágenes
    if ($RemoveImages) {
        Write-Host ""
        Write-Host "  [2/3] Eliminando imágenes Docker del proyecto..." -ForegroundColor Yellow
        
        $images = docker images --filter "label=maintainer=SistemaSatHospitalario" -q 2>$null
        if ($images) {
            docker rmi $images -f 2>&1 | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
            Write-Host "    ✅ Imágenes eliminadas" -ForegroundColor Green
        } else {
            Write-Host "    ℹ️  No se encontraron imágenes del proyecto" -ForegroundColor DarkCyan
        }
    } else {
        Write-Host ""
        Write-Host "  [2/3] Imágenes conservadas (use -RemoveImages para eliminar)" -ForegroundColor DarkGray
    }

    # Limpiar hosts file
    Write-Host ""
    Write-Host "  [3/3] Limpiando DNS local..." -ForegroundColor Yellow

    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    $isAdmin = $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

    if ($isAdmin) {
        $hostsPath = "C:\Windows\System32\drivers\etc\hosts"
        $content = Get-Content $hostsPath
        $filtered = $content | Where-Object { 
            $_ -notmatch [regex]::Escape($DomainName) -and 
            $_ -ne "# Sistema Sat Hospitalario (Docker)" 
        }
        Set-Content -Path $hostsPath -Value $filtered -Encoding ASCII
        ipconfig /flushdns | Out-Null
        Write-Host "    ✅ Entrada '$DomainName' removida del archivo hosts" -ForegroundColor Green
    } else {
        Write-Host "    ⚠️  No es administrador — remueva manualmente de hosts:" -ForegroundColor Yellow
        Write-Host "    C:\Windows\System32\drivers\etc\hosts → borrar línea con '$DomainName'" -ForegroundColor DarkGray
    }

} finally {
    Pop-Location
}

Write-Host ""
Write-Host "  ════════════════════════════════════════════" -ForegroundColor Green
Write-Host "    ✅ DESINSTALACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "  ════════════════════════════════════════════" -ForegroundColor Green
Write-Host ""
Write-Host "    Nota: MySQL del host NO fue afectado." -ForegroundColor DarkGray
Write-Host "    Para reinstalar: .\install.ps1" -ForegroundColor DarkGray
Write-Host ""
