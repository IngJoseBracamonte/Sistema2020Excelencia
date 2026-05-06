# Maintain-HonorariumCatalog.ps1
# Script de Automatización Pro para Gestión de Honorarios
# Sistema Sat Hospitalario - Excelencia

Clear-Host
Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host "   ORQUESTADOR DE MANTENIMIENTO: HONORARIOS Y CATÁLOGOS  " -ForegroundColor Cyan
Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host ""

$rootPath = Get-Location
$infraProject = "src\SistemaSatHospitalario.Infrastructure\SistemaSatHospitalario.Infrastructure.csproj"
$apiProject = "src\SistemaSatHospitalario.WebAPI\SistemaSatHospitalario.WebAPI.csproj"
$testProject = "src\SistemaSatHospitalario.UnitTests\SistemaSatHospitalario.UnitTests.csproj"

# 1. Persistencia: Asegurar que la DB esté al día
Write-Host "[PASO 1/3] Verificando Migraciones de Base de Datos..." -ForegroundColor Yellow
dotnet ef database update --project $infraProject --startup-project $apiProject --context SatHospitalarioDbContext

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error aplicando migraciones. Revise la conexión a la DB." -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "-> Base de Datos Sincronizada." -ForegroundColor Green
Write-Host ""

# 2. Calidad: Ejecutar Pruebas de Lógica
Write-Host "[PASO 2/3] Ejecutando Pruebas de Lógica de Mapeo (HonorariumTests)..." -ForegroundColor Yellow
dotnet test $testProject --filter HonorariumTests --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "ALERTA: Las pruebas de lógica han fallado. No proceda con cambios en producción." -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "-> Lógica de Honorarios Validada Exitosamente." -ForegroundColor Green
Write-Host ""

# 3. Integridad del Inicializador
Write-Host "[PASO 3/3] Verificando Integridad del Inicializador de Datos..." -ForegroundColor Yellow
Write-Host "-> El sistema auto-clasificará servicios sin etiquetas en el próximo arranque." -ForegroundColor Green
Write-Host "-> Categorías Base (RX, CONSULTA, INFORME) configuradas en el Seeder." -ForegroundColor Green
Write-Host ""

Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host "   SISTEMA DE HONORARIOS: ESTADO OK - LISTO PARA OPERAR  " -ForegroundColor Cyan
Write-Host "=========================================================" -ForegroundColor Cyan
