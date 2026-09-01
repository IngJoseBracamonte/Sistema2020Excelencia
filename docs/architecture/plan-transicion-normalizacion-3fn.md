# Plan de Transicion a 3FN en SatHospitalario

## Objetivo y limites

Normalizar progresivamente la base moderna `SatHospitalario` sin modificar MySQL `sistema2020` Legacy ni interrumpir los clientes actuales. Las FK y GUID son la fuente de verdad operativa. Los textos solo se conservan como proyecciones de lectura temporal durante la migracion; no se conservan snapshots ni duplicados. Las unicas excepciones son los valores clinicos obtenidos directamente de Pruebas de Laboratorio y los registros de Triage.

No se eliminaran columnas, se ejecutaran actualizaciones masivas ni se impondran restricciones nuevas hasta que las fases previas demuestren que los datos y consumidores son compatibles.

## Resultado de la revision del DbContext

`SatHospitalarioDbContext` ya configura relaciones con FK para `DetalleServicioCuenta.TipoServicioId`, `ServicioClinico.EspecialidadId`, `ServicioInsumoReceta.ServicioClinicoId` y `ServicioInsumoReceta.InsumoId`.

Existen cuatro brechas prioritarias:

| Prioridad | Entidad | Situacion actual | Regla objetivo |
| --- | --- | --- | --- |
| P0 | `ServicioClinico`, `DetalleServicioCuenta` | Persisten `TipoServicio` y `TipoServicioId`; handlers enrutan por texto. | `TipoServicioId` es autoritativo para operaciones; el texto se elimina tras migrar los consumidores. |
| P0 | `Insumo` | Persiste `Categoria` como texto aunque existe `CategoriaInsumo`; no hay FK. | Agregar `CategoriaInsumoId` y relacion EF Core; la categoria textual deja de ser selector. |
| P1 | `OrdenCompraInventario` | Tiene `ProveedorId` y `ProveedorNombre`, sin navegacion ni FK. | `ProveedorId` identifica al proveedor; el nombre se resuelve mediante la relacion y se elimina de la orden. |
| P1 | `ServicioInsumoReceta` | Tiene `ServicioClinicoId` y `ServicioCodigo`. | El servicio se ubica por ID; el codigo se proyecta desde el servicio y no se persiste en la receta. |

Los siguientes campos se consideran redundancias a eliminar o reemplazar por relaciones y proyecciones: descripcion y precio facturados en `DetalleServicioCuenta`; nombres anteriores/nuevos en `HistorialModificacionCuenta`, `LogAsignacionHonorario` y `LogAuditoriaPrecio`; y textos de `MovimientoInsumo.Motivo` que repliquen catálogos o entidades. Solo los resultados obtenidos directamente de Pruebas de Laboratorio y los registros de Triage pueden mantener valores clínicos capturados.

## Fase 0: Medicion y caracterizacion

Antes de cambiar escrituras, generar un reporte de FK nulas, inexistentes o inactivas, textos sin equivalencia, equivalencias ambiguas y divergencias entre texto e ID. El reporte debe conservar el identificador del registro, valor actual, candidato de catalogo y resultado de la validacion.

```sql
SELECT
    d.Id,
    d.TipoServicio,
    d.TipoServicioId,
    ts.Id AS TipoServicioResueltoId,
    ts.Codigo AS TipoServicioCodigo,
    ts.Nombre AS TipoServicioNombre
FROM DetallesServicioCuenta d
LEFT JOIN TiposServicio ts ON ts.Id = d.TipoServicioId
WHERE d.TipoServicioId IS NULL
   OR ts.Id IS NULL
   OR UPPER(TRIM(d.TipoServicio)) <> UPPER(TRIM(ts.Nombre));

SELECT
    s.Id,
    s.Codigo,
    s.TipoServicio,
    s.TipoServicioId,
    ts.Codigo AS TipoServicioCodigo,
    ts.Nombre AS TipoServicioNombre
FROM ServiciosClinicos s
LEFT JOIN TiposServicio ts ON ts.Id = s.TipoServicioId
WHERE s.TipoServicioId IS NULL
   OR ts.Id IS NULL
   OR UPPER(TRIM(s.TipoServicio)) <> UPPER(TRIM(ts.Nombre));

SELECT
    i.Id,
    i.Codigo,
    i.Nombre,
    i.Categoria,
    COUNT(ci.Id) AS CoincidenciasCategoria
FROM Insumos i
LEFT JOIN CategoriasInsumo ci
    ON UPPER(TRIM(ci.Nombre)) = UPPER(TRIM(i.Categoria))
GROUP BY i.Id, i.Codigo, i.Nombre, i.Categoria
HAVING COUNT(ci.Id) <> 1;

SELECT
    r.Id,
    r.ServicioClinicoId,
    r.ServicioCodigo,
    s.Codigo AS ServicioCodigoResuelto
FROM ServiciosInsumoRecetas r
LEFT JOIN ServiciosClinicos s ON s.Id = r.ServicioClinicoId
WHERE s.Id IS NULL
   OR r.ServicioCodigo <> s.Codigo;
```

No se hara backfill automatico de registros ambiguos o huerfanos. Deben resolverse mediante una tabla de excepciones aprobada y con auditoria.

## Fase 1: Backend ID-prioritario sin cambio destructivo

Cambiar primero el comportamiento operativo de Application, manteniendo las columnas y los campos actuales del contrato.

1. En los commands de carga, sincronizacion, traslado y cierre, resolver el tipo desde `ServicioClinico.TipoServicioId` cuando `ServicioId` sea un GUID nativo valido.
2. Usar `TipoServicio` textual solo para perfiles Legacy o filas historicas sin FK valida. Registrar el fallback con entidad, ID, texto, usuario y correlacion de solicitud.
3. Mantener dual-write mientras existan consumidores de texto: las nuevas filas guardan el ID y un texto derivado del catalogo. El texto recibido por el cliente no debe prevalecer sobre la relacion resuelta.
4. Agregar proyecciones de lectura normalizadas sin quitar propiedades existentes:

```json
{
  "tipoServicioId": 1,
  "tipoServicio": {
    "id": 1,
    "codigo": "MED",
    "nombre": "Medico"
  },
  "tipo": "CONSULTA"
}
```

`tipo` queda como alias de compatibilidad. El frontend debe seleccionar y enviar el ID, y mostrar el nombre de la proyeccion.

**Criterio de salida:** una discrepancia entre request `TipoServicio` y catalogo no cambia la ruta de negocio cuando existe un `ServicioId` nativo valido.

## Fase 2: Categoria de insumo y contratos de referencias

Agregar de forma aditiva `Insumo.CategoriaInsumoId`, su navegacion y la configuracion EF Core `HasForeignKey`, indice y FK hacia `CategoriasInsumo` en `SatHospitalario`.

1. El DTO de escritura acepta `categoriaInsumoId`; `categoria` textual se acepta temporalmente solo si el ID no viene y resuelve una coincidencia unica activa.
2. El DTO de lectura expone una referencia normalizada:

```json
{
  "categoria": {
    "id": "guid",
    "codigo": "DESC",
    "nombre": "Descartable"
  },
  "categoriaLegacy": "Descartable"
}
```

3. Ejecutar backfill por lotes solo para coincidencias unicas de categoria. Conservar una bitacora de las filas actualizadas y un reporte separado para excepciones.
4. Tras migrar los clientes, prohibir `categoria` como identificador de escritura y dejar de propagar cambios de nombre a todos los insumos.

**Criterio de salida:** todos los insumos operativos nuevos tienen una categoria activa por FK, y los casos no mapeables permanecen visibles en el reporte de excepciones.

## Fase 3: Proveedor y receta

Para ordenes de compra, configurar navegacion, indice y FK de `ProveedorId` a `Proveedor`. En las respuestas, exponer `proveedor: { id, codigo, nombre }` y retirar `ProveedorNombre`; no se conserva como snapshot.

Para recetas, `ServicioClinicoId` es el unico selector de escritura. Las consultas deben proyectar `codigo` y `descripcion` desde `ServicioClinico`; `ServicioCodigo` se retira tras migrar los consumidores. Ningun consumo, costo o regla clinica puede localizar recetas por ese texto.

**Criterio de salida:** compras y recetas nuevas rechazan relaciones inexistentes, y sus lecturas incluyen referencias resueltas desde las FK.

## Fase 4: Endurecimiento y retiro

Cuando las metricas muestren cero fallbacks no justificados y los reportes de excepciones esten resueltos:

1. Requerir las FK para escrituras nuevas y activar las restricciones pendientes.
2. Deshabilitar la lectura operacional por texto mediante feature flags por modulo.
3. Retirar todos los campos redundantes, incluidos los de auditoria, que no sean resultados directos de laboratorio ni triages.
4. Retirar columnas de compatibilidad solo mediante una migracion separada, con respaldo validado y periodo de observacion. No forman parte del rollback automatico.

## Pruebas requeridas

Las pruebas actuales cubren carga de servicios, catalogo unificado, inventario y cuentas por pagar, pero no prueban la precedencia de una FK ante texto divergente, el backfill ni los contratos HTTP. Cada fase debe agregar las siguientes pruebas antes de habilitarla:

| Capa | Casos minimos |
| --- | --- |
| Domain/Application | Con ID valido y texto divergente, la ruta se decide por ID; con ID nulo de registro heredado, se usa fallback auditado; ID inexistente o catalogo inactivo se rechaza. |
| Infrastructure | FK, indice y navegacion se crean en SQLite/MySQL de prueba; backfill actualiza solo equivalencias unicas; huerfanos y ambiguos no se actualizan. |
| API | Escrituras nuevas con GUID valido devuelven referencias `{ id, codigo, nombre }`; contratos compatibles conservan aliases; respuestas `400`, `404`, `409` y `422` son consistentes. |
| E2E | La UI carga catalogos, selecciona GUID, envia IDs y muestra el nombre resuelto; una discrepancia texto/ID no altera facturacion, inventario ni imagenologia. |

Se actualizaran en primer lugar las pruebas de `CargarServicioACuentaCommandHandler`, `GetUnifiedCatalogQueryHandler`, `ServicioCatalogCrud`, `GetOrdenesCompraQueryHandler` y las suites E2E de catalogo, facturacion e inventario.

## Rollback y control operativo

- Fase 1 se revierte activando feature flags de lectura textual; no borra datos.
- Fase 2 y 3 solo revierten constraints, indices o columnas nuevas vacias. Un rollback no reconstruye columnas redundantes retiradas; estas requieren respaldo validado y restauracion aprobada.
- Toda discrepancia, fallback y remediacion registra actor, fecha UTC, entidad, valores anterior/nuevo, origen de resolucion y correlacion.
- Los cambios se despliegan por modulo: catalogo/carga, inventario, compras y recetas. Cada despliegue exige pruebas verdes, reporte de integridad y revision de logs de API antes del siguiente.