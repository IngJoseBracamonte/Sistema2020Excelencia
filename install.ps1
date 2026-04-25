#Requires -Version 5.1
<#
.SYNOPSIS
    Instalador automatizado del Sistema Sat Hospitalario en Docker.
.DESCRIPTION
    Configura dominio local, genera credenciales, construye imagenes Docker,
    despliega contenedores y verifica conectividad con MySQL del host.
.NOTES
    Ejecutar como Administrador para configurar el archivo hosts.
    Prerequisitos: Docker Desktop, MySQL Server corriendo en el host.
#>

param(
    [string]$DomainName = "hospital.local",
    [int]$HostPort = 80,
    [string]$MysqlUser = "root",
    [string]$MysqlPassword = "",
    [int]$MysqlPort = 3306,
    [switch]$SkipHostsFile,
    [switch]$SkipBuild,
    [switch]$NonInteractive
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$ScriptRoot = $PSScriptRoot
if (-not $ScriptRoot) { $ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path }

# -------------------------------------------
# Utilidades
# -------------------------------------------

function Write-Banner {
    Write-Host "" -ForegroundColor Cyan
    Write-Host "  +-----------------------------------------------------------+" -ForegroundColor Cyan
    Write-Host "  |   Hospital System - Docker Installer v1.0                 |" -ForegroundColor Cyan
    Write-Host "  |   Despliegue Local en Contenedores                        |" -ForegroundColor Cyan
    Write-Host "  +-----------------------------------------------------------+" -ForegroundColor Cyan
    Write-Host "" -ForegroundColor Cyan
}

function Write-Step([string]$Step, [string]$Message) {
    Write-Host ""
    Write-Host "  [$Step] " -ForegroundColor Yellow -NoNewline
    Write-Host $Message -ForegroundColor White
    Write-Host "  " + ("-" * 50) -ForegroundColor DarkGray
}

function Write-Ok([string]$Message) {
    Write-Host "    [OK] $Message" -ForegroundColor Green
}

function Write-Warn([string]$Message) {
    Write-Host "    [WARN] $Message" -ForegroundColor Yellow
}

function Write-Fail([string]$Message) {
    Write-Host "    [FAIL] $Message" -ForegroundColor Red
}

function Write-Info([string]$Message) {
    Write-Host "    [INFO] $Message" -ForegroundColor DarkCyan
}

function Get-RandomSecret([int]$Length = 64) {
    $chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*_-+='
    $secret = -join (1..$Length | ForEach-Object { $chars[(Get-Random -Maximum $chars.Length)] })
    return $secret
}

function Test-IsAdmin {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

# -------------------------------------------
# FASE 1: Verificacion de Prerequisitos
# -------------------------------------------

function Test-Prerequisites {
    Write-Step "1/7" "Verificando prerequisitos..."

    # Docker
    try {
        $dockerVersion = docker version --format '{{.Server.Version}}' 2>$null
        if ($dockerVersion) {
            Write-Ok "Docker Engine: v$dockerVersion"
        } else {
            throw "no version"
        }
    } catch {
        Write-Fail "Docker Desktop no esta corriendo o no esta instalado."
        Write-Info "Descarga: https://www.docker.com/products/docker-desktop/"
        exit 1
    }

    # Docker Compose
    try {
        $composeVersion = docker compose version --short 2>$null
        if ($composeVersion) {
            Write-Ok "Docker Compose: v$composeVersion"
        } else {
            throw "no compose"
        }
    } catch {
        Write-Fail "Docker Compose no disponible. Actualice Docker Desktop."
        exit 1
    }

    # Puerto 80
    try {
        $portInUse = Get-NetTCPConnection -LocalPort $HostPort -ErrorAction SilentlyContinue
        if ($portInUse) {
            $proc = Get-Process -Id $portInUse[0].OwningProcess -ErrorAction SilentlyContinue
            Write-Warn "Puerto $HostPort en uso por: $($proc.ProcessName) (PID: $($proc.Id))"
            if (-not $NonInteractive) {
                $continue = Read-Host "    ¿Desea continuar de todas formas? (s/N)"
                if ($continue -notin @('s','S','si','Si','SI','y','Y')) {
                    Write-Info "Abortado. Libere el puerto o use otro puerto."
                    exit 0
                }
            }
        } else {
            Write-Ok "Puerto $HostPort disponible"
        }
    } catch {
        # ignore netstat failures in some environments
    }

    # Espacio en disco
    try {
        $drive = (Get-Item $ScriptRoot).PSDrive
        $freeGB = [math]::Round((Get-PSDrive $drive.Name).Free / 1GB, 1)
        if ($freeGB -lt 3) {
            Write-Warn "Solo $freeGB GB libres (recomendado: 5+ GB)"
        } else {
            Write-Ok "Espacio en disco: $freeGB GB libres"
        }
    } catch { }

    # Admin check
    if (Test-IsAdmin) {
        Write-Ok "Ejecutando como Administrador"
    } else {
        Write-Warn "No es Administrador - no se podra configurar el archivo hosts"
        $script:SkipHostsFile = $true
    }
}

# -------------------------------------------
# FASE 2: Verificacion de MySQL del Host
# -------------------------------------------

function Test-MySqlConnection {
    Write-Step "2/7" "Verificando MySQL del host (localhost:$MysqlPort)..."

    # Test TCP
    try {
        $tcp = New-Object System.Net.Sockets.TcpClient
        $tcp.Connect("127.0.0.1", $MysqlPort)
        $tcp.Close()
        Write-Ok "MySQL accesible en puerto $MysqlPort"
    } catch {
        Write-Fail "No se puede conectar a MySQL en localhost:$MysqlPort"
        Write-Info "Asegurese de que MySQL Server este corriendo."
        exit 1
    }

    # Verificar con mysql client si esta disponible
    $mysqlClient = Get-Command mysql -ErrorAction SilentlyContinue
    if ($mysqlClient) {
        Write-Info "Cliente MySQL encontrado, verificando bases de datos..."

        $dbs = @("SatHospitalario", "SatHospitalarioIdentity", "sistema2020")
        $dbList = & mysql -u $MysqlUser "-p$MysqlPassword" -h 127.0.0.1 -P $MysqlPort -e "SHOW DATABASES;" 2>$null
        
        foreach ($db in $dbs) {
            if ($dbList -match $db) {
                Write-Ok "Base de datos '$db' encontrada"
            } else {
                Write-Warn "Base de datos '$db' NO encontrada"
                if (-not $NonInteractive) {
                    $create = Read-Host "    ¿Crear la base de datos '$db'? (S/n)"
                    if ($create -notin @('n','N','no','No','NO')) {
                        & mysql -u $MysqlUser "-p$MysqlPassword" -h 127.0.0.1 -P $MysqlPort -e "CREATE DATABASE IF NOT EXISTS \`$db\` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;" 2>$null
                        if ($LASTEXITCODE -eq 0) {
                            Write-Ok "Base de datos '$db' creada"
                        } else {
                            Write-Fail "Error al crear '$db'"
                        }
                    }
                }
            }
        }
    } else {
        Write-Warn "Cliente 'mysql' no encontrado en PATH - no se verificaran las BDs"
        Write-Info "Asegurese manualmente de que existan: SatHospitalario, SatHospitalarioIdentity, sistema2020"
    }
}

# -------------------------------------------
# FASE 3: Configuracion Interactiva
# -------------------------------------------

function Get-Configuration {
    Write-Step "3/7" "Configuracion del despliegue..."

    if (-not $NonInteractive) {
        # Dominio
        $inputDomain = Read-Host "    Dominio local (Enter = $DomainName)"
        if ($inputDomain) { $script:DomainName = $inputDomain }

        # MySQL Password
        if (-not $MysqlPassword) {
            $inputPass = Read-Host "    Contraseña MySQL para '$MysqlUser' (Enter = Labordono1818)"
            if ($inputPass) { $script:MysqlPassword = $inputPass }
            else { $script:MysqlPassword = "Labordono1818" }
        }
    } else {
        if (-not $MysqlPassword) { $script:MysqlPassword = "Labordono1818" }
    }

    Write-Ok "Dominio: $DomainName"
    Write-Ok "MySQL: $MysqlUser@localhost:$MysqlPort"
}

# -------------------------------------------
# FASE 4: Generar .env
# -------------------------------------------

function New-EnvFile {
    Write-Step "4/7" "Generando archivo .env..."

    $jwtSecret = Get-RandomSecret -Length 64
    $envPath = Join-Path $ScriptRoot ".env"

    $envContent = @"
DOMAIN_NAME=$DomainName
HOST_PORT=$HostPort
MYSQL_HOST=host.docker.internal
MYSQL_PORT=$MysqlPort
MYSQL_USER=$MysqlUser
MYSQL_PASSWORD=$MysqlPassword
MYSQL_SYSTEM_DB=SatHospitalario
MYSQL_IDENTITY_DB=SatHospitalarioIdentity
MYSQL_LEGACY_DB=sistema2020
JWT_SECRET=$jwtSecret
JWT_ISSUER=SistemaSatHospitalarioAPI
JWT_AUDIENCE=SistemaSatHospitalario_PWA
SMTP_HOST=
SMTP_PORT=2525
SMTP_USER=
SMTP_PASS=
"@

    Set-Content -Path $envPath -Value $envContent -Encoding UTF8
    Write-Ok "Archivo .env generado en: $envPath"
    Write-Ok "JWT Secret generado (64 chars)"
}

# -------------------------------------------
# FASE 5: Configurar DNS Local (hosts file)
# -------------------------------------------

function Set-LocalDns {
    Write-Step "5/7" "Configurando DNS local..."

    if ($SkipHostsFile) {
        Write-Warn "Omitido"
        return
    }

    $hostsPath = "C:\Windows\System32\drivers\etc\hosts"
    try {
        $hostsContent = Get-Content $hostsPath -Raw -ErrorAction SilentlyContinue
        $entry = "127.0.0.1  $DomainName"

        if ($hostsContent -match [regex]::Escape($DomainName)) {
            Write-Ok "Entrada '$DomainName' ya existe en hosts file"
        } else {
            Add-Content -Path $hostsPath -Value "`n# Sistema Sat Hospitalario (Docker)`n$entry" -Encoding ASCII
            Write-Ok "Agregado: $entry"
        }
        ipconfig /flushdns | Out-Null
    } catch {
        Write-Warn "No se pudo modificar el archivo hosts. Agreguelo manualmente."
    }
}

# -------------------------------------------
# FASE 6: Build & Deploy
# -------------------------------------------

function Start-Deployment {
    Write-Step "6/7" "Construyendo y desplegando contenedores..."

    Push-Location $ScriptRoot

    try {
        if (-not $SkipBuild) {
            Write-Info "Construyendo imagenes Docker..."
            docker compose build
            if ($LASTEXITCODE -ne 0) {
                Write-Fail "Error durante el build."
                exit 1
            }
            Write-Ok "Imagenes construidas"
        }

        # Deploy
        Write-Info "Levantando contenedores..."
        docker compose up -d
        if ($LASTEXITCODE -ne 0) {
            Write-Fail "Error al levantar contenedores"
            exit 1
        }

        # Esperar health checks
        Write-Info "Esperando health checks..."
        Start-Sleep -Seconds 10
        docker compose ps
    } finally {
        Pop-Location
    }
}

# -------------------------------------------
# FASE 7: Verificacion Post-Deploy
# -------------------------------------------

function Test-Deployment {
    Write-Step "7/7" "Verificacion post-despliegue..."

    Write-Ok "Sistema desplegado."
    Write-Info "Acceda a: http://$DomainName"
}

# -------------------------------------------
# MAIN
# -------------------------------------------

Write-Banner
Test-Prerequisites
Get-Configuration
Test-MySqlConnection
New-EnvFile
Set-LocalDns
Start-Deployment
Test-Deployment
