# Análisis: `DetalleServicioCuenta.UsuarioTecnico`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `DetalleServicioCuenta` |
| **Propiedad obsoleta** | `UsuarioTecnico` |
| **Reemplazo** | `UsuarioTecnicoId` |
| **Mensaje de obsolescencia** | "Usar UsuarioTecnicoId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DetalleServicioCuenta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/ExportGenericListQuery.cs` (línea 149)

**Tipo de cambio:** Migrar de `UsuarioTecnico` (string) a `UsuarioTecnicoId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioTecnico`. Migrar a `usuarioTecnicoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `DetalleServicioCuenta.UsuarioTecnico`.
- [ ] 2. Verificar que `UsuarioTecnicoId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioTecnico` → `UsuarioTecnicoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioTecnico`.

## 5. Verificación

- `dotnet build` sin CS0618 para `DetalleServicioCuenta.UsuarioTecnico`.
- El reporte de servicios muestra el técnico correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioTecnicoId` para servicios legacy.