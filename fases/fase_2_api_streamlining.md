# Fase 2: Simplificación de API (Security & Controllers)

**Objetivo**: Transformar los controladores de "mapeadores manuales" a "puertas de enlace delgadas" siguiendo el principio de Responsabilidad Única.

## Implementaciones Clave
1.  **Refactorización de Controladores**:
    - División de controladores masivos (ej. `BillingController`) en controladores específicos por subdominio.
    - Eliminación de la lógica de "enriquecimiento manual" de comandos; los controladores solo inyectan el `UserId` desde el token.
2.  **Extensiones de Claims**:
    - Implementar métodos de extensión para `ClaimsPrincipal` que permitan recuperar datos del usuario de forma tipada y centralizada.
3.  **Matriz de Seguridad**:
    - Sustituir decoradores `[Authorize(Roles = "...")]` por políticas basadas en requerimientos (`AuthorizationPolicy`), facilitando la gestión centralizada de permisos.

## Beneficios Arquitectónicos
- **Modularidad**: Facilita la navegación del código y las pruebas unitarias de los controladores.
- **Seguridad**: Reduce el riesgo de errores al extraer datos de identidad de múltiples formas.

---
**Senior Strategy**: "Un controlador es un cartero; su única misión es entregar el mensaje al destinatario correcto."
