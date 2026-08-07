#!/bin/sh
set -e

# ═══════════════════════════════════════════════════════════
# entrypoint.sh — Auto-generación SSL & Arranque Nginx
# ═══════════════════════════════════════════════════════════

SSL_DIR="/etc/nginx/ssl"
MOUNTED_CRT="${SSL_DIR}/server.crt"
MOUNTED_KEY="${SSL_DIR}/server.key"

INTERNAL_SSL_DIR="/etc/ssl/certs"
INTERNAL_CRT="${INTERNAL_SSL_DIR}/selfsigned.crt"
INTERNAL_KEY="${INTERNAL_SSL_DIR}/selfsigned.key"
CONF_FILE="/etc/nginx/conf.d/default.conf"

if [ -s "$MOUNTED_CRT" ] && [ -s "$MOUNTED_KEY" ]; then
    echo "=> [SSL] Usando certificados provistos en ${SSL_DIR}"
else
    echo "=> [SSL] Certificados no encontrados en ${SSL_DIR}. Generando certificados autofirmados en ${INTERNAL_SSL_DIR}..."
    mkdir -p "$INTERNAL_SSL_DIR"
    openssl req -x509 -nodes -days 3650 -newkey rsa:2048 \
        -keyout "$INTERNAL_KEY" \
        -out "$INTERNAL_CRT" \
        -subj "/C=VE/ST=DistritoCapital/L=Caracas/O=SatHospitalario/CN=localhost" \
        2>/dev/null

    echo "=> [SSL] Certificado autofirmado (2048 bits, 10 años) generado exitosamente."

    if [ -f "$CONF_FILE" ]; then
        echo "=> [SSL] Ajustando Nginx conf para apuntar a certificados internos..."
        sed -i "s|/etc/nginx/ssl/server.crt|${INTERNAL_CRT}|g" "$CONF_FILE"
        sed -i "s|/etc/nginx/ssl/server.key|${INTERNAL_KEY}|g" "$CONF_FILE"
    fi
fi

echo "=> [SAT-FRONTEND] Iniciando Nginx en primer plano..."
exec nginx -g "daemon off;"
