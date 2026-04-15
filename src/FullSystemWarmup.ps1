param(
    [string]$BaseUrl = "http://localhost:5218",
    [string]$Username = "admin",
    [string]$Password = "Admin123*!"
)

Write-Host "--- SIMULADOR DE CARGA INICIAL (WARMUP) ---" -ForegroundColor Cyan
Write-Host "Entorno Objetivo: $BaseUrl" -ForegroundColor Gray
Write-Host "Simulando flujo completo tras el Login..." -ForegroundColor Gray

function Show-ModuleStatus($name, $status, $time, $details) {
    $color = if ($status -eq "OK") { "Green" } else { "Red" }
    Write-Host "[Módulo: $name]" -NoNewline
    Write-Host " Status: $status" -ForegroundColor $color -NoNewline
    Write-Host " Time: $($time)ms" -NoNewline
    if ($details) { Write-Host " | $details" } else { Write-Host "" }
}

# 1. AUTENTICACIÓN
$sw = [diagnostics.stopwatch]::StartNew()
try {
    $loginBody = @{ username = $Username; password = $Password } | ConvertTo-Json
    $loginRes = Invoke-RestMethod -Uri "$BaseUrl/api/Auth/login" -Method Post -Body $loginBody -ContentType "application/json"
    $sw.Stop()
    $token = if ($loginRes.token) { $loginRes.token } else { $loginRes.Token }
    if (!$token) { throw "Token not found in response" }
    Show-ModuleStatus "IDENTITY/AUTH" "OK" $sw.ElapsedMilliseconds "Token obtenido con éxito"
} catch {
    $sw.Stop()
    Show-ModuleStatus "IDENTITY/AUTH" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
    exit
}

$headers = @{ Authorization = "Bearer $token" }

# 2. PERFIL DE USUARIO
$sw.Restart()
try {
    $profileRes = Invoke-RestMethod -Uri "$BaseUrl/api/Auth/debug-users" -Method Get -Headers $headers
    $sw.Stop()
    $details = "User: $($Username) | Roles: OK"
    Show-ModuleStatus "USER_PROFILE" "OK" $sw.ElapsedMilliseconds $details
} catch {
    $sw.Stop()
    Show-ModuleStatus "USER_PROFILE" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

# 3. DASHBOARD / FINANCIAL INSIGHTS
$sw.Restart()
try {
    $insightsRes = Invoke-RestMethod -Uri "$BaseUrl/api/Dashboard/Insights" -Method Get -Headers $headers
    $sw.Stop()
    $details = "Ventas: $($insightsRes.ventasHoy) | Pacientes: $($insightsRes.pacientesHoy)"
    Show-ModuleStatus "FINANCE_INSIGHTS" "OK" $sw.ElapsedMilliseconds $details
} catch {
    $sw.Stop()
    Show-ModuleStatus "FINANCE_INSIGHTS" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

# 4. CONSULTAS / CITAS (Vía Billing Controller)
$sw.Restart()
try {
    $appointmentsRes = Invoke-RestMethod -Uri "$BaseUrl/api/Billing/Appointments" -Method Get -Headers $headers
    $sw.Stop()
    Show-ModuleStatus "APPOINTMENTS" "OK" $sw.ElapsedMilliseconds "Módulo de citas disponible"
} catch {
    $sw.Stop()
    Show-ModuleStatus "APPOINTMENTS" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

# 5. RADIOLOGÍA / VENTAS RX
$sw.Restart()
try {
    $billingRes = Invoke-RestMethod -Uri "$BaseUrl/api/Billing/DailyBilledPatients" -Method Get -Headers $headers
    $sw.Stop()
    Show-ModuleStatus "RX_MODULE" "OK" $sw.ElapsedMilliseconds "Módulo de facturacuón disponible"
} catch {
    $sw.Stop()
    Show-ModuleStatus "RX_MODULE" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

# 6. EXPEDIENTE DE FACTURACIÓN (AUDITORÍA)
$sw.Restart()
try {
    $expRes = Invoke-RestMethod -Uri "$BaseUrl/api/Expediente/billing" -Method Get -Headers $headers
    $sw.Stop()
    Show-ModuleStatus "BILLING_AUDIT" "OK" $sw.ElapsedMilliseconds "Módulo de auditoría listo"
} catch {
    $sw.Stop()
    Show-ModuleStatus "BILLING_AUDIT" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

# 7. DIAGNÓSTICOS DE INFRAESTRUCTURA
$sw.Restart()
try {
    $healthRes = Invoke-RestMethod -Uri "$BaseUrl/api/Diagnostics/HealthInsight" -Method Get -Headers $headers
    $sw.Stop()
    $details = "DB Latency: $($healthRes.databaseLatency)ms"
    Show-ModuleStatus "INFRA_HEALTH" "OK" $sw.ElapsedMilliseconds $details
} catch {
    $sw.Stop()
    Show-ModuleStatus "INFRA_HEALTH" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

Write-Host "--- REPORTE DE DISPONIBILIDAD COMPLETADO ---" -ForegroundColor Cyan
