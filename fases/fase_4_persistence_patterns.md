# Fase 4: Patrones de Persistencia (Repos & Context)

**Objetivo**: Optimizar la capa de infraestructura para mejorar el rendimiento y desacoplar la lógica de base de datos del núcleo de la aplicación.

## Implementaciones Clave
1.  **Consolidación de Repositorios**:
    - Refactorizar `IBillingRepository` para usar patrones de carga ansiosa (Eager Loading) controlada, evitando el exceso de llamadas a la base de datos.
2.  **Abstracción de Contexto**:
    - Asegurar que `IApplicationDbContext` solo exponga lo necesario, evitando que los Handlers utilicen métodos internos de EF Core que comprometan el desacoplamiento.
3.  **Optimización de Consultas**:
    - Implementar proyecciones directas (`.Select()`) a DTOs en las Queries para evitar cargar entidades pesadas en memoria cuando solo se requiere visualización.

## Beneficios Arquitectónicos
- **Rendimiento**: Reducción significativa de la latencia en operaciones de lectura masiva.
- **Mantenibilidad**: Centralización del acceso a datos, facilitando migraciones futuras o cambios en el motor de base de datos.

---
**Senior Strategy**: "La base de datos es un detalle; el repositorio es el contrato que protege tu negocio."
