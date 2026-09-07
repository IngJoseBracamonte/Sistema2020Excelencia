# Análisis: `CajaDiaria.NombreUsuario`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CajaDiaria` |
| **Propiedad obsoleta** | `NombreUsuario` |
| **Reemplazo** | Derivar de `UsuarioIdentityId` vía Identity |
| **Mensaje de obsolescencia** | "Derivar de UsuarioIdentityId vía Identity. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CajaDiaria.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/ObtenerResumenCajasQuery.cs` (línea 60)
- `Queries/Admision/ExportCashierAuditQuery.cs` (líneas 50, 123, 124, 156)
- `Queries/Admision/GetCajaSummariesQuery.cs` (línea 101)
- `Commands/Admision/CerrarCajaCommand.cs` (línea 166)

**Tipo de cambio:** Derivar el nombre del usuario desde `UsuarioIdentityId` consultando Identity, en lugar de almacenarlo como columna.

## 3. Impacto en Frontend

- Verificar si la API expone `nombreUsuario`. Debe seguir exponiéndose pero calculado desde Identity.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CajaDiaria.NombreUsuario`.
- [ ] 2. Verificar que `UsuarioIdentityId` está poblado.
- [ ] 3. En los queries, hacer join con Identity para obtener el nombre del usuario.
- [ ] 4. Mantener el campo `nombreUsuario` en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `NombreUsuario`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CajaDiaria.NombreUsuario`.
- Los reportes de caja muestran el nombre del usuario correctamente.

## 6. Riesgos

- Requiere acceso a Identity desde los queries de caja (puede implicar un join adicional).
- Si `UsuarioIdentityId` está vacío en datos legacy, el nombre no se podrá derivar.