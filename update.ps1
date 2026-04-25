# ═══════════════════════════════════════════════════════════
# Hospital System - Actualizador Rápido v1.0
# ═══════════════════════════════════════════════════════════

$ErrorActionPreference = "Stop"

function Write-Ok ($msg) { Write-Host "    [OK] $msg" -ForegroundColor Green }
function Write-Info ($msg) { Write-Host "    [INFO] $msg" -ForegroundColor Cyan }
function Write-Header ($msg) { 
    Write-Host "`n  $msg" -ForegroundColor White -BackgroundColor DarkBlue
    Write-Host "   + " + ("-" * 50) -ForegroundColor Gray
}

Write-Host @"

  +-----------------------------------------------------------+
  |   Hospital System - Fast Update                           |
  |   Compilacion y Despliegue de Cambios                     |
  +-----------------------------------------------------------+
"@

# 1. Detectar IP Local (Por si cambió el WiFi/Red)
Write-Header "[1/3] Verificando IP Local..."
try {
    $LocalIp = (Get-NetRoute -DestinationPrefix 0.0.0.0/0 | Get-NetIPInterface | Get-NetIPAddress -AddressFamily IPv4 | Select-Object -First 1).IPAddress
    $PcName = $env:COMPUTERNAME
    Write-Ok "IP Local detectada: $LocalIp"
    Write-Ok "Nombre de PC detectado: $PcName"
    
    # Actualizar LOCAL_IP y PC_NAME en el .env si existe
    if (Test-Path ".env") {
        $envContent = Get-Content ".env"
        $newContent = $envContent -replace "LOCAL_IP=.*", "LOCAL_IP=$LocalIp"
        $newContent = $newContent -replace "PC_NAME=.*", "PC_NAME=$PcName"
        $newContent | Set-Content ".env"
        Write-Ok "Archivo .env actualizado con la IP y Nombre de PC actuales."
    }
} catch {
    Write-Info "No se pudo actualizar la IP, se mantendra la anterior."
}

# 2. Re-compilar y Levantar
Write-Header "[2/3] Actualizando Contenedores..."
Write-Info "Revisando cambios en el codigo y reconstruyendo imagenes..."
docker compose up -d --build

# 3. Estado Final
Write-Header "[3/3] Verificando Estado..."
Start-Sleep -Seconds 2
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

Write-Host "`n  [LISTO] El sistema ha sido actualizado con tus ultimos cambios." -ForegroundColor Green
Write-Host "  Acceso: http://localhost o http://$LocalIp`n" -ForegroundColor Cyan
