# ═══════════════════════════════════════════════════════════
# Hospital System - Monitor de Salud y Auditoria v1.0
# ═══════════════════════════════════════════════════════════

function Write-Status ($service, $status) {
    $color = if ($status -eq "Healthy") { "Green" } else { "Red" }
    Write-Host "  > Service: " -NoNewline; Write-Host "$service " -ForegroundColor Cyan -NoNewline; Write-Host "is " -NoNewline; Write-Host $status -ForegroundColor $color
}

Write-Host @"

  +-----------------------------------------------------------+
  |   Hospital System - Auditoria de Procesos                 |
  |   Verificacion de Logs y Estabilidad                      |
  +-----------------------------------------------------------+
"@

# 1. Verificar Contenedores
Write-Host "`n[1/3] Estado de Infraestructura Docker:" -ForegroundColor Yellow
$containers = docker ps --format "{{.Names}}|{{.Status}}"
foreach ($c in $containers) {
    $parts = $c.Split("|")
    $status = if ($parts[1].Contains("healthy")) { "Healthy" } else { "Unhealthy/Starting" }
    Write-Status $parts[0] $status
}

# 2. Auditoria de Logs de la API (Backend)
Write-Host "`n[2/3] Buscando Errores Críticos en el Backend (sat-api):" -ForegroundColor Yellow
  $logs = docker logs sat-api --since 10m 2>&1
  $missingTable = $logs | Select-String "Table '.*' doesn't exist"
  
  if ($missingTable) {
      Write-Host "  [ALERTA] Se detectaron tablas faltantes en la DB." -ForegroundColor Red
      Write-Host "  [SOLUCION] El sistema ejecutará el Auto-Migrator en el próximo reinicio." -ForegroundColor Cyan
      Write-Host "  Ejecuta: .\update.ps1 para aplicar la corrección automática." -ForegroundColor Yellow
  }

  $criticalErrors = $logs | Select-String "ERR|Exception|500|Fatal" | Select-Object -Last 5
  if ($criticalErrors) {
      Write-Host "  [!] Se detectaron posibles anomalías en los logs recientes:" -ForegroundColor Yellow
      $criticalErrors | ForEach-Object { Write-Host "      - $($_.ToString().Trim())" -ForegroundColor Gray }
  } else {
      Write-Host "  [OK] No se detectaron errores críticos recientes." -ForegroundColor Green
  }

# 3. Auditoria de Logs del Frontend
Write-Host "`n[3/3] Buscando Errores de Proxy/Routing (sat-frontend):" -ForegroundColor Yellow
$frontLogs = docker logs sat-frontend --since 10m 2>&1
$frontErrors = $frontLogs | Select-String -Pattern " 404 ", " 500 ", " 502 ", "error"

if ($frontErrors) {
    Write-Host "  [!] Se detectaron errores de navegacion o proxy:" -ForegroundColor Yellow
    $frontErrors | Select-Object -First 5 | ForEach-Object { Write-Host "      - $_" -ForegroundColor Gray }
} else {
    Write-Host "  [OK] Nginx operando sin errores de comunicacion." -ForegroundColor Green
}

Write-Host "`n  Auditoria completada. Si ves todo en verde, el sistema esta estable.`n" -ForegroundColor White
