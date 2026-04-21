$ErrorActionPreference = "Stop"

$LocalUrl = "http://localhost:5242"
$CloudUrl = "https://sistemasathospitalario.onrender.com"

# Ignorar certificados SSL inválidos para entorno local
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

function Test-Endpoint {
    param (
        [string]$BaseUrl,
        [string]$Endpoint,
        [string]$Method = "GET",
        [string]$Body = $null,
        [string]$EnvName
    )

    $Url = "$BaseUrl$Endpoint"
    Write-Host "Probando [$EnvName] $Method $Url..." -NoNewline

    try {
        if ($Method -eq "GET") {
            $response = Invoke-RestMethod -Uri $Url -Method GET -TimeoutSec 15
        } else {
            $headers = @{ "Content-Type" = "application/json" }
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Headers $headers -Body $Body -TimeoutSec 15
        }
        Write-Host " OK" -ForegroundColor Green
        return $true
    } catch {
        Write-Host " FAILED" -ForegroundColor Red
        Write-Host "  Error: $_" -ForegroundColor Red
        return $false
    }
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Advanced Reports Smoke Test " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Test Local
Write-Host "`n--- Testing Local Environment ---" -ForegroundColor Yellow
$localCatalog = Test-Endpoint -BaseUrl $LocalUrl -Endpoint "/api/Catalog/unified" -EnvName "Local"
$localSettings = Test-Endpoint -BaseUrl $LocalUrl -Endpoint "/api/ConfiguracionGeneral" -EnvName "Local"

# Test Cloud
Write-Host "`n--- Testing Cloud Environment ---" -ForegroundColor Yellow
$cloudCatalog = Test-Endpoint -BaseUrl $CloudUrl -Endpoint "/api/Catalog/unified" -EnvName "Cloud"
$cloudSettings = Test-Endpoint -BaseUrl $CloudUrl -Endpoint "/api/ConfiguracionGeneral" -EnvName "Cloud"

Write-Host "`n--- Summary ---" -ForegroundColor Cyan
Write-Host "Local Catalog: $($localCatalog)"
Write-Host "Local Settings: $($localSettings)"
Write-Host "Cloud Catalog: $($cloudCatalog)"
Write-Host "Cloud Settings: $($cloudSettings)"
Write-Host "========================================" -ForegroundColor Cyan
