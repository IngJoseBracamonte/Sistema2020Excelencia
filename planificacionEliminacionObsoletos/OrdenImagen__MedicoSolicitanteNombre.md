# Análisis: `OrdenImagen.MedicoSolicitanteNombre`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenImagen` |
| **Propiedad obsoleta** | `MedicoSolicitanteNombre` |
| **Reemplazo** | `MedicoSolicitante.Nombre` vía `MedicoSolicitanteId` |
| **Mensaje de obsolescencia** | "Usar MedicoSolicitante.Nombre vía MedicoSolicitanteId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/OrdenImagen.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `WebAPI/Controllers/Admision/ImagingController.cs` (líneas 70, 146, 371, 460, 555)

**Tipo de cambio:** Derivar el nombre del médico desde `MedicoSolicitanteId` (join con `MedicoSolicitante.Nombre`).

## 3. Impacto en Frontend

- Verificar si la API expone `medicoSolicitanteNombre`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenImagen.MedicoSolicitanteNombre`.
- [ ] 2. Verificar que `MedicoSolicitanteId` está poblado.
- [ ] 3. En los queries, hacer join con `MedicoSolicitante` para obtener `Nombre`.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `MedicoSolicitanteNombre`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenImagen.MedicoSolicitanteNombre`.
- Las órdenes de imagen muestran el médico solicitante correctamente.

## 6. Riesgos

- Requiere join con la tabla de médicos.
- Si `MedicoSolicitanteId` está vacío en datos legacy, el nombre no se podrá derivar.