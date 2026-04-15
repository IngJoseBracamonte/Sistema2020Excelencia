param(
    [int]$Users = 5,
    [int]$Iterations = 10
)

Write-Host "--- MOTOR DE PRUEBAS DE CARGA (LOADRUNNER) ---" -ForegroundColor Cyan
Write-Host "Simulando carga de $Users usuarios concurrentes por $Iterations iteraciones..." -ForegroundColor Gray

$baseUrl = "http://localhost:5218"
$adminUser = "admin"
$adminPass = "Admin123*!"

$results = @()
$scriptBlock = {
    param($url, $user, $pass)
    $sw = [diagnostics.stopwatch]::StartNew()
    try {
        $body = @{ username = $user; password = $pass } | ConvertTo-Json
        $res = Invoke-RestMethod -Uri "$url/api/Auth/login" -Method Post -Body $body -ContentType "application/json"
        $sw.Stop()
        return [PSCustomObject]@{ Status = "OK"; Latency = $sw.ElapsedMilliseconds }
    } catch {
        $sw.Stop()
        return [PSCustomObject]@{ Status = "FAIL"; Latency = $sw.ElapsedMilliseconds }
    }
}

Write-Host "Lanzando pruebas de Login..." -ForegroundColor Yellow
$jobs = @()
foreach ($i in 1..$Users) {
    foreach ($j in 1..$Iterations) {
        $jobs += Start-Job -ScriptBlock $scriptBlock -ArgumentList @($baseUrl, $adminUser, $adminPass)
    }
}

Write-Host "Esperando resultados..." -ForegroundColor Gray
$results = $jobs | Wait-Job | Receive-Job
Remove-Job $jobs

$successCount = ($results | Where-Object { $_.Status -eq "OK" }).Count
$failCount = ($results | Where-Object { $_.Status -eq "FAIL" }).Count
$avgLatency = ($results.Latency | Measure-Object -Average).Average
$maxLatency = ($results.Latency | Measure-Object -Maximum).Maximum

Write-Host "`n--- RESULTADOS DE CARGA (LOGIN) ---" -ForegroundColor Cyan
Write-Host "Total Peticiones: $($Users * $Iterations)"
Write-Host "Éxitos: $successCount" -ForegroundColor Green
Write-Host "Fallos: $failCount" -ForegroundColor Red
Write-Host "Latencia Promedio: $([Math]::Round($avgLatency, 2)) ms"
Write-Host "Latencia Máxima: $maxLatency ms"
Write-Host "------------------------------------" -ForegroundColor Cyan
