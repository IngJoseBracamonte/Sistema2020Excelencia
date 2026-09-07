# Análisis: `HistorialModificacionCuenta.ConvenioNuevoNombre`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `HistorialModificacionCuenta` |
| **Propiedad obsoleta** | `ConvenioNuevoNombre` |
| **Reemplazo** | Derivar del convenio vía `ConvenioNuevoId` |
| **Mensaje de obsolescencia** | "Derivar del convenio vía ConvenioNuevoId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/HistorialModificacionCuenta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetHistorialModificacionesQuery.cs` (línea 81)

**Tipo de cambio:** Derivar el nombre del convenio desde `ConvenioNuevoId` en lugar de almacenarlo.

## 3. Impacto en Frontend

- Verificar si la API expone `convenioNuevoNombre`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `HistorialModificacionCuenta.ConvenioNuevoNombre`.
- [ ] 2. Verificar que `ConvenioNuevoId` está poblado.
- [ ] 3. En los queries, hacer join con el convenio para obtener el nombre.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `ConvenioNuevoNombre`.

## 5. Verificación

- `dotnet build` sin CS0618 para `HistorialModificacionCuenta.ConvenioNuevoNombre`.
- El historial de modificaciones muestra el convenio nuevo correctamente.

## 6. Riesgos

- Requiere join con la tabla de convenios.
- Si `ConvenioNuevoId` está vacío en datos legacy, el nombre no se podrá derivar.