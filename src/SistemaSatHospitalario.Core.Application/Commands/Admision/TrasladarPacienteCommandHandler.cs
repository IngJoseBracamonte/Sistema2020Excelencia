using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class TrasladarPacienteCommandHandler : IRequestHandler<TrasladarPacienteCommand, TrasladarPacienteResult>
    {
        private readonly IApplicationDbContext _context;

        public TrasladarPacienteCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TrasladarPacienteResult> Handle(TrasladarPacienteCommand request, CancellationToken cancellationToken)
        {
            // 1. Obtener la cuenta activa actual del paciente
            var cuentaActual = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.PacienteId == request.PacienteId && c.Estado == EstadoConstants.Abierta, cancellationToken);

            if (cuentaActual == null)
            {
                throw new InvalidOperationException($"El paciente con ID {request.PacienteId} no tiene una cuenta activa para realizar el traslado o egreso.");
            }

            // 2. Liquidación parcial de estancia de origen por tiempo transcurrido
            if (cuentaActual.AreaClinicaId.HasValue)
            {
                var camaAnterior = await _context.AreasClinicas
                    .Include(a => a.ServicioTarifaBase)
                    .FirstOrDefaultAsync(a => a.Id == cuentaActual.AreaClinicaId.Value, cancellationToken);
                
                if (camaAnterior != null)
                {
                    // Lógica del "Efecto Sándwich" - Usar FechaHoraEgresoEfectiva o UtcNow
                    var egresoEfectivo = request.FechaHoraEgresoEfectiva ?? DateTime.UtcNow;
                    if (egresoEfectivo < cuentaActual.FechaCarga)
                    {
                        egresoEfectivo = cuentaActual.FechaCarga; // Salvaguarda clínica
                    }

                    var delta = egresoEfectivo - cuentaActual.FechaCarga;
                    double totalHours = delta.TotalHours;
                    if (totalHours < 0) totalHours = 0;

                    decimal precioEstancia = 0;
                    decimal cantidadEstancia = 1;

                    if (request.MontoSobrescrito.HasValue)
                    {
                        precioEstancia = request.MontoSobrescrito.Value;
                        cantidadEstancia = 1; // Tarifa plana/monto fijo
                    }
                    else if (camaAnterior.ServicioTarifaBase != null)
                    {
                        precioEstancia = camaAnterior.ServicioTarifaBase.PrecioBase;
                        if (cuentaActual.ConvenioId.HasValue)
                        {
                            var priceConv = await _context.PreciosServicioConvenio
                                .FirstOrDefaultAsync(p => p.SeguroConvenioId == cuentaActual.ConvenioId.Value 
                                                          && p.ServicioClinicoId == camaAnterior.ServicioTarifaBaseId.Value, cancellationToken);
                            if (priceConv != null)
                            {
                                precioEstancia = priceConv.PrecioDiferencial;
                            }
                        }

                        var servicio = camaAnterior.ServicioTarifaBase;
                        if (servicio.PermiteFraccionamiento)
                        {
                            cantidadEstancia = (decimal)Math.Max(totalHours, 0.1);
                        }
                        else
                        {
                            cantidadEstancia = (decimal)Math.Max(Math.Ceiling(totalHours / 24.0), 1.0);
                        }
                    }

                    if (camaAnterior.ServicioTarifaBase != null)
                    {
                        var servicio = camaAnterior.ServicioTarifaBase;
                        string descripcionCargo = $"Cargo de Estancia - {camaAnterior.Nombre}";
                        if (request.MontoSobrescrito.HasValue)
                        {
                            descripcionCargo += " (Ajuste Manual)";
                        }
                        else
                        {
                            descripcionCargo += $" ({cantidadEstancia:F1} {servicio.UnidadMedida ?? "Bloque(s)"})";
                        }

                        var nuevoDetalle = cuentaActual.AgregarServicio(
                            servicio.Id,
                            descripcionCargo,
                            precioEstancia,
                            0, // Honorario
                            cantidadEstancia,
                            servicio.TipoServicio,
                            request.UsuarioTraslado,
                            servicio.LegacyMappingId,
                            camaAnterior.Id
                        );

                        await _context.DetallesServicioCuenta.AddAsync(nuevoDetalle, cancellationToken);
                    }

                    // Gestión de Transición Física Polimórfica (Quirófano)
                    bool esDestinoQuirofano = !request.EsEgreso && 
                        (string.Equals(request.NuevoTipoIngreso, "Quirofano", StringComparison.OrdinalIgnoreCase) || 
                         string.Equals(request.NuevoTipoIngreso, "Quirófano", StringComparison.OrdinalIgnoreCase));

                    if (esDestinoQuirofano)
                    {
                        camaAnterior.MarcarEnRetencionQuirurgica();
                    }
                    else
                    {
                        camaAnterior.Liberar();
                    }
                }
            }

            // Si el paciente tenía una cama retenida y el destino es diferente, la liberamos del limbo
            if (cuentaActual.CamaRetenidaId.HasValue && (request.EsEgreso || request.NuevaAreaClinicaId != cuentaActual.CamaRetenidaId))
            {
                var camaRetenida = await _context.AreasClinicas
                    .FirstOrDefaultAsync(a => a.Id == cuentaActual.CamaRetenidaId.Value, cancellationToken);
                if (camaRetenida != null)
                {
                    camaRetenida.Liberar();
                }
            }

            // 3. Cerrar administrativamente la cuenta actual
            cuentaActual.Facturar(); // Cambia el estado a 'Facturada' y guarda la fecha de cierre

            Guid? parentCuentaId = cuentaActual.CuentaPrincipalId ?? cuentaActual.Id;
            Guid? nuevaCuentaId = null;

            // 4. Si no es un egreso definitivo (Alta), crear la nueva cuenta para la nueva ubicación
            if (!request.EsEgreso)
            {
                var nuevaCuenta = new CuentaServicios(
                    request.PacienteId,
                    request.UsuarioTraslado,
                    request.NuevoTipoIngreso, // ej: "Hospitalizacion", "Emergencia", etc.
                    request.NuevoConvenioId,
                    request.NuevaAreaClinicaId,
                    request.NuevaSubAreaClinica
                );

                // Enlazar a la cuenta principal
                nuevaCuenta.VincularCuentaPrincipal(parentCuentaId.Value);

                // Gestionar la retención de la cama anterior en la nueva cuenta
                bool esDestinoQuirofano = 
                    string.Equals(request.NuevoTipoIngreso, "Quirofano", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(request.NuevoTipoIngreso, "Quirófano", StringComparison.OrdinalIgnoreCase);

                if (esDestinoQuirofano)
                {
                    nuevaCuenta.AsignarCamaRetenida(cuentaActual.AreaClinicaId);
                }
                else if (cuentaActual.CamaRetenidaId.HasValue && request.NuevaAreaClinicaId != cuentaActual.CamaRetenidaId)
                {
                    nuevaCuenta.AsignarCamaRetenida(null);
                }
                else if (cuentaActual.CamaRetenidaId.HasValue && request.NuevaAreaClinicaId == cuentaActual.CamaRetenidaId)
                {
                    // Regresó a su cama de origen retenida, liberamos la marca de retención
                    nuevaCuenta.AsignarCamaRetenida(null);
                }

                // Ocupar la nueva cama si fue especificada
                AreaClinica? nuevaCama = null;
                if (request.NuevaAreaClinicaId.HasValue)
                {
                    nuevaCama = await _context.AreasClinicas
                        .Include(a => a.ServicioTarifaBase)
                        .FirstOrDefaultAsync(a => a.Id == request.NuevaAreaClinicaId.Value, cancellationToken);
                    if (nuevaCama != null)
                    {
                        nuevaCama.MarcarComoOcupada();
                    }
                }

                // Generar ítem de cargo en la nueva cuenta por el Traslado / Ingreso a la nueva área
                bool esAreaClinica = !string.IsNullOrEmpty(request.NuevoTipoIngreso) &&
                    (request.NuevoTipoIngreso.Equals("Emergencia", StringComparison.OrdinalIgnoreCase) ||
                     request.NuevoTipoIngreso.Equals("Hospitalizacion", StringComparison.OrdinalIgnoreCase) ||
                     request.NuevoTipoIngreso.Equals("Hospitalización", StringComparison.OrdinalIgnoreCase) ||
                     request.NuevoTipoIngreso.Equals("UCI", StringComparison.OrdinalIgnoreCase) ||
                     request.NuevoTipoIngreso.Equals("Quirofano", StringComparison.OrdinalIgnoreCase) ||
                     request.NuevoTipoIngreso.Equals("Quirófano", StringComparison.OrdinalIgnoreCase));

                if (esAreaClinica)
                {
                    bool esEmergencia = request.NuevoTipoIngreso.Equals("Emergencia", StringComparison.OrdinalIgnoreCase);
                    decimal precioTraslado = 0.00m;
                    Guid servicioId = Guid.Empty;
                    string? legacyMappingId = null;

                    if (!esEmergencia && nuevaCama?.ServicioTarifaBase != null)
                    {
                        precioTraslado = nuevaCama.ServicioTarifaBase.PrecioBase;
                        servicioId = nuevaCama.ServicioTarifaBase.Id;
                        legacyMappingId = nuevaCama.ServicioTarifaBase.LegacyMappingId;

                        if (request.NuevoConvenioId.HasValue)
                        {
                            var priceConv = await _context.PreciosServicioConvenio
                                .FirstOrDefaultAsync(p => p.SeguroConvenioId == request.NuevoConvenioId.Value 
                                                          && p.ServicioClinicoId == nuevaCama.ServicioTarifaBaseId.Value, cancellationToken);
                            if (priceConv != null)
                            {
                                precioTraslado = priceConv.PrecioDiferencial;
                            }
                        }
                    }
                    else if (nuevaCama?.ServicioTarifaBase != null)
                    {
                        servicioId = nuevaCama.ServicioTarifaBase.Id;
                        legacyMappingId = nuevaCama.ServicioTarifaBase.LegacyMappingId;
                    }

                    string descripcionCargo = $"Traslado a {request.NuevoTipoIngreso}" + (nuevaCama != null ? $" ({nuevaCama.Nombre})" : "");
                    
                    nuevaCuenta.AgregarServicio(
                        servicioId,
                        descripcionCargo,
                        precioTraslado,
                        0m, // Honorario
                        1m, // Cantidad
                        "Servicio",
                        request.UsuarioTraslado,
                        legacyMappingId,
                        nuevaCama?.Id
                    );
                }

                await _context.CuentasServicios.AddAsync(nuevaCuenta, cancellationToken);
                nuevaCuentaId = nuevaCuenta.Id;
            }

            // 5. Guardar cambios en base de datos
            await _context.SaveChangesAsync(cancellationToken);

            return new TrasladarPacienteResult
            {
                CuentaCerradaId = cuentaActual.Id,
                NuevaCuentaId = nuevaCuentaId,
                CuentaPrincipalId = parentCuentaId
            };
        }
    }
}
