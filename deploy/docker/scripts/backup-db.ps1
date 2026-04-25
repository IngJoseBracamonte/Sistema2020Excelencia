<#
.SYNOPSIS
    Backup de las bases de datos MySQL del Sistema Sat Hospitalario.
.DESCRIPTION
    Genera dumps SQL de las 3 bases de datos en la carpeta deploy/docker/backups/.
    Requiere el cliente 'mysql' y 'mysqldump' en el PATH.
.EXAMPLE
    .\backup-db.ps1
    .\backup-db.ps1 -MysqlUser root -MysqlPassword MiPass
#>

param(
    [string]$MysqlUser = "root",
    [string]$MysqlPassword = "Labordono1818",
    [string]$MysqlHost = "127.0.0.1",
    [int]$MysqlPort = 3306
)

$ErrorActionPreference = "Stop"
$timestamp = Get-Date -Format "yyyy-MM-dd_HHmmss"
$backupDir = Join-Path $PSScriptRoot "..\backups"

if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
}

$databases = @("SatHospitalario", "SatHospitalarioIdentity", "sistema2020")

Write-Host ""
Write-Host "  🗄️  Backup de Bases de Datos — $timestamp" -ForegroundColor Cyan
Write-Host "  ──────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""

$mysqldump = Get-Command mysqldump -ErrorAction SilentlyContinue
if (-not $mysqldump) {
    Write-Host "  ❌ 'mysqldump' no encontrado en PATH." -ForegroundColor Red
    Write-Host "  Agregue la carpeta bin de MySQL a su PATH." -ForegroundColor DarkGray
    exit 1
}

foreach ($db in $databases) {
    $outputFile = Join-Path $backupDir "${db}_${timestamp}.sql"
    Write-Host "  Exportando $db..." -NoNewline -ForegroundColor White

    try {
        & mysqldump -u $MysqlUser "-p$MysqlPassword" -h $MysqlHost -P $MysqlPort `
            --single-transaction --routines --triggers --events `
            $db > $outputFile 2>$null

        if ($LASTEXITCODE -eq 0) {
            $size = [math]::Round((Get-Item $outputFile).Length / 1MB, 2)
            Write-Host " ✅ ($size MB)" -ForegroundColor Green
        } else {
            Write-Host " ❌ Error (código: $LASTEXITCODE)" -ForegroundColor Red
        }
    } catch {
        Write-Host " ❌ $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "  📁 Backups guardados en: $backupDir" -ForegroundColor Cyan
Write-Host ""
