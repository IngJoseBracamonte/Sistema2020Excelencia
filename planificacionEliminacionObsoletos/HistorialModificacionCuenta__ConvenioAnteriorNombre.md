# Análisis: `HistorialModificacionCuenta.ConvenioAnteriorNombre`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `HistorialModificacionCuenta` |
| **Propiedad obsoleta** | `ConvenioAnteriorNombre` |
| **Reemplazo** | Derivar del convenio vía `ConvenioAnteriorId` |
| **Mensaje de obsolescencia** | "Derivar del convenio vía ConvenioAnteriorId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/HistorialModificacionCuenta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetHistorialModificacionesQuery.cs` (línea 79)

**Tipo de cambio:** Derivar el nombre del convenio desde `ConvenioAnteriorId` en lugar de almacenarlo.

## 3. Impacto en Frontend

- Verificar si la API expone `convenioAnteriorNombre`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `HistorialModificacionCuenta.ConvenioAnteriorNombre`.
- [ ] 2. Verificar que `ConvenioAnteriorId` está poblado.
- [ ] 3. En los queries, hacer join con el convenio para obtener el nombre.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `ConvenioAnteriorNombre`.

## 5. Verificación

- `dotnet build` sin CS0618 para `HistorialModificacionCuenta.ConvenioAnteriorNombre`.
- El historial de modificaciones muestra el convenio anterior correctamente.

## 6. Riesgos

- Requiere join con la tabla de convenios.
- Si `ConvenioAnteriorId` está vacío en datos legacy, el nombre no se podrá derivar.