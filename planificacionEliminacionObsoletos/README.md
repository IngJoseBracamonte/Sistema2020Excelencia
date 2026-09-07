# Planificación de Eliminación de Obsoletos

> **Objetivo:** Analizar y planificar la eliminación de cada propiedad marcada como `[Obsolete]` ("Columna legacy pendiente de DROP") en el backend, y su impacto en el frontend.

Cada propiedad obsoleta tiene **1 archivo de análisis** en esta carpeta. Este README es el índice maestro.

## Cómo usar

1. Abre el análisis de la propiedad que quieras migrar (enlace en la tabla).
2. Sigue el análisis paso a paso: impacto backend → impacto frontend → migración → verificación.
3. Marca la propiedad como completada (`[x]`).

## Convención de nombres

`<Entidad>__<Propiedad>.md`

---

## Índice de análisis

### Entidades de Admisión

| # | Propiedad | Reemplazo | Análisis | Estado |
|---|-----------|-----------|----------|--------|
| 1 | `AuditLog.UserId` | `UsuarioIdentityId` | [AuditLog__UserId.md](AuditLog__UserId.md) | [ ] |
| 2 | `CajaDiaria.Estado` | `EstadoId / EstadoNav` | [CajaDiaria__Estado.md](CajaDiaria__Estado.md) | [ ] |
| 3 | `CajaDiaria.UsuarioId` | `UsuarioIdentityId` | [CajaDiaria__UsuarioId.md](CajaDiaria__UsuarioId.md) | [ ] |
| 4 | `CajaDiaria.NombreUsuario` | Derivar de `UsuarioIdentityId` | [CajaDiaria__NombreUsuario.md](CajaDiaria__NombreUsuario.md) | [ ] |
| 5 | `CajaDiaria.DeclaracionCierreJson` | `DeclaracionesPorMetodo` | [CajaDiaria__DeclaracionCierreJson.md](CajaDiaria__DeclaracionCierreJson.md) | [ ] |
| 6 | `CajaDiaria.TotalIngresado` | `DeclaracionesPorMetodo.Sum(...)` | [CajaDiaria__TotalIngresado.md](CajaDiaria__TotalIngresado.md) | [ ] |
| 7 | `CajaDiaria.TotalCobrado` | `DeclaracionesPorMetodo.Sum(...)` | [CajaDiaria__TotalCobrado.md](CajaDiaria__TotalCobrado.md) | [ ] |
| 8 | `CajaDiaria.Diferencia` | `TotalIngresado - TotalCobrado` | [CajaDiaria__Diferencia.md](CajaDiaria__Diferencia.md) | [ ] |
| 9 | `CierreInventario.Usuario` | `UsuarioId` | [CierreInventario__Usuario.md](CierreInventario__Usuario.md) | [ ] |
| 10 | `CirugiaLog.UsuarioId` | `UsuarioIdentityId` | [CirugiaLog__UsuarioId.md](CirugiaLog__UsuarioId.md) | [ ] |
| 11 | `CirugiaObservacionHistorial.UsuarioRegistro` | `UsuarioRegistroId` | [CirugiaObservacionHistorial__UsuarioRegistro.md](CirugiaObservacionHistorial__UsuarioRegistro.md) | [ ] |
| 12 | `CitaMedica.Estado` | `EstadoId / EstadoNav` | [CitaMedica__Estado.md](CitaMedica__Estado.md) | [x] |
| 13 | `CompromisoPago.UsuarioCreacion` | `UsuarioCreacionId` | [CompromisoPago__UsuarioCreacion.md](CompromisoPago__UsuarioCreacion.md) | [ ] |
| 14 | `CuentaPorCobrar.UsuarioAuditoria` | `UsuarioAuditoriaId` | [CuentaPorCobrar__UsuarioAuditoria.md](CuentaPorCobrar__UsuarioAuditoria.md) | [ ] |
| 15 | `CuentaServicios.UsuarioCarga` | `UsuarioCargaId` | [CuentaServicios__UsuarioCarga.md](CuentaServicios__UsuarioCarga.md) | [ ] |
| 16 | `CuentaServicios.Estado` | `EstadoId / EstadoNav` | [CuentaServicios__Estado.md](CuentaServicios__Estado.md) | [ ] |
| 17 | `CuentaServicios.TipoIngreso` | `TipoIngresoId / TipoIngresoNav` | [CuentaServicios__TipoIngreso.md](CuentaServicios__TipoIngreso.md) | [ ] |
| 18 | `CuentaServicios.UsuarioValidacion` | `UsuarioValidacionId` | [CuentaServicios__UsuarioValidacion.md](CuentaServicios__UsuarioValidacion.md) | [ ] |
| 19 | `CuentaServicios.UsuarioAuditoria` | `UsuarioAuditoriaId` | [CuentaServicios__UsuarioAuditoria.md](CuentaServicios__UsuarioAuditoria.md) | [ ] |
| 20 | `DetallePago.MetodoPago` | `MetodoPagoId / MetodoPagoNav` | [DetallePago__MetodoPago.md](DetallePago__MetodoPago.md) | [ ] |
| 21 | `DetallePago.UsuarioCarga` | `UsuarioCargaId` | [DetallePago__UsuarioCarga.md](DetallePago__UsuarioCarga.md) | [ ] |
| 22 | `DetalleServicioCuenta.TipoServicio` | `TipoServicioId / TipoServicioNav` | [DetalleServicioCuenta__TipoServicio.md](DetalleServicioCuenta__TipoServicio.md) | [ ] |
| 23 | `DetalleServicioCuenta.UsuarioCarga` | `UsuarioCargaId` | [DetalleServicioCuenta__UsuarioCarga.md](DetalleServicioCuenta__UsuarioCarga.md) | [ ] |
| 24 | `DetalleServicioCuenta.CategoriaHonorario` | Derivar de `TipoServicioId / MedicoResponsable` | [DetalleServicioCuenta__CategoriaHonorario.md](DetalleServicioCuenta__CategoriaHonorario.md) | [ ] |
| 25 | `DetalleServicioCuenta.UsuarioTecnico` | `UsuarioTecnicoId` | [DetalleServicioCuenta__UsuarioTecnico.md](DetalleServicioCuenta__UsuarioTecnico.md) | [ ] |
| 26 | `DocumentLog.UserId` | `UsuarioIdentityId` | [DocumentLog__UserId.md](DocumentLog__UserId.md) | [ ] |
| 27 | `DocumentLog.UserName` | Derivar de `UsuarioIdentityId` | [DocumentLog__UserName.md](DocumentLog__UserName.md) | [ ] |
| 28 | `HistorialModificacionCuenta.PacienteAnteriorNombre` | Derivar del paciente | [HistorialModificacionCuenta__PacienteAnteriorNombre.md](HistorialModificacionCuenta__PacienteAnteriorNombre.md) | [ ] |
| 29 | `HistorialModificacionCuenta.PacienteNuevoNombre` | Derivar del paciente | [HistorialModificacionCuenta__PacienteNuevoNombre.md](HistorialModificacionCuenta__PacienteNuevoNombre.md) | [ ] |
| 30 | `HistorialModificacionCuenta.ConvenioAnteriorNombre` | Derivar del convenio | [HistorialModificacionCuenta__ConvenioAnteriorNombre.md](HistorialModificacionCuenta__ConvenioAnteriorNombre.md) | [ ] |
| 31 | `HistorialModificacionCuenta.ConvenioNuevoNombre` | Derivar del convenio | [HistorialModificacionCuenta__ConvenioNuevoNombre.md](HistorialModificacionCuenta__ConvenioNuevoNombre.md) | [ ] |
| 32 | `HistorialModificacionCuenta.DetalleServiciosCambiosJson` | `DetallesModificados` | [HistorialModificacionCuenta__DetalleServiciosCambiosJson.md](HistorialModificacionCuenta__DetalleServiciosCambiosJson.md) | [ ] |
| 33 | `HonorarioConfig.UsuarioConfiguro` | `UsuarioConfiguroId` | [HonorarioConfig__UsuarioConfiguro.md](HonorarioConfig__UsuarioConfiguro.md) | [ ] |
| 34 | `HonorarioMedicoServicio.UsuarioModifico` | `UsuarioModificoId` | [HonorarioMedicoServicio__UsuarioModifico.md](HonorarioMedicoServicio__UsuarioModifico.md) | [ ] |
| 35 | `HonorariumMappingRule.UsuarioCreo` | `UsuarioCreoId` | [HonorariumMappingRule__UsuarioCreo.md](HonorariumMappingRule__UsuarioCreo.md) | [ ] |
| 36 | `Insumo.UnidadMedidaBase` | `UnidadMedidaId / UnidadMedidaNav` | [Insumo__UnidadMedidaBase.md](Insumo__UnidadMedidaBase.md) | [ ] |
| 37 | `Insumo.Categoria` | `CategoriaInsumoId / CategoriaInsumo` | [Insumo__Categoria.md](Insumo__Categoria.md) | [ ] |
| 38 | `LogAsignacionHonorario.UsuarioOperador` | `UsuarioOperadorId` | [LogAsignacionHonorario__UsuarioOperador.md](LogAsignacionHonorario__UsuarioOperador.md) | [ ] |
| 39 | `LogAuditoriaPrecio.UsuarioOperador` | `UsuarioOperadorId` | [LogAuditoriaPrecio__UsuarioOperador.md](LogAuditoriaPrecio__UsuarioOperador.md) | [ ] |
| 40 | `LogAuditoriaPrecio.AutorizadoPor` | `AutorizadoPorId` | [LogAuditoriaPrecio__AutorizadoPor.md](LogAuditoriaPrecio__AutorizadoPor.md) | [ ] |
| 41 | `MovimientoInsumo.UnidadMedidaOriginal` | `UnidadMedidaOriginalId` | [MovimientoInsumo__UnidadMedidaOriginal.md](MovimientoInsumo__UnidadMedidaOriginal.md) | [ ] |
| 42 | `MovimientoInsumo.Usuario` | `UsuarioIdentityId` | [MovimientoInsumo__Usuario.md](MovimientoInsumo__Usuario.md) | [ ] |
| 43 | `OrdenCirugia.UsuarioCreacion` | `UsuarioCreacionId` | [OrdenCirugia__UsuarioCreacion.md](OrdenCirugia__UsuarioCreacion.md) | [ ] |
| 44 | `OrdenCompraInventario.ProveedorNombre` | `Proveedor.RazonSocial` | [OrdenCompraInventario__ProveedorNombre.md](OrdenCompraInventario__ProveedorNombre.md) | [ ] |
| 45 | `OrdenCompraInventario.MontoTotalBs` | `MontoTotalUSD * tasaCambio` | [OrdenCompraInventario__MontoTotalBs.md](OrdenCompraInventario__MontoTotalBs.md) | [ ] |
| 46 | `OrdenCompraInventario.TotalAbonadoUSD` | `Pagos.Sum(...)` | [OrdenCompraInventario__TotalAbonadoUSD.md](OrdenCompraInventario__TotalAbonadoUSD.md) | [ ] |
| 47 | `OrdenCompraInventario.SaldoPendienteUSD` | `MontoTotalUSD - TotalAbonadoUSD` | [OrdenCompraInventario__SaldoPendienteUSD.md](OrdenCompraInventario__SaldoPendienteUSD.md) | [ ] |
| 48 | `OrdenImagen.PacienteNombre` | `Paciente.NombreCorto` | [OrdenImagen__PacienteNombre.md](OrdenImagen__PacienteNombre.md) | [ ] |
| 49 | `OrdenImagen.MedicoSolicitanteNombre` | `MedicoSolicitante.Nombre` | [OrdenImagen__MedicoSolicitanteNombre.md](OrdenImagen__MedicoSolicitanteNombre.md) | [ ] |
| 50 | `PagoProveedor.UsuarioId` | `UsuarioIdentityId` | [PagoProveedor__UsuarioId.md](PagoProveedor__UsuarioId.md) | [ ] |
| 51 | `PedidoInterSede.UsuarioCreador` | `UsuarioCreadorId` | [PedidoInterSede__UsuarioCreador.md](PedidoInterSede__UsuarioCreador.md) | [ ] |
| 52 | `ReciboFactura.EstadoFiscal` | `EstadoFiscalId / EstadoFiscalNav` | [ReciboFactura__EstadoFiscal.md](ReciboFactura__EstadoFiscal.md) | [ ] |
| 53 | `ReciboFactura.UsuarioEmision` | `UsuarioEmisionId` | [ReciboFactura__UsuarioEmision.md](ReciboFactura__UsuarioEmision.md) | [ ] |
| 54 | `ReservaTemporal.UsuarioId` | `UsuarioIdentityId` | [ReservaTemporal__UsuarioId.md](ReservaTemporal__UsuarioId.md) | [ ] |
| 55 | `ServicioInsumoReceta.UnidadMedidaConsumo` | `UnidadMedidaConsumoId` | [ServicioInsumoReceta__UnidadMedidaConsumo.md](ServicioInsumoReceta__UnidadMedidaConsumo.md) | [ ] |
| 56 | `SolicitudInsumoCirugia.UsuarioSolicitud` | `UsuarioSolicitudId` | [SolicitudInsumoCirugia__UsuarioSolicitud.md](SolicitudInsumoCirugia__UsuarioSolicitud.md) | [ ] |
| 57 | `SolicitudInsumoCirugia.UsuarioDespacho` | `UsuarioDespachoId` | [SolicitudInsumoCirugia__UsuarioDespacho.md](SolicitudInsumoCirugia__UsuarioDespacho.md) | [ ] |
| 58 | `TransferenciaReposicionStock.UsuarioId` | `UsuarioIdentityId` | [TransferenciaReposicionStock__UsuarioId.md](TransferenciaReposicionStock__UsuarioId.md) | [ ] |
| 59 | `TriageEnfermeria.UsuarioRegistro` | `UsuarioRegistroId` | [TriageEnfermeria__UsuarioRegistro.md](TriageEnfermeria__UsuarioRegistro.md) | [ ] |
| 60 | `ValoracionFisica.GlasgowTotal` | `GlasgowOcular + GlasgowVerbal + GlasgowMotor` | [ValoracionFisica__GlasgowTotal.md](ValoracionFisica__GlasgowTotal.md) | [ ] |
| 61 | `ValoracionFisica.UsuarioRegistro` | `UsuarioRegistroId` | [ValoracionFisica__UsuarioRegistro.md](ValoracionFisica__UsuarioRegistro.md) | [ ] |

### Entidades Comunes / Otras

| # | Propiedad | Reemplazo | Análisis | Estado |
|---|-----------|-----------|----------|--------|
| 62 | `Notification.TargetUserId` | `TargetUserGuidId` | [Notification__TargetUserId.md](Notification__TargetUserId.md) | [ ] |
| 63 | `ErrorTicket.UsuarioAsociado` | `UsuarioAsociadoId` | [ErrorTicket__UsuarioAsociado.md](ErrorTicket__UsuarioAsociado.md) | [ ] |
| 64 | `ErrorTicket.ResueltoPor` | `ResueltoPorId` | [ErrorTicket__ResueltoPor.md](ErrorTicket__ResueltoPor.md) | [ ] |
| 65 | `OrdenDeServicio.NombrePaciente` | `Paciente.NombreCorto` | [OrdenDeServicio__NombrePaciente.md](OrdenDeServicio__NombrePaciente.md) | [ ] |
| 66 | `OrdenDeServicio.TotalCobrado` | Calcular desde detalles | [OrdenDeServicio__TotalCobrado.md](OrdenDeServicio__TotalCobrado.md) | [ ] |

---

## Plantilla de cada análisis

Cada archivo sigue esta estructura profesional:

1. **Identificación** — propiedad, entidad, reemplazo, mensaje de obsolescencia.
2. **Impacto en Backend** — archivos que la usan, tipo de cambio.
3. **Impacto en Frontend** — si la API la expone, archivos que la consumen.
4. **Análisis paso a paso** — checklist de migración.
5. **Verificación** — cómo confirmar el cambio.
6. **Riesgos** — consideraciones especiales.