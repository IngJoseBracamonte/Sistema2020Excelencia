# Análisis: `DocumentLog.UserName`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `DocumentLog` |
| **Propiedad obsoleta** | `UserName` |
| **Reemplazo** | Derivar de `UsuarioIdentityId` vía Identity |
| **Mensaje de obsolescencia** | "Derivar de UsuarioIdentityId vía Identity. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DocumentLog.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 857)

**Tipo de cambio:** Derivar el nombre del usuario desde `UsuarioIdentityId` consultando Identity.

## 3. Impacto en Frontend

- Verificar si la API expone `userName`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `DocumentLog.UserName`.
- [ ] 2. Verificar que `UsuarioIdentityId` está poblado.
- [ ] 3. En los queries, hacer join con Identity para obtener el nombre.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UserName`.

## 5. Verificación

- `dotnet build` sin CS0618 para `DocumentLog.UserName`.
- Los logs de documentos muestran el nombre del usuario correctamente.

## 6. Riesgos

- Requiere acceso a Identity desde los queries de logs.
- Si `UsuarioIdentityId` está vacío en datos legacy, el nombre no se podrá derivar.