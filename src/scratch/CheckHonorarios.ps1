# Honorarium Automation Check
# This script verifies the new Honorarium endpoints and auto-assignment logic

$baseUrl = "http://localhost:5000/api" # Ajustar según puerto real
$token = "TU_TOKEN_AQUI" # Se asume sesión activa en el navegador

function Test-Endpoint {
    param($name, $path, $method = "GET", $body = $null)
    Write-Host "Checking $name... " -NoNewline
    try {
        $params = @{
            Uri = "$baseUrl/$path"
            Method = $method
            Headers = @{ Authorization = "Bearer $token" }
            ContentType = "application/json"
        }
        if ($body) { $params.Body = $body | ConvertTo-Json }
        
        $res = Invoke-RestMethod @params
        Write-Host "OK" -ForegroundColor Green
        return $res
    } catch {
        Write-Host "FAILED" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Gray
        return $null
    }
}

Write-Host "--- INICIANDO AUTOMATIZACIÓN DE HONORARIOS ---" -ForegroundColor Cyan

# 1. Verificar Configuración
$configs = Test-Endpoint "Configuración de Honorarios" "HonorarioConfig"
if ($configs) {
    Write-Host "Categorías configuradas: $($configs.Count)"
}

# 2. Verificar Pendientes
$pendientes = Test-Endpoint "Servicios Pendientes" "AsignacionHonorarios/pendientes?estado=PENDIENTE"
if ($pendientes) {
    Write-Host "Servicios sin asignar: $($pendientes.Count)"
}

# 3. Verificar Reporte de Cálculo
$hoy = Get-Date -Format "yyyy-MM-dd"
$reporte = Test-Endpoint "Reporte de Cálculo" "Medicos/reporte/calculo-honorarios?startDate=$hoy&endDate=$hoy"
if ($reporte) {
    Write-Host "Médicos con actividad hoy: $($reporte.Count)"
}

Write-Host "--- VERIFICACIÓN COMPLETADA ---" -ForegroundColor Cyan
