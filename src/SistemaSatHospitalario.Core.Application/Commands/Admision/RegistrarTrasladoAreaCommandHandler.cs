using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegistrarTrasladoAreaCommandHandler : IRequestHandler<RegistrarTrasladoAreaCommand, RegistrarTrasladoAreaResult>
    {
        private readonly IApplicationDbContext _context;

        public RegistrarTrasladoAreaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RegistrarTrasladoAreaResult> Handle(RegistrarTrasladoAreaCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == request.CuentaId && c.Estado == EstadoConstants.Abierta, cancellationToken);

            if (cuenta == null)
            {
                throw new InvalidOperationException($"No se encontró una cuenta activa con ID {request.CuentaId}.");
            }

            // 1. Libera la cama de origen si existía
            if (cuenta.AreaClinicaId.HasValue)
            {
                var camaOrigen = await _context.AreasClinicas
                    .FirstOrDefaultAsync(a => a.Id == cuenta.AreaClinicaId.Value, cancellationToken);
                if (camaOrigen != null)
                {
                    camaOrigen.Liberar();
                }
            }

            // 2. Ocupa la cama destino
            var camaDestino = await _context.AreasClinicas
                .FirstOrDefaultAsync(a => a.Id == request.CamaDestinoId, cancellationToken);

            if (camaDestino == null)
            {
                throw new InvalidOperationException($"No se encontró la cama destino con ID {request.CamaDestinoId}.");
            }

            camaDestino.MarcarComoOcupada();

            // 3. Actualiza el área clínica y subárea en la cuenta
            cuenta.AsignarAreaClinica(camaDestino.Id, request.AreaDestino);

            // 4. Si cambia el médico tratante, actualizar el médico asignado a la cuenta
            if (request.CambiaMedicoTratante && request.NuevoMedicoId.HasValue && request.NuevoMedicoId.Value != Guid.Empty)
            {
                cuenta.AsignarMedico(request.NuevoMedicoId.Value);
            }

            // 5. Vincular al Servicio de Catálogo (Maestro de Servicios - HOSPITALARIO) para Métricas y Facturación
            Guid? detalleId = null;
            if (request.MontoACobrarUsd > 0)
            {
                string codigoServicioTarget = request.AreaDestino?.ToUpperInvariant() switch
                {
                    "EMERGENCIA" => "HOSP-EMG-01",
                    "HOSPITALIZACION" => "HOSP-HOS-01",
                    "UCI" => "HOSP-UCI-01",
                    _ => "HOSP-HOS-01"
                };

                var servicioCatalogo = await _context.ServiciosClinicos
                    .FirstOrDefaultAsync(s => s.Codigo == codigoServicioTarget, cancellationToken);

                // Fallback si no se encuentra por código exacto: buscar el primero de tipo Hospitalario
                if (servicioCatalogo == null)
                {
                    servicioCatalogo = await _context.ServiciosClinicos
                        .FirstOrDefaultAsync(s => s.TipoServicio == "Hospitalario" || s.HonorariumCategory == "HOSPITALARIO", cancellationToken);
                }

                Guid servicioId = servicioCatalogo?.Id ?? Guid.NewGuid();
                string descripcionCargo = servicioCatalogo != null
                    ? $"{servicioCatalogo.Descripcion} ({request.AreaDestino} - {request.CantidadHoras}h)"
                    : $"Cargo por Traslado a {request.AreaDestino} ({request.CantidadHoras}h)";

                if (!string.IsNullOrWhiteSpace(request.Observacion))
                {
                    descripcionCargo += $" - Obs: {request.Observacion}";
                }

                var nuevoDetalle = cuenta.AgregarServicio(
                    servicioId,
                    descripcionCargo,
                    request.MontoACobrarUsd,
                    0, // Honorario médico base
                    1, // Cantidad
                    servicioCatalogo?.TipoServicio ?? "Hospitalario",
                    request.UsuarioTraslado,
                    servicioCatalogo?.LegacyMappingId,
                    camaDestino.Id
                );

                await _context.DetallesServicioCuenta.AddAsync(nuevoDetalle, cancellationToken);
                detalleId = nuevoDetalle.Id;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new RegistrarTrasladoAreaResult
            {
                CuentaId = cuenta.Id,
                CamaDestinoId = camaDestino.Id,
                DetalleCargoId = detalleId,
                MontoCargadoUsd = request.MontoACobrarUsd,
                Exitoso = true
            };
        }
    }
}
