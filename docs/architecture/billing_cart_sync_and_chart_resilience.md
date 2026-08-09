# Billing Cart Sync & Chart Resilience (v4.0.4)

## Resumen Ejecutivo

Se corrigieron tres familias de errores interrelacionados que se manifestaban en cascada:

1. **TypeError ApexCharts**: `Cannot read properties of undefined (reading 'type')` al inicializar gráficos sin un objeto `chart` base cuando la API de insights retorna 401/error.
2. **400 Bad Request en `/api/Billing/SincronizarCarrito`**: Payload JSON corrupto con valores `NaN`/`Infinity` causados por división entre tasa de cambio = 0.
3. **Cascada de timeout 504**: Conexión SignalR interrumpida por errores no manejados.

## Cambios Realizados

### Backend
- **Sin cambios estructurales requeridos**: El `DetalleSyncDto` ya maneja correctamente el `ServicioId` como `Guid` y el `ServicioCarritoDto.ServicioId` como `string`. La línea 524 retorna `serviceGuid` que es `Guid.Empty` para items legacy, lo cual es correcto.

### Frontend

#### `dashboard.component.ts`
- Chart options ahora se inicializan con `chart.type` definido: `{ chart: { type: 'area', ... } }` en vez de `{}`. Esto previene que ApexCharts intente acceder a `undefined.type`.
- Error handler en `refreshKPIs()` loguea el código HTTP sin corromper el estado.

#### `admin-analytics.component.ts`
- Misma estrategia de inicialización defensiva para 3 gráficos.
- Arrays del `BusinessInsights` se acceden con fallback `|| []` para prevenir `.map()` sobre `undefined`.
- Donut chart con guard anti-cero: si todos los valores son 0, se usa `[1]` como serie para prevenir fallo de render.

#### `billing-facade.service.ts`
- Guard `this.tasaCambioDia() || 1` para prevenir división por 0 en normalización legacy.
- `Number.isFinite()` como sanitizador final: cualquier resultado no-finito se sustituye por 0.
- Aplicado en 3 puntos: `totalCargadoUSD` computed, `syncCartWithBackend` precio y honorario.

## Pruebas Unitarias

Se crearon 5 tests nuevos en `SyncCarritoCommandTests.cs`:
- `Handle_ItemConGuid_DebeRetornarServicioIdGuidEnDetalles` ✅
- `Handle_ItemConIdLegacy_DebeRetornarGuidEmptyComoServicioId` ✅
- `Handle_PrecioModificadoSinSupervisorKey_DebeLanzarExcepcion` ✅
- `Handle_PrecioModificadoConSupervisorKeyValida_DebePermitirSync` ✅
- `Handle_PacienteNoExiste_DebeLanzarExcepcion` ✅

**Total: 5/5 aprobados.**
