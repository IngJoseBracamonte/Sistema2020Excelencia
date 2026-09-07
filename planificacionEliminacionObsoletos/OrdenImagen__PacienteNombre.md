# Análisis: `OrdenImagen.PacienteNombre`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenImagen` |
| **Propiedad obsoleta** | `PacienteNombre` |
| **Reemplazo** | `Paciente.NombreCorto` vía `PacienteId` |
| **Mensaje de obsolescencia** | "Usar Paciente.NombreCorto vía PacienteId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/OrdenImagen.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Infrastructure/Integration/OrdenExternaService.cs` (líneas 49, 79)
- `WebAPI/Controllers/Admision/ImagingController.cs` (líneas 56, 118, 132, 174, 276, 308, 362, 390, 401-402, 412, 446, 625)

**Tipo de cambio:** Derivar el nombre del paciente desde `PacienteId` (join con `Paciente.NombreCorto`).

## 3. Impacto en Frontend

- Verificar si la API expone `pacienteNombre`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenImagen.PacienteNombre`.
- [ ] 2. Verificar que `PacienteId` está poblado.
- [ ] 3. En los queries, hacer join con `Paciente` para obtener `NombreCorto`.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `PacienteNombre`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenImagen.PacienteNombre`.
- Las órdenes de imagen muestran el paciente correctamente.

## 6. Riesgos

- Requiere join con la tabla de pacientes.
- Si `PacienteId` está vacío en datos legacy, el nombre no se podrá derivar.