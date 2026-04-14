# Fase 3: Escalabilidad y Rendimiento (Edge Computing)

**Objetivo**: Optimizar la latencia y la capacidad de respuesta del sistema bajo carga distribuida.

## Implementaciones Clave
1.  **Caché Distribuido (Redis)**:
    - Uso de **Aiven for Redis** para persistencia de sesiones y metadatos de dashboard.
    - Estrategia de invalidación de caché para Tasa de Cambio y configuraciones globales.
2.  **Optimización Estática (Netlify Edge)**:
    - Despliegue de assets de Angular en la red de borde (CDN) para carga instantánea.
    - Implementación de Brotli/Gzip para compresión de payloads.
3.  **Procesamiento Asíncrono**:
    - Descarga de generación de reportes PDF pesados a "Background Tasks" para liberar el thread de la API.

## Beneficios Arquitectónicos
- **Reducción de Latencia**: Menor tiempo de carga para el usuario final (Time to First Byte).
- **Escalabilidad Elástica**: Preparación para picos de tráfico sin degradar la base de datos MySQL.
- **Eficiencia de Recursos**: Mejor aprovechamiento del cómputo en Render.

---
**Senior Strategy**: "Escalar no es añadir servidores, es quitar fricción."
