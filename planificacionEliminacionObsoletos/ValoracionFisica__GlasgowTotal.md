# Análisis: `ValoracionFisica.GlasgowTotal`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `ValoracionFisica` |
| **Propiedad obsoleta** | `GlasgowTotal` |
| **Reemplazo** | `GlasgowOcular + GlasgowVerbal + GlasgowMotor` |
| **Mensaje de obsolescencia** | "Calcular como GlasgowOcular + GlasgowVerbal + GlasgowMotor. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/ValoracionFisica.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/Admision/RegistrarTriageYValoracionCommandHandler.cs` (líneas 75, 130)
- `Commands/Admision/ModificarTriageYValoracionCommandHandler.cs` (línea 92)
- `Queries/Admision/GetTriageYValoracionHistoryQueryHandler.cs` (línea 65)
- `Queries/Admision/GetCamasMonitoreoQuery.cs` (línea 132)

**Tipo de cambio:** Calcular el total de Glasgow derivado de la suma de sus componentes.

## 3. Impacto en Frontend

- Verificar si la API expone `glasgowTotal`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `ValoracionFisica.GlasgowTotal`.
- [ ] 2. Reemplazar por `GlasgowOcular + GlasgowVerbal + GlasgowMotor` (calculado).
- [ ] 3. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 4. Compilar backend y frontend.
- [ ] 5. Crear migración para eliminar la columna `GlasgowTotal`.

## 5. Verificación

- `dotnet build` sin CS0618 para `ValoracionFisica.GlasgowTotal`.
- El total de Glasgow en triage y monitoreo es correcto.

## 6. Riesgos

- Verificar que los componentes (Ocular, Verbal, Motor) estén siempre poblados antes de calcular.