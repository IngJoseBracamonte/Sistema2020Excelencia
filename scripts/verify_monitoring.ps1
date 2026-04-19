Write-Host "--- Verificando Automatización de Monitoreo Proactivo ---" -ForegroundColor Cyan

$diPath = "src/SistemaSatHospitalario.Infrastructure/DependencyInjection.cs"
$diContent = Get-Content $diPath -Raw

Write-Host "[CHECK] Registro de Hosted Service en DI..." -NoNewline
if ($diContent -match "services.AddHostedService<SistemaSatHospitalario.Infrastructure.Services.LegacyLabMonitoringWorker>") {
    Write-Host " [OK]" -ForegroundColor Green
} else {
    Write-Host " [FALLO]" -ForegroundColor Red
}

$workerPath = "src/SistemaSatHospitalario.Infrastructure/Services/LegacyLabMonitoringWorker.cs"
Write-Host "[CHECK] Existencia del Worker File..." -NoNewline
if (Test-Path $workerPath) {
    Write-Host " [OK]" -ForegroundColor Green
} else {
    Write-Host " [FALLO]" -ForegroundColor Red
}

$logPath = "legacy_sync_log.txt"
Write-Host "[CHECK] Heartbeats en el log file..." -NoNewline
if (Test-Path $logPath) {
    $recentLog = Get-Content $logPath -Tail 5
    if ($recentLog -match "\[MONITOR\]") {
        Write-Host " [OK] (Detectados en últimas líneas)" -ForegroundColor Green
    } else {
        Write-Host " [PENDIENTE] (Aún no hay heartbeats, espere el primer ciclo)" -ForegroundColor Yellow
    }
} else {
    Write-Host " [N/A] (Archivo de log no creado aún)" -ForegroundColor Gray
}

Write-Host "`nVerificación Finalizada." -ForegroundColor Cyan
