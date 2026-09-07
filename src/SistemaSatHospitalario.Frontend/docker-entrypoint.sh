#!/bin/sh
set -e

# =========================================================
# Entrypoint — Sistema Sat Hospitalario Frontend
# Genera certificados SSL autofirmados si no existen y
# actualiza la configuracion de Nginx para apuntar a ellos.
#
# IMPORTANTE: Los certificados se generan en /etc/ssl/certs
# y /etc/ssl/private (directorios ESCRIBIBLES), NO en
# /etc/nginx/ssl que puede estar montado como volumen
# read-only desde el host (./ssl:/etc/nginx/ssl:ro).
# =========================================================

CERT_DIR="/etc/ssl/certs"
KEY_DIR="/etc/ssl/private"
CERT_FILE="$CERT_DIR/selfsigned.crt"
KEY_FILE="$KEY_DIR/selfsigned.key"

# Si existen certificados montados en /etc/nginx/ssl, usarlos
if [ -f "/etc/nginx/ssl/server.crt" ] && [ -f "/etc/nginx/ssl/server.key" ]; then
    echo "[Entrypoint] Certificados SSL encontrados en /etc/nginx/ssl (volumen host)."
    CERT_FILE="/etc/nginx/ssl/server.crt"
    KEY_FILE="/etc/nginx/ssl/server.key"
else
    # Generar autofirmados en directorios escribibles
    if [ ! -f "$CERT_FILE" ] || [ ! -f "$KEY_FILE" ]; then
        echo "[Entrypoint] Generando certificado SSL autofirmado (10 anos)..."
        mkdir -p "$CERT_DIR" "$KEY_DIR"
        openssl req -x509 -nodes -days 3650 -newkey rsa:2048 \
            -keyout "$KEY_FILE" -out "$CERT_FILE" \
            -subj "/CN=localhost/O=SistemaSatHospitalario/C=VE"
        echo "[Entrypoint] Certificado generado en $CERT_FILE"
    fi
fi

# Actualizar nginx.conf para apuntar a los certificados reales
# Usar sed con patron que tolera multiples espacios
echo "[Entrypoint] Actualizando configuracion de Nginx..."
sed -i "s|ssl_certificate[[:space:]]*/etc/nginx/ssl/server.crt;|ssl_certificate $CERT_FILE;|g" /etc/nginx/conf.d/default.conf
sed -i "s|ssl_certificate_key[[:space:]]*/etc/nginx/ssl/server.key;|ssl_certificate_key $KEY_FILE;|g" /etc/nginx/conf.d/default.conf

echo "[Entrypoint] Configuracion SSL lista. Iniciando Nginx..."
exec "$@"
