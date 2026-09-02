# Plan Maestro de Normalización 3FN y Refactorización del Dominio Base

**Proyecto**: SistemaSatHospitalario (.NET 9 + Clean Architecture + MySQL + Angular 19+)  
**Enfoque**: DB-Driven, Integridad Referencial 3FN, TDD Primero, Cero cadenas libres para estados/catálogos, Preservación intacta del Legacy (`sistema2020`).

---

## 📋 Registro de Tareas de Refactorización

### TAREA 1: Normalización de Estados de `CitasMedicas` (`EstadosCitaMedica`)
- **Problema**: El campo `Estado` en `CitaMedica` se almacena como texto libre (`varchar`/`longtext`), corriendo riesgo de inconsistencias tipográficas e impidiendo indexación y validación relacional óptima.
- **Acciones Arquitectónicas**:
  1. **Base de Datos / Migración**:
     - Crear tabla catálogo `EstadosCitaMedica` (`Id INT PRIMARY KEY`, `Codigo VARCHAR(50) UNIQUE`, `Nombre VARCHAR(100)`, `Activo TINYINT(1) DEFAULT 1`).
     - Poblar valores iniciales:
       - `1` - `PENDIENTE` - *"Pendiente"*
       - `2` - `CONFIRMADA` - *"Confirmada"*
       - `3` - `ATENDIDA` - *"Atendida"*
       - `4` - `CANCELADA` - *"Cancelada"*
     - Agregar columna `EstadoId INT NOT NULL` en `CitasMedicas` con FK hacia `EstadosCitaMedica(Id)`.
  2. **Capa Dominio (`Core.Domain`)**:
     - Crear entidad `EstadoCitaMedica.cs` (Catálogo maestro).
     - Crear clase de constantes `EstadoCitaConstants.cs`:
       ```csharp
       public static class EstadoCitaConstants
       {
           public const int PendienteId = 1;
           public const int ConfirmadaId = 2;
           public const int AtendidaId = 3;
           public const int CanceladaId = 4;
       }
       ```
     - Modificar `CitaMedica.cs`:
       - Reemplazar `public string Estado { get; }` por `public int EstadoId { get; private set; }`.
       - Agregar propiedad de navegación `public virtual EstadoCitaMedica Estado { get; private set; } = null!;`.
  3. **Capa Infraestructura (`Core.Infrastructure`)**:
     - Configurar entidad `EstadoCitaMedica` y relación 1:N en `SatHospitalarioDbContext.cs`.
     - Actualizar auto-reparación en `SystemDbInitializer.cs`.
  4. **Capa Aplicación & Frontend**:
     - Adaptar DTOs y consultas (`GetActiveAppointmentsQuery`, etc.) proyectando `EstadoId` y `EstadoNombre`.
     - Actualizar interfaces y componentes en Angular vinculados a `estadoId`.

---

### TAREA 2: Dependencia Transitiva en `CirugiasObservacionesHistorial`
- **Problema**: Coexisten `UsuarioRegistro` (string) y `UsuarioRegistroId`, generando dependencia transitiva y redundancia de datos de usuario.
- **Acciones Arquitectónicas**:
  1. **Dominio**: Eliminar la propiedad persistida `UsuarioRegistro` (string) en `CirugiaObservacionHistorial.cs` y asegurar `public Guid UsuarioRegistroId { get; private set; }`.
  2. **Infraestructura**: Configurar mapeo y FK en `CirugiasObservacionesHistorial`.
  3. **Aplicación / DTOs**: Exponer `UsuarioRegistroNombre` proyectado dinámicamente en consultas CQRS.
  4. **Frontend**: Enlazar la vista de historial de observaciones a `usuarioRegistroNombre`.

---

### TAREA 3: Desnormalización de Auditoría en `CuentasPorCobrar` y `CompromisosPago`
- **Problema**: `UsuarioAuditoria` y `UsuarioCreacion` guardan texto plano en lugar de FKs relacionales a usuarios.
- **Acciones Arquitectónicas**:
  1. **Dominio**:
     - En `CuentaPorCobrar.cs`: Reemplazar `string? UsuarioAuditoria` por `Guid? UsuarioAuditoriaId` y agregar `Guid? UsuarioCreacionId`.
     - En `CompromisoPago.cs`: Reemplazar `string UsuarioCreacion` por `Guid UsuarioCreacionId`.
  2. **Infraestructura**: Mapear FKs relacionales y ajustar interceptores de auditoría / servicios (`ICurrentUserService`).
  3. **Aplicación / DTOs**: Proyectar nombres de usuario en DTOs de salida.

---

### TAREA 4: Catálogo de Motivos de Autorización / Omisión (`MotivosAutorizacion`)
- **Problema**: Observaciones repetitivas de autorización (ej. *"Autorizado por Dirección Médica"*) guardadas como cadenas duplicadas en omisiones de compromisos de pago.
- **Acciones Arquitectónicas**:
  1. **Base de Datos / Migración**:
     - Crear tabla catálogo `MotivosAutorizacion` (`Id INT AUTO_INCREMENT PRIMARY KEY`, `Nombre VARCHAR(150)`, `Activo TINYINT(1) DEFAULT 1`).
     - Sembrar registros por defecto.
  2. **Dominio**:
     - Crear entidad `MotivoAutorizacion.cs`.
     - En `CompromisoPago.cs`, agregar `public int? MotivoAutorizacionId { get; private set; }` y su navegación `public virtual MotivoAutorizacion? MotivoAutorizacion { get; private set; }`.
  3. **Aplicación**: Endpoint/Query `GetMotivosAutorizacionQuery` para dropdowns DB-Driven.
  4. **Frontend**: Selector dinámico en diálogo de compromisos de pago.

---

### TAREA 5: Catálogo Relacional de Unidades de Medida (`UnidadesMedida`)
- **Problema**: Las unidades de medida se manejan mediante un Enum rígido en C# (`UnidadMedida`) o cadenas en servicios clínicos, impidiendo una administración dinámica DB-Driven desde base de datos y violando la regla de integridad referencial para insumos y recetas.
- **Acciones Arquitectónicas**:
  1. **Base de Datos / Migración**:
     - Crear tabla catálogo `UnidadesMedida` (`Id INT PRIMARY KEY`, `Codigo VARCHAR(20) UNIQUE`, `Nombre VARCHAR(100)`, `Simbolo VARCHAR(20)`, `EsFraccionable TINYINT(1) DEFAULT 1`, `Activo TINYINT(1) DEFAULT 1`).
     - Sembrar registros estándar:
       - `1` - `UNIDAD` - *"Unidad"* (`UND`)
       - `2` - `KG` - *"Kilogramo"* (`kg`)
       - `3` - `G` - *"Gramo"* (`g`)
       - `4` - `DG` - *"Decigramo"* (`dg`)
       - `5` - `MG` - *"Miligramo"* (`mg`)
       - `6` - `L` - *"Litro"* (`L`)
       - `7` - `ML` - *"Mililitro"* (`mL`)
     - En `Insumos`: Asegurar FK `UnidadMedidaId INT NOT NULL` referenciando a `UnidadesMedida(Id)`.
     - En `ServiciosInsumoRecetas`: Asegurar FK `UnidadMedidaConsumoId INT NOT NULL` referenciando a `UnidadesMedida(Id)`.
     - En `MovimientosInsumos`: Asegurar FK `UnidadMedidaOriginalId INT NOT NULL` referenciando a `UnidadesMedida(Id)`.
  2. **Capa Dominio (`Core.Domain`)**:
     - Crear entidad `UnidadMedidaCatalogo.cs` (o `UnidadMedida.cs` como entidad maestra `BaseEntity`).
     - Crear clase de constantes `UnidadMedidaConstants.cs`:
       ```csharp
       public static class UnidadMedidaConstants
       {
           public const int UnidadId = 1;
           public const int KgId = 2;
           public const int GramoId = 3;
           public const int DecigramoId = 4;
           public const int MiligramoId = 5;
           public const int LitroId = 6;
           public const int MililitroId = 7;
       }
       ```
     - En `Insumo.cs`: Reemplazar la propiedad enum por `public int UnidadMedidaId { get; private set; }` y su navegación `public virtual UnidadMedida UnidadMedida { get; private set; } = null!;`.
     - En `ServicioInsumoReceta.cs` y `MovimientoInsumo.cs`: Relacionar a `UnidadMedidaId` como FK.
  3. **Capa Infraestructura (`Core.Infrastructure`)**:
     - Mapear `UnidadMedida` en `SatHospitalarioDbContext.cs` con sus relaciones y restricciones de integridad.
     - Auto-sanar tabla y semillas en `SystemDbInitializer.cs`.
  4. **Capa Aplicación & Frontend**:
     - Crear Query `GetUnidadesMedidaQuery` para llenar combos dinámicos en inventario y recetas.
     - Actualizar formularios de creación/edición de insumos y recetas en Angular consumiendo la API de unidades.

---

### TAREA 6: Servicio Central de Catálogos Cacheados con Invalidación Reactiva (`ICatalogLookupService`)
- **Problema**: Los catálogos maestros y tablas de estados (unidades de medida, estados de citas, motivos de autorización, categorías, etc.) son consultados de forma intensiva en cada transacción. Mantener Enums fijos en código impide la extensibilidad dinámica, pero consultar la base de datos en cada validación genera sobrecarga.
- **Acciones Arquitectónicas (Patrón Cache-Aside + Event Eviction)**:
  1. **Abstracción (`Core.Application / Core.Domain`)**:
     - Crear interfaz `ICatalogLookupService`:
       ```csharp
       public interface ICatalogLookupService
       {
           Task<IReadOnlyList<UnidadMedidaDto>> GetUnidadesMedidaAsync(CancellationToken ct = default);
           Task<IReadOnlyList<EstadoCitaMedicaDto>> GetEstadosCitaAsync(CancellationToken ct = default);
           Task<IReadOnlyList<MotivoAutorizacionDto>> GetMotivosAutorizacionAsync(CancellationToken ct = default);
           Task<bool> EsUnidadFraccionableAsync(int unidadMedidaId, CancellationToken ct = default);
           void Invalidate(string catalogKey);
           void InvalidateAll();
       }
       ```
  2. **Implementación con `IMemoryCache` (`Infrastructure.Services`)**:
     - Implementar `CatalogLookupService` utilizando `IMemoryCache` con tiempo de expiración deslizable (sliding expiration) y absoluto de seguridad.
     - Centralizar claves de caché (`"CATALOG_UNIDADES_MEDIDA"`, `"CATALOG_ESTADOS_CITA"`, etc.).
  3. **Invalidación Automática Reactiva (Eviction on Command)**:
     - En cada Command que cree, modifique o desactive un registro en una tabla catálogo (ej. `CrearUnidadMedidaCommand`, `ActualizarMotivoAutorizacionCommand`):
       - Llamar a `_catalogLookupService.Invalidate("CATALOG_...")` al guardar cambios exitosamente.
       - Alternativamente, usar un `IDomainEvent` (`CatalogChangedEvent`) despachado automáticamente en el `SaveChangesAsync` de `SatHospitalarioDbContext` para purgar la caché correspondiente sin intervención manual.
  4. **Sincronización en Frontend (Angular Signals)**:
     - En el frontend, los servicios Angular (`CatalogoService`, `ReceivablesService`, `InventarioService`) expondrán Signals cacheados (`unidadesMedida = signal<UnidadMedida[]>([])`).
     - Al emitir mutaciones administrativas sobre catálogos, el frontend recargará el Signal dinámicamente refrescando todos los dropdowns activos en la UI de inmediato.

---

## ⏳ Tareas Adicionales por Evaluar y Agregar (En Proceso de Review)
*(Se irán detallando y versionando a medida que indiques los siguientes cambios en las entidades de dominio base)*


