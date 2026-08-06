# ==============================================================================
# Script de Generacion e Instalacion Automatica de Certificados SSL (Windows 10)
# Sistema Sat Hospitalario v2.0
# ==============================================================================

# Requiere ejecutar como Administrador para instalar el certificado en las Autoridades Raiz de Confianza
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ATENCION: Este script necesita permisos de Administrador para registrar el certificado en Windows." -ForegroundColor Yellow
    Write-Host "Por favor, abre PowerShell como Administrador y vuelve a ejecutar: .\generate-ssl.ps1`n" -ForegroundColor Yellow
}

Write-Host "Detectando configuracion de red de la maquina servidor..." -ForegroundColor Cyan

# 1. Deteccion de Nombre de Equipo (Hostname)
$hostname = $env:COMPUTERNAME
$hostnameLower = $hostname.ToLower()

# 2. Deteccion de Direccion IP IPv4 Activa
$activeIp = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { 
    $_.InterfaceAlias -notlike "*Loopback*" -and 
    $_.InterfaceAlias -notlike "*vEthernet*" -and 
    $_.InterfaceAlias -notlike "*WSL*" -and 
    $_.IPAddress -notlike "169.254*" 
} | Select-Object -First 1).IPAddress

if (-not $activeIp) {
    $activeIp = "127.0.0.1"
}

Write-Host "--------------------------------------------------------" -ForegroundColor Gray
Write-Host "   HostName Detectado: $hostname ($hostnameLower)" -ForegroundColor Green
Write-Host "   IP Servidor Detectada: $activeIp" -ForegroundColor Green
Write-Host "--------------------------------------------------------" -ForegroundColor Gray

# 3. Ubicacion del Ejecutable de OpenSSL
$opensslCmd = Get-Command openssl -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Path
if (-not $opensslCmd) {
    $gitOpenSsl = "C:\Program Files\Git\usr\bin\openssl.exe"
    if (Test-Path $gitOpenSsl) {
        $opensslCmd = $gitOpenSsl
    } else {
        Write-Host "Error: No se encontro OpenSSL en el sistema ni en Git for Windows." -ForegroundColor Red
        Write-Host "Instale Git para Windows o agregue OpenSSL al PATH." -ForegroundColor Yellow
        exit 1
    }
}
Write-Host "Utilizando OpenSSL en: $opensslCmd" -ForegroundColor Gray

# 4. Creacion del Directorio de Salida ssl/
$scriptPath = $PSScriptRoot
$sslDir = Join-Path $scriptPath "ssl"
if (-not (Test-Path $sslDir)) {
    New-Item -ItemType Directory -Path $sslDir | Out-Null
}

$cnfPath = Join-Path $sslDir "openssl.cnf"
$keyPath = Join-Path $sslDir "server.key"
$crtPath = Join-Path $sslDir "server.crt"

# 5. Generacion Dinamica del Archivo openssl.cnf con Subject Alternative Names (SAN)
$cnfContent = @"
[req]
default_bits       = 2048
distinguished_name = req_distinguished_name
req_extensions     = v3_req
prompt             = no

[req_distinguished_name]
C  = VE
ST = Distrito Capital
L  = Caracas
O  = Sistema Sat Hospitalario
OU = IT Infrastructure
CN = $hostname

[v3_req]
basicConstraints = CA:FALSE
keyUsage         = nonRepudiation, digitalSignature, keyEncipherment
extendedKeyUsage = serverAuth
subjectAltName   = @alt_names

[alt_names]
DNS.1 = localhost
DNS.2 = $hostname
DNS.3 = $hostnameLower
DNS.4 = $hostname.local
IP.1  = 127.0.0.1
IP.2  = $activeIp
"@

Set-Content -Path $cnfPath -Value $cnfContent -Encoding ASCII

Write-Host "`nGenerando Certificado SSL autofirmado de 10 anos (2048-bit RSA + SAN)..." -ForegroundColor Cyan
& $opensslCmd req -x509 -nodes -days 3650 -newkey rsa:2048 -keyout $keyPath -out $crtPath -config $cnfPath -extensions v3_req 2>$null

if ((Test-Path $keyPath) -and (Test-Path $crtPath)) {
    Write-Host "Certificados generados exitosamente en:" -ForegroundColor Green
    Write-Host "   Llave Privada: $keyPath" -ForegroundColor White
    Write-Host "   Certificado:   $crtPath" -ForegroundColor White
    
    # 6. Importar Certificado en el Almacen de Confianza de Windows (si se ejecuta como Admin)
    if ($isAdmin) {
        Write-Host "`nRegistrando certificado en Autoridades Raiz de Confianza de Windows..." -ForegroundColor Cyan
        try {
            Import-Certificate -FilePath $crtPath -CertStoreLocation Cert:\LocalMachine\Root | Out-Null
            Write-Host "Certificado instalado en Windows con exito. El navegador marcara el sitio como SEGURO." -ForegroundColor Green
        } catch {
            Write-Host "No se pudo instalar automaticamente en el almacen de Windows: $_" -ForegroundColor Yellow
        }
    }

    Write-Host "`n========================================================" -ForegroundColor Gray
    Write-Host " INSTRUCCIONES DE USO CON DOCKER COMPOSE:" -ForegroundColor White
    Write-Host "========================================================" -ForegroundColor Gray
    Write-Host "1. Los archivos se crearon en la carpeta .\ssl\" -ForegroundColor Yellow
    Write-Host "2. Tu Nginx y Docker responderan de forma segura a:" -ForegroundColor Yellow
    Write-Host "   - https://$activeIp" -ForegroundColor Cyan
    Write-Host "   - https://$hostnameLower" -ForegroundColor Cyan
    Write-Host "   - https://localhost" -ForegroundColor Cyan
    Write-Host "========================================================`n" -ForegroundColor Gray
} else {
    Write-Host "Ocurrio un error generando los archivos SSL." -ForegroundColor Red
}
