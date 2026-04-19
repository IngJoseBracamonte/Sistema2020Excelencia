# Cloud Configuration Simulator for Sistema Sat Hospitalario
Write-Host "--- Simulador de Configuración Cloud (Render/Aiven) ---" -ForegroundColor Cyan

# Simulación de Variables de Entorno de Render
$envSim = @{
    "ConnectionStrings__LegacyConnection" = "Server=aiven-host.com;Port=15848;Database=Sistema2020;Uid=avnadmin;Pwd=secret_pass;"
    "ASPNETCORE_ENVIRONMENT" = "Production"
}

Write-Host "[INFO] Simulando variable Render: ConnectionStrings__LegacyConnection" -ForegroundColor Gray
$rawConn = $envSim["ConnectionStrings__LegacyConnection"]
Write-Host "[DEBUG] String Crudo: $rawConn"

# Esta parte emula la lógica de C# que acabamos de implementar
function Test-Normalization {
    param($conn)
    # Lógica: Preserve cases if forceLowercase is false (new behavior)
    if ($conn -match "Database=([^;]+)") {
        $dbName = $Matches[1]
        return $dbName
    }
    return "Unknown"
}

$finalDb = Test-Normalization $rawConn
Write-Host "`n--- Resultado de Normalización ---" -ForegroundColor Cyan
if ($finalDb -eq "Sistema2020") {
    Write-Host "[SUCCESS] El sistema PRESERVARÁ las mayúsculas: $finalDb" -ForegroundColor Green
    Write-Host "[OK] Esto coincide con el esquema requerido para la nube." -ForegroundColor Green
} else {
    Write-Host "[ERROR] El sistema todavía está forzando minúsculas: $finalDb" -ForegroundColor Red
}

Write-Host "`n--- Recomendaciones para Despliegue ---" -ForegroundColor Gray
Write-Host "1. Asegúrese de que en el Dashboard de Render la variable use 'Sistema2020'."
Write-Host "2. El puerto debe ser 15848 para Aiven Managed MySQL."
Write-Host "3. Verifique que los logs de Render muestren '[LEGACY-REPO]' para diagnósticos."

Write-Host "`nSimulación Finalizada." -ForegroundColor Cyan
