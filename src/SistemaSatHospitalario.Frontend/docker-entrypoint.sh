#!/bin/sh
set -e

# Certificados SSL opcionales o montados en el volumen host /etc/nginx/ssl
SSL_CERT="/etc/nginx/ssl/server.crt"
SSL_KEY="/etc/nginx/ssl/server.key"

# Si los certificados no existen en el volumen montado, generar autofirmados en la capa escribible
if [ ! -f "$SSL_CERT" ] || [ ! -f "$SSL_KEY" ]; then
    echo "[Entrypoint] Certificados SSL no encontrados en /etc/nginx/ssl/. Generando autofirmado temporal (10 años)..."
    mkdir -p /etc/ssl/certs /etc/ssl/private
    SSL_CERT="/etc/ssl/certs/selfsigned.crt"
    SSL_KEY="/etc/ssl/private/selfsigned.key"

    if [ ! -f "$SSL_CERT" ]; then
        openssl req -x509 -nodes -days 3650 -newkey rsa:2048 \
            -keyout "$SSL_KEY" -out "$SSL_CERT" \
            -subj "/CN=localhost/O=SistemaSatHospitalario/C=VE"
    fi

    echo "[Entrypoint] Actualizando configuración de Nginx con certs en /etc/ssl/..."
    sed -i "s|ssl_certificate /etc/nginx/ssl/server.crt;|ssl_certificate $SSL_CERT;|g" /etc/nginx/conf.d/default.conf
    sed -i "s|ssl_certificate_key /etc/nginx/ssl/server.key;|ssl_certificate_key $SSL_KEY;|g" /etc/nginx/conf.d/default.conf
fi

exec "$@"
