# ⚠️ Registro de Errores y Lecciones (Mistakes.md)

Este documento registra los errores cometidos, problemas de diseño y fallos técnicos encontrados para asegurar que No se repitan en futuras iteraciones.

## 🏛️ Errores de Diseño Arquitectónico

### 1. Auto-Stub de Baja Calidad (V11.0)
- **Error**: Se implementó una creación de paciente stub en `AgendarTurnoCommandHandler` usando datos genéricos (`Cedula = "LEGACY"`, `Nombre = "Paciente del Legado {id}"`).
- **Impacto**: La base de datos se pobló con registros inconsistentes, afectando la calidad de los reportes y la UI de Cobros.
- **Corrección**: Implementar "Onboarding JIT con Paridad Total". Siempre consultar al `ILegacyLabRepository` antes de crear un stub para obtener datos reales (Cédula, Nombre, Apellidos, Teléfono).
- **Lección**: La integridad referencial no es suficiente; la calidad de los datos es mandatoria.

## 🏗️ Errores de Configuración y Build

### 2. Referencias Obsoletas en Proyectos .NET Framework
- **Error**: El proyecto `Conexiones.csproj` mantenía referencias a archivos inexistentes (`HojadeTrabajo.cs`) y rutas absolutas locales (`C:\Proyectos\...`).
- **Impacto**: Fallo total del build de la solución.
- **Corrección**: Limpiar archivos `.csproj` eliminando referencias a archivos movidos (a `Laboratorio`) y normalizar `HintPaths` a rutas relativas dentro de `packages`.
- **Lección**: Los proyectos heredados son frágiles a cambios de estructura de carpetas.

### 3. Falta de Sincronización en Migración de Identidad
- **Error**: Se migró de `int` a `Guid` en el sistema de identidad pero no se actualizaron inmediatamente los constructores de `CitaMedica` en los proyectos de `UnitTests`.
- **Impacto**: Errores `CS1503` (Tipo inválido) bloqueando el CI/CD.
- **Corrección**: Cada cambio de esquema en `Core.Domain` debe ser reflejado inmediatamente en todos los proyectos de prueba (`UnitTests`, `IntegrationTests`).

## 💾 Errores de Persistencia (EF Core)

### 4. Ignorar Cambios de Modelo Pendientes
- **Error**: Se intentó actualizar la base de datos sin generar una migración tras cambios en las entidades de `DbContext`.
- **Impacto**: Error `20409` (Pending changes context).
- **Corrección**: Siempre verificar el estado del modelo con `dotnet ef migrations add` antes de `database update`.

### 5. Bloqueos de Archivos por Procesos Activos
- **Error**: Intentar aplicar migraciones o compilar mientras el servidor `WebAPI` o el Dashboard de Aspire mantienen bloqueadas las DLLs de infraestructura.
- **Impacto**: Errores de acceso denegado (IO Exception: `MSB3021`, `MSB3027`).
- **Corrección**: Terminar procesos activos (`taskkill /F /IM ...` o `/PID ...`) antes de realizar operaciones de build o persistencia crítica.
- **Lección**: Las orquestaciones multiplataforma (Aspire) requieren una gestión manual de limpieza de procesos en entornos Windows cuando el auto-hot-reload no es suficiente.

### 6. Desajuste de Configuración JWT (401 Unauthorized)
- **Error**: El Dashboard devolvía 401 a pesar de un login exitoso.
- **Impacto**: Los usuarios no podían ver las métricas tras ingresar.
- **Causa**: Discrepancia en `Issuer` y `Audience` entre la generación del token (backend) y la validación (middleware), exacerbada por la inyección parcial de variables de entorno en el `AppHost` de Aspire.
- **Corrección**: Asegurar que el `AppHost` inyecte todas las variables de configuración (`JwtConfig:Issuer`, `JwtConfig:Audience`) explícitamente al proyecto API.
- **Lección**: No dependas de los valores por defecto en `appsettings.json` cuando usas un orquestador que inyecta secretos; inyecta la configuración completa para coherencia total.

### 7. Desajuste en Nombres de Roles (DashBoard $0.00 Mismatch)
- **Error**: El Dashboard mostraba ceros a pesar de datos existentes en la DB.
- **Impacto**: Los administradores no veían la salud financiera real del hospital.
- **Causa**: Hardcoding de roles en el backend (`Administrador`, `Asistente Rx`) que no coincidían con los roles sembrados (`Admin`, `Asistente RX`). La comparación `==` de C# es sensible a mayúsculas.
- **Corrección**: Implementar `AuthorizationConstants` (Senior Pattern) y usar `StringComparison.OrdinalIgnoreCase`.
- **Lección**: Centralizar siempre los roles en constantes de dominio y usar comparaciones proactivas e insensibles a mayúsculas para evitar bloqueos por strings "mágicos".

### 8. Error de Traducción LINQ por Propiedades No Mapeadas (Dashboard & AR)
- **Error**: Intentar filtrar, proyectar o agregar (`Sum`, `Where`, `Select`) usando propiedades calculadas como `ReciboFactura.Estado` o `CuentaPorCobrar.SaldoPendienteBase`.
- **Impacto**: Excepción `System.InvalidOperationException` y error 500.
- **Causa**: Las propiedades que solo tienen `get` y no están mapeadas en el `DbContext` (o están marcadas con `.Ignore()`) no pueden ser traducidas a SQL por EF Core. 
- **Corrección**: 
  - Usar los miembros de persistencia subyacentes en la consulta (ej: `r.EstadoFiscal`).
  - Para cálculos, escribir la expresión directamente en el LINQ (ej: `ar.MontoTotalBase - ar.MontoPagadoBase`).
  - Asegurar que estas propiedades estén en `.Ignore()` en el `DbContext` (Rule #9).
- **Lección**: La capa de `IQueryable` debe operar estrictamente sobre el esquema de base de datos. Si se necesita lógica compleja reutilizable en LINQ, usar `Expressions` o realizar el cálculo tras materializar con `ToListAsync()` (si el volumen de datos lo permite).
