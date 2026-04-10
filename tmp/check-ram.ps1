while($true) {
    $mem = Get-CimInstance Win32_OperatingSystem | Select-Object FreePhysicalMemory, TotalVisibleMemorySize
    $freeGB = [math]::round($mem.FreePhysicalMemory / 1MB, 2)
    $totalGB = [math]::round($mem.TotalVisibleMemorySize / 1MB, 2)
    $percent = [math]::round(($mem.FreePhysicalMemory / $mem.TotalVisibleMemorySize) * 100, 1)
    
    Clear-Host
    Write-Host "--- MONITOR DE MEMORIA (Modo Lite) ---" -ForegroundColor Cyan
    Write-Host "Memoria Libre: $freeGB GB de $totalGB GB ($percent%)" -ForegroundColor Yellow
    Write-Host "--------------------------------------"
    
    # Mostrar top 5 procesos por memoria
    Get-Process | Sort-Object WorkingSet64 -Descending | Select-Object -First 5 Name, @{Name='RAM_MB';Expression={[math]::round($_.WorkingSet64 / 1MB, 2)}} | Format-Table
    
    Start-Sleep -Seconds 5
}
