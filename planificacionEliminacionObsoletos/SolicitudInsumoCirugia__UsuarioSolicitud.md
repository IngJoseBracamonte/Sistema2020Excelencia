# Análisis: `SolicitudInsumoCirugia.UsuarioSolicitud`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `SolicitudInsumoCirugia` |
| **Propiedad obsoleta** | `UsuarioSolicitud` |
| **Reemplazo** | `UsuarioSolicitudId` |
| **Mensaje de obsolescencia** | "Usar UsuarioSolicitudId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/SolicitudInsumoCirugia.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetPedidosInterSedePendientesQueryHandler.cs` (línea 69)
- `Queries/Admision/GetPacientesQuirurgicosListaQuery.cs` (línea 249)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1461)

**Tipo de cambio:** Migrar de `UsuarioSolicitud` (string) a `UsuarioSolicitudId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioSolicitud`. Migrar a `usuarioSolicitudId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `SolicitudInsumoCirugia.UsuarioSolicitud`.
- [ ] 2. Verificar que `UsuarioSolicitudId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioSolicitud` → `UsuarioSolicitudId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioSolicitud`.

## 5. Verificación

- `dotnet build` sin CS0618 para `SolicitudInsumoCirugia.UsuarioSolicitud`.
- Las solicitudes de insumos de cirugía muestran el usuario solicitante correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioSolicitudId` para solicitudes legacy.