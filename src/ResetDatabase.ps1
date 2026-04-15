Write-Host "--- INICIANDO RESET TOTAL DE BASE DE DATOS (MYSQL DIRECT) ---" -ForegroundColor Cyan

# 1. Limpieza de Migraciones
Write-Host "[1/4] Eliminando carpetas de migraciones obsoletas..." -ForegroundColor Yellow
$migrationPaths = @(
    "src\SistemaSatHospitalario.Infrastructure\Persistence\Migrations",
    "src\SistemaSatHospitalario.Infrastructure\Identity\Migrations",
    "src\SistemaSatHospitalario.Infrastructure\Persistence\Legacy\Migrations"
)

foreach ($path in $migrationPaths) {
    if (Test-Path $path) {
        Remove-Item -Path "$path\*" -Recurse -Force
        Write-Host "  - Limpiado: $path"
    }
}

# 2. Re-generación de Migraciones
Write-Host "[2/4] Generando migraciones iniciales limpias..." -ForegroundColor Yellow

Write-Host "  - Generando InitialIdentity..."
dotnet ef migrations add InitialIdentity --context SatHospitalarioIdentityDbContext --project src\SistemaSatHospitalario.Infrastructure --startup-project src\SistemaSatHospitalario.WebAPI --output-dir Identity/Migrations

Write-Host "  - Generando InitialApplication..."
dotnet ef migrations add InitialApplication --context SatHospitalarioDbContext --project src\SistemaSatHospitalario.Infrastructure --startup-project src\SistemaSatHospitalario.WebAPI --output-dir Persistence/Migrations

Write-Host "  - Generando InitialLegacy..."
dotnet ef migrations add InitialLegacy --context Sistema2020LegacyDbContext --project src\SistemaSatHospitalario.Infrastructure --startup-project src\SistemaSatHospitalario.WebAPI --output-dir Persistence/Legacy/Migrations

# 3. Aplicación a Base de Datos
Write-Host "[3/4] Aplicando cambios a MySQL (Requiere credenciales en appsettings.Development.json)..." -ForegroundColor Yellow

Write-Host "  - Actualizando Identity..."
dotnet ef database update --context SatHospitalarioIdentityDbContext --project src\SistemaSatHospitalario.Infrastructure --startup-project src\SistemaSatHospitalario.WebAPI

Write-Host "  - Actualizando Application..."
dotnet ef database update --context SatHospitalarioDbContext --project src\SistemaSatHospitalario.Infrastructure --startup-project src\SistemaSatHospitalario.WebAPI

Write-Host "  - Actualizando Legacy..."
dotnet ef database update --context Sistema2020LegacyDbContext --project src\SistemaSatHospitalario.Infrastructure --startup-project src\SistemaSatHospitalario.WebAPI

# 4. Verificación final
Write-Host "[4/4] Proceso completado. La base de datos está en estado Initial." -ForegroundColor Green
Write-Host "TIP: Revisa el log de la API al iniciar para ver los 'Insight Report Logs' detallados." -ForegroundColor Cyan
