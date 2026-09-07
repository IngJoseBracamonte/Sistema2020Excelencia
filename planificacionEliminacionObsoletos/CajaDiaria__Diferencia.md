# Análisis: `CajaDiaria.Diferencia`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CajaDiaria` |
| **Propiedad obsoleta** | `Diferencia` |
| **Reemplazo** | `TotalIngresado - TotalCobrado` |
| **Mensaje de obsolescencia** | "Calcular como TotalIngresado - TotalCobrado. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CajaDiaria.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetCajaSummariesQuery.cs` (línea 109)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 171)

**Tipo de cambio:** Calcular la diferencia derivada de los totales.

## 3. Impacto en Frontend

- Verificar si la API expone `diferencia`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CajaDiaria.Diferencia`.
- [ ] 2. Reemplazar por `TotalIngresado - TotalCobrado` (calculado).
- [ ] 3. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 4. Compilar backend y frontend.
- [ ] 5. Crear migración para eliminar la columna `Diferencia`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CajaDiaria.Diferencia`.
- La diferencia de caja es correcta.

## 6. Riesgos

- Depende de que `TotalIngresado` y `TotalCobrado` estén correctamente calculados.