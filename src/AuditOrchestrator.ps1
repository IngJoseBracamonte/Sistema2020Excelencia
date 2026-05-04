param(
    [string]$BaseUrl = "http://localhost:5218",
    [string]$Username = "admin",
    [string]$Password = "Admin123*!"
)

$ErrorActionPreference = "Stop"

Write-Host "--- ORQUESTADOR DE AUDITORIA Y CIERRE ---" -ForegroundColor Cyan

# 1. Login
Write-Host "Iniciando sesion..." -NoNewline
$loginBody = @{ username = $Username; password = $Password } | ConvertTo-Json
$loginRes = Invoke-RestMethod -Uri "$BaseUrl/api/Auth/login" -Method Post -Body $loginBody -ContentType "application/json"
$token = if ($loginRes.token) { $loginRes.token } else { $loginRes.Token }
Write-Host " OK" -ForegroundColor Green

$headers = @{ Authorization = "Bearer $token" }

# 2. Verificar Insights de Seguros
Write-Host "Verificando insights de facturacion..." -NoNewline
$insights = Invoke-RestMethod -Uri "$BaseUrl/api/Dashboard/Insights" -Method Get -Headers $headers
Write-Host " OK" -ForegroundColor Green
Write-Host "  -> Ventas por Seguro: $($insights.ventasPorSeguro.Count) convenios detectados"

# 3. Descargar Cierre de Caja (Modo Auditoria)
Write-Host "Generando Reporte de Cierre de Caja Excel..." -NoNewline
$date = Get-Date -Format "yyyy-MM-dd"
$reportUrl = "$BaseUrl/api/Reports/cash-closing?auditMode=true&date=$date"

try {
    $reportData = Invoke-WebRequest -Uri $reportUrl -Headers $headers -Method Get
    $fileName = "Cierre_Caja_Auditado_$date.xlsx"
    
    $filePath = Join-Path (Get-Location) $fileName
    [System.IO.File]::WriteAllBytes($filePath, $reportData.Content)
    
    Write-Host " OK" -ForegroundColor Green
    Write-Host "  -> Archivo generado: $fileName ($($reportData.Content.Length) bytes)"
    Write-Host "  -> Ruta: $filePath"
} catch {
    Write-Host " FALLO" -ForegroundColor Red
    Write-Host "  Error: $_"
}

Write-Host "--- PROCESO DE AUDITORIA FINALIZADO ---" -ForegroundColor Cyan
