# Análisis: `DetalleServicioCuenta.CategoriaHonorario`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `DetalleServicioCuenta` |
| **Propiedad obsoleta** | `CategoriaHonorario` |
| **Reemplazo** | Derivar de `TipoServicioId / MedicoResponsable` |
| **Mensaje de obsolescencia** | "Derivar de TipoServicioId / MedicoResponsable. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DetalleServicioCuenta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admin/GetServiciosSinAsignarQuery.cs` (línea 74)
- `Queries/Admin/GetDoctorHonorariumSummaryQuery.cs` (líneas 50, 62, 77, 90)

**Tipo de cambio:** Derivar la categoría de honorario desde `TipoServicioId` y `MedicoResponsable` en lugar de almacenarla.

## 3. Impacto en Frontend

- Verificar si la API expone `categoriaHonorario`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `DetalleServicioCuenta.CategoriaHonorario`.
- [ ] 2. Definir la lógica de derivación desde `TipoServicioId` y `MedicoResponsable`.
- [ ] 3. Reemplazar las lecturas por la lógica de derivación.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `CategoriaHonorario`.

## 5. Verificación

- `dotnet build` sin CS0618 para `DetalleServicioCuenta.CategoriaHonorario`.
- El resumen de honorarios de médicos es correcto.

## 6. Riesgos

- La lógica de derivación debe replicar exactamente la categoría almacenada.
- Requiere validar con datos reales que la derivación coincide.