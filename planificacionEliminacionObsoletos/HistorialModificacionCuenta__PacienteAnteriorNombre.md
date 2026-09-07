# Análisis: `HistorialModificacionCuenta.PacienteAnteriorNombre`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `HistorialModificacionCuenta` |
| **Propiedad obsoleta** | `PacienteAnteriorNombre` |
| **Reemplazo** | Derivar del paciente vía `PacienteAnteriorId` |
| **Mensaje de obsolescencia** | "Derivar del paciente vía PacienteAnteriorId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/HistorialModificacionCuenta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetHistorialModificacionesQuery.cs` (línea 73)

**Tipo de cambio:** Derivar el nombre del paciente desde `PacienteAnteriorId` en lugar de almacenarlo.

## 3. Impacto en Frontend

- Verificar si la API expone `pacienteAnteriorNombre`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `HistorialModificacionCuenta.PacienteAnteriorNombre`.
- [ ] 2. Verificar que `PacienteAnteriorId` está poblado.
- [ ] 3. En los queries, hacer join con el paciente para obtener el nombre.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `PacienteAnteriorNombre`.

## 5. Verificación

- `dotnet build` sin CS0618 para `HistorialModificacionCuenta.PacienteAnteriorNombre`.
- El historial de modificaciones muestra el paciente anterior correctamente.

## 6. Riesgos

- Requiere join con la tabla de pacientes.
- Si `PacienteAnteriorId` está vacío en datos legacy, el nombre no se podrá derivar.