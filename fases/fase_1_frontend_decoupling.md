# Fase 1: Desacoplamiento de Frontend (Auth & Storage)

**Objetivo**: Profesionalizar el manejo de estado y persistencia local en Angular para eliminar la dependencia directa de `localStorage` y mejorar la seguridad.

## Implementaciones Clave
1.  **StorageService Wrapper**:
    - Crear un servicio centralizado que gestione el acceso a `localStorage/sessionStorage`.
    - Implementar serialización/deserialización segura y tipado fuerte para las claves del sistema.
2.  **Abstracción de Seguridad**:
    - Centralización de los roles en un `Enum` TypeScript para evitar strings hardcoded (`isAdministrador`, `isCajero`).
    - Preparación del interceptor de autenticación para soportar lógica de `Refresh Token`.
3.  **Estado Reactivo con Signals**:
    - Refactorizar `AuthService` para que el estado de sesión fluya de manera reactiva y protegida, evitando mutaciones directas fuera del servicio.

## Beneficios Arquitectónicos
- **Mantenibilidad**: Cambiar la estrategia de almacenamiento (ej. a IndexedDB) solo requerirá modificar un archivo.
- **Robustez**: Reducción de errores por typos en nombres de roles o claves de almacenamiento.

---
**Senior Strategy**: "No confíes en el navegador; envuelve sus APIs en contratos que tú controles."
