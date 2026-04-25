# ═══════════════════════════════════════════════════════════
# Dockerfile — Sistema Sat Hospitalario Frontend (Angular 19)
# Multi-stage: Node 22 (build) → Nginx Alpine (serve)
# ═══════════════════════════════════════════════════════════

# Stage 1: Build Angular
FROM node:22-alpine AS build
WORKDIR /app

# Copiar package files e instalar dependencias (cache layer)
COPY package*.json ./
RUN npm install --prefer-offline --no-audit --legacy-peer-deps

# Copiar código fuente y compilar
COPY . .
RUN npm run build -- --configuration docker

# Stage 2: Serve con Nginx
FROM nginx:alpine AS final

LABEL maintainer="SistemaSatHospitalario" \
      description="Hospital System Frontend PWA" \
      version="1.0"

# Instalar curl para healthcheck
RUN apk add --no-cache curl

# Copiar el build de Angular al directorio de Nginx
COPY --from=build /app/dist/sistema-sat-hospitalario.frontend/browser /usr/share/nginx/html

# Copiar configuración Nginx personalizada (proxy a API + SPA routing)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

# Health check nativo Docker
HEALTHCHECK --interval=30s --timeout=5s --retries=3 --start-period=10s \
    CMD curl -f http://localhost:80/ || exit 1

CMD ["nginx", "-g", "daemon off;"]
