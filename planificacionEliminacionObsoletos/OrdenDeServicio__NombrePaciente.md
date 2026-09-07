# Análisis: `OrdenDeServicio.NombrePaciente`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenDeServicio` |
| **Propiedad obsoleta** | `NombrePaciente` |
| **Reemplazo** | `Paciente.NombreCorto` vía `PacienteId` |
| **Mensaje de obsolescencia** | "Usar Paciente.NombreCorto vía PacienteId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/OrdenDeServicio.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- No aparece en las advertencias del build actual (posiblemente sin usos activos o usos no detectados).

**Tipo de cambio:** Derivar el nombre del paciente desde `PacienteId` (join con `Paciente.NombreCorto`).

## 3. Impacto en Frontend

- Verificar si la API expone `nombrePaciente`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenDeServicio.NombrePaciente` (grep).
- [ ] 2. Verificar que `PacienteId` está poblado.
- [ ] 3. En los queries, hacer join con `Paciente` para obtener `NombreCorto`.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `NombrePaciente`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenDeServicio.NombrePaciente`.
- Las órdenes de servicio muestran el paciente correctamente.

## 6. Riesgos

- Requiere join con la tabla de pacientes.
- Si `PacienteId` está vacío en datos legacy, el nombre no se podrá derivar.