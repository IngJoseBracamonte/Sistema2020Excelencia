# Análisis: `SolicitudInsumoCirugia.UsuarioDespacho`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `SolicitudInsumoCirugia` |
| **Propiedad obsoleta** | `UsuarioDespacho` |
| **Reemplazo** | `UsuarioDespachoId` |
| **Mensaje de obsolescencia** | "Usar UsuarioDespachoId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/SolicitudInsumoCirugia.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (líneas 1462, 1485)

**Tipo de cambio:** Migrar de `UsuarioDespacho` (string) a `UsuarioDespachoId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioDespacho`. Migrar a `usuarioDespachoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `SolicitudInsumoCirugia.UsuarioDespacho`.
- [ ] 2. Verificar que `UsuarioDespachoId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioDespacho` → `UsuarioDespachoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioDespacho`.

## 5. Verificación

- `dotnet build` sin CS0618 para `SolicitudInsumoCirugia.UsuarioDespacho`.
- Las solicitudes de insumos de cirugía muestran el usuario de despacho correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioDespachoId` para solicitudes legacy.