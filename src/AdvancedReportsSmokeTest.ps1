param(
    [string]$BaseUrl = "http://localhost:5218",
    [string]$Username = "admin",
    [string]$Password = "Admin123*!"
)

Write-Host "--- PRUEBAS DE REPORTES AVANZADOS (AUDITORÍA & NEGOCIO) ---" -ForegroundColor Cyan
Write-Host "Entorno Objetivo: $BaseUrl" -ForegroundColor Gray
Write-Host "Verificando módulos de Expediente, Control de Citas y Auditoría Técnica..." -ForegroundColor Gray

function Show-AuditStatus($name, $status, $time, $details) {
    $color = if ($status -eq "OK") { "Green" } else { "Red" }
    Write-Host "[Reporte: $name]" -NoNewline
    Write-Host " Status: $status" -ForegroundColor $color -NoNewline
    Write-Host " Time: $($time)ms" -NoNewline
    if ($details) { Write-Host " | $details" } else { Write-Host "" }
}

# 1. LOGIN
$loginBody = @{ username = $Username; password = $Password } | ConvertTo-Json
$loginRes = Invoke-RestMethod -Uri "$BaseUrl/api/Auth/login" -Method Post -Body $loginBody -ContentType "application/json"
$token = if ($loginRes.token) { $loginRes.token } else { $loginRes.Token }
if (!$token) { throw "Token not found in response" }
$headers = @{ Authorization = "Bearer $token" }

# 2. EXPEDIENTE DE FACTURACIÓN (AUDITORÍA)
$sw = [diagnostics.stopwatch]::StartNew()
try {
    $expedienteRes = Invoke-RestMethod -Uri "$BaseUrl/api/Expediente/billing?startDate=2026-01-01&searchTerm=audit" -Method Get -Headers $headers
    $sw.Stop()
    Show-AuditStatus "EXPEDIENTE_FACTURACION" "OK" $sw.ElapsedMilliseconds "Módulo de Auditoría respondido"
} catch {
    $sw.Stop()
    Show-AuditStatus "EXPEDIENTE_FACTURACION" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

# 3. CONTROL DE CITAS (OPERACIONES)
$sw.Restart()
try {
    $citasRes = Invoke-RestMethod -Uri "$BaseUrl/api/Expediente/citas" -Method Get -Headers $headers
    $sw.Stop()
    Show-AuditStatus "CONTROL_CITAS" "OK" $sw.ElapsedMilliseconds "Listado operacional disponible"
} catch {
    $sw.Stop()
    Show-AuditStatus "CONTROL_CITAS" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

# 4. DASHBOARD DE VALIDACIÓN TÉCNICA
$sw.Restart()
try {
    $validationRes = Invoke-RestMethod -Uri "$BaseUrl/api/Validation/dashboard" -Method Get -Headers $headers
    $sw.Stop()
    Show-AuditStatus "AUDITORIA_TECNICA" "OK" $sw.ElapsedMilliseconds "Dashboard de validación cargado"
} catch {
    $sw.Stop()
    Show-AuditStatus "AUDITORIA_TECNICA" "FAIL" $sw.ElapsedMilliseconds $_.Exception.Message
}

Write-Host "--- PRUEBAS DE NEGOCIO COMPLETADAS ---" -ForegroundColor Cyan
