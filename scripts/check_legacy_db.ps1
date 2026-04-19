# Core Diagnostic Script for Laboratory Legacy Sync
Write-Host "--- Iniciando Diagnóstico de Laboratorio ---" -ForegroundColor Cyan

$appSettingsPath = "src/SistemaSatHospitalario.WebAPI/appsettings.json"
if (-not (Test-Path $appSettingsPath)) {
    Write-Error "No se encontró appsettings.json en $appSettingsPath"
    exit
}

$settings = Get-Content $appSettingsPath | ConvertFrom-Json
$legacyConn = $settings.ConnectionStrings.LegacyConnection

if ([string]::IsNullOrEmpty($legacyConn)) {
    Write-Host "[ALERTA] LegacyConnection no está definida en appsettings.json" -ForegroundColor Red
} else {
    Write-Host "[OK] LegacyConnection detectada: $($legacyConn -replace 'password=[^;]+', 'password=****')" -ForegroundColor Green
    
    # Intentar parsear el nombre de la base de datos
    if ($legacyConn -match "database=([^;]+)") {
        $dbName = $Matches[1]
        Write-Host "[INFO] Base de datos objetivo: $dbName" -ForegroundColor Yellow
    }
}

Write-Host "`n--- Verificando LocalDB (SatHospitalario) ---" -ForegroundColor Cyan
$nativeConn = $settings.ConnectionStrings."mysql-system"
Write-Host "[OK] mysql-system detectada: $($nativeConn -replace 'password=[^;]+', 'password=****')" -ForegroundColor Green

Write-Host "`n--- Próximos pasos ---" -ForegroundColor Gray
Write-Host "1. Asegúrese de que el servicio MySQL esté corriendo localmente."
Write-Host "2. Verifique que la base de datos '$dbName' exista en su servidor MySQL."
Write-Host "3. Si usa Docker, verifique que el puerto coincida."

Write-Host "`nDiagnóstico Finalizado." -ForegroundColor Cyan
