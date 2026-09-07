# Análisis: `OrdenCirugia.UsuarioCreacion`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenCirugia` |
| **Propiedad obsoleta** | `UsuarioCreacion` |
| **Reemplazo** | `UsuarioCreacionId` |
| **Mensaje de obsolescencia** | "Usar UsuarioCreacionId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/OrdenCirugia.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetPacientesQuirurgicosListaQuery.cs` (línea 193)
- `Queries/Admision/GetOrdenesCirugiaQuery.cs` (líneas 151, 274)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1368)

**Tipo de cambio:** Migrar de `UsuarioCreacion` (string) a `UsuarioCreacionId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioCreacion`. Migrar a `usuarioCreacionId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenCirugia.UsuarioCreacion`.
- [ ] 2. Verificar que `UsuarioCreacionId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioCreacion` → `UsuarioCreacionId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioCreacion`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenCirugia.UsuarioCreacion`.
- Las órdenes de cirugía muestran el usuario de creación correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioCreacionId` para órdenes legacy.