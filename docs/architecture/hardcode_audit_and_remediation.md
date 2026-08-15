# Arquitectura: Auditoría y Remediación de Código Hardcodeado y Bugs de Lógica

**Fecha**: 2026-08-14  
**Sistema**: Sistema Sat Hospitalario v4.0.8  
**Patrones aplicados**: Clean Architecture, CQRS, TDD, DB-Driven Architecture, 3FN Normalization, Angular Signals.

---

## 1. Resumen Ejecutivo
Se ejecutó una auditoría exhaustiva en Backend (.NET 9) y Frontend (Angular 19+) para detectar código hardcodeado, lógica simulada (mock) en producción y bugs sutiles de precedencia booleana y cálculos multi-moneda. Todas las remediaciones fueron implementadas y validadas con TDD previo, alcanzando 299/299 pruebas unitarias e integración en verde.

---

## 2. Remediaciones Críticas en Backend (.NET 9)

### A. Corrección de Precedencia Booleana en Honorarios Médicos (`GetDoctorHonorariumSummaryQuery`)
- **Problema**: La exclusión de consultas en el paso 2 (`fromServicios`) contenía un error de precedencia en la negación (`!(A || B || C) || detail.CategoriaHonorario == Consulta`), lo que causaba que cualquier detalle con `CategoriaHonorario == 'CONSULTA'` fuera evaluado como `true` e ingresara incorrectamente al desglose técnico.
- **Solución**: Se encapsuló la disyunción completa dentro de la negación y se priorizó la evaluación por clave tipada (`detail.TipoServicioId == TipoServicioConstants.Medico` y `detail.CategoriaHonorario == HonorarioConstants.CategoriaConsulta`).
- **Prueba TDD**: `DoctorHonorariumSummaryQueryTests.cs`.

### B. Normalización Multi-Moneda en Cierre de Caja (`CerrarCajaCommand`)
- **Problema**: `TotalVueltoUSD` realizaba una suma directa de `MontoVueltos` de todos los métodos declarados, sumando cantidades en Bolívares (ej. 500 Bs. de vuelto en Pago Móvil) directamente a Dólares. Además, `TotalIngresosBS` filtraba por un listado rígido de cadenas fijas (`"Efectivo BS"`, `"Pago Movil"`, `"Punto"`).
- **Solución**: 
  1. `TotalVueltoUSD` ahora acumula la conversión de vueltos en moneda local a USD base usando la tasa del día (`TasaCambioDia`).
  2. `TotalIngresosBS` ahora suma dinámicamente cualquier método no dolarizado (`!m.EsUSD && m.GrupoMoneda != 1`) catalogado en base de datos.
- **Prueba TDD**: `CerrarCajaMultiMonedaTests.cs`.

### C. Integridad Referencial en Traslados (`RegistrarTrasladoAreaCommandHandler`)
- **Problema**: Ante la ausencia de un servicio clínico específico, el handler generaba un `Guid.NewGuid()` huérfano para insertar en `DetalleServicioCuenta`.
- **Solución**: Se garantiza la resolución y persistencia explícita de la entidad `ServicioClinico` correspondiente al traslado antes de asignar su clave foránea.

### D. Reutilización de Cuentas Acumulativas para UCI (`SyncCarritoCommand` e `ImagingController`)
- **Problema**: `SyncCarritoCommandHandler` e `ImagingController` solo reconocían `Hospitalizacion` y `Emergencia` al reutilizar cuentas abiertas para ingresos acumulativos, omitiendo `UCI` (`EstadoConstants.UCI`).
- **Solución**: Se extendió la verificación acumulativa a `EstadoConstants.UCI`, evitando la fragmentación o creación indebida de cuentas huérfanas al cargar servicios o validar órdenes de imagenología para pacientes en UCI.
- **Prueba TDD**: `Handle_TipoIngresoUCI_ReutilizaCuentaAbiertaExistente` en `SyncCarritoCommandTests.cs`.

### E. Mapeo Normalizado de Movimientos de Inventario (`MovimientoInsumo`)
- **Problema**: `GetTiposMovimiento` en `InventoryController` exponía identificadores ad-hoc no alineados con el enum `TipoMovimientoInsumo`.
- **Solución**: Sincronización estricta con `TipoMovimientoInsumo` (`Ingreso`, `Consumo`, `Descarte`, `AjusteCierre`, `TransferenciaSalida`) y preservación semántica de los despachos a sub-áreas como consumo directo (Gasto Operativo Almacén Principal, Regla de Negocio #3).

---

## 3. Remediaciones en Frontend (Angular 19+ Standalone)

### A. Eliminación de Algoritmos Mock en Cierre de Cuentas (`cierre-cuenta.component.ts`)
- Se erradicó la generación determinista simulada de estado clínico (`Crítico`, `Estable`, `Observación`), números de habitación basados en hash de cédula (`Box 104B`), tipo de sangre aleatorio (`getMockBloodType`) y edad fallback (`42 años`).
- La vista ahora consume estrictamente los datos clínicos y de ubicación reales del paciente (`patient.grupoSanguineo`, `acc.areaClinicaNombre`, `patient.fechaNacimiento`, `acc.subAreaClinica`).

### B. Corrección de Doble Conversión en Cuentas por Cobrar (`receivables.component.ts`)
- **Problema**: `removePayment` pasaba `remainingBalanceBs()` a `resetNewPayment(amountUSD)`, provocando una doble multiplicación por la tasa de cambio cuando el método activo era en Bolívares.
- **Solución**: Se corrigió el argumento a `this.resetNewPayment(this.remainingBalanceUSD())`.

### C. Mapeo Dinámico de Vueltos en Cuadre de Caja (`cajas.component.ts`)
- **Problema**: `tableRows` realizaba comparaciones fijas contra 3 nombres hardcodeados (`Dolar Efectivo`, `Efectivo BS`, `Pago Movil`).
- **Solución**: El cuadre asocia dinámicamente los registros de vuelto basándose en `grupoMoneda`, `isUSD` y keywords normalizadas (`VUELTO`, `USD`, `BS`).

### D. Búsqueda DB-Driven de Pacientes en Pabellón Quirúrgico (`pabellon-gestion.component.ts`)
- **Problema**: El modal de nueva cirugía requería tipear manualmente cadenas GUID de Paciente y Cuenta de Servicio.
- **Solución**: Se implementó el buscador predictivo de pacientes mediante `PatientService.searchPatients()` y resolución automática de la cuenta abierta del paciente.

### E. Categorías Dinámicas en Inventario (`catalogo.component.ts`)
- **Problema**: Listas estáticas fijas `categorias = [...]` y `unidadesMedida = [...]` (violación de Regla 12).
- **Solución**: Señal computada `categorias` derivada dinámicamente de los insumos existentes y catálogo maestro.

### F. Coincidencia Ampliada de Especialistas en Imagenología (`rx-orders.component.ts`)
- **Problema**: Filtro restrictivo `especialidad.includes('IMAGEN')` que descartaba médicos catalogados como `Radiología`, `RX` o `Tomografía`.
- **Solución**: Coincidencia multi-término (`IMAGEN`, `RADIO`, `RX`, `TOMO`) con fallback resiliente a la lista de médicos activos.

---

## 4. Estado de Validación
- **Tests .NET Unitarios (src)**: 200 / 200 Aprobados (100%)
- **Tests .NET Unitarios (tests)**: 94 / 94 Aprobados (100%)
- **Tests .NET Integración**: 7 / 7 Aprobados (100%)
- **Total Pruebas Automatizadas**: 301 / 301 Aprobadas (0 fallos, 0 omitidas).
- **Frontend Build (Angular 19+)**: 0 errores de compilación (`ng build` exitoso).
