using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class EnviarASubAreaCommand : IRequest<EnviarASubAreaResponseDto>
    {
        public Guid InsumoId { get; set; }
        public Guid AreaClinicaId { get; set; }
        public string? NombreSubArea { get; set; }
        public decimal Cantidad { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
    }

    public class EnviarASubAreaResponseDto
    {
        public bool Success { get; set; }
        public Guid MovimientoId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class EnviarASubAreaCommandHandler : IRequestHandler<EnviarASubAreaCommand, EnviarASubAreaResponseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<EnviarASubAreaCommandHandler> _logger;

        public EnviarASubAreaCommandHandler(IApplicationDbContext context, ILogger<EnviarASubAreaCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EnviarASubAreaResponseDto> Handle(EnviarASubAreaCommand request, CancellationToken cancellationToken)
        {
            if (request.Cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a enviar debe ser mayor a cero.", nameof(request.Cantidad));
            }

            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == request.InsumoId, cancellationToken);
            if (insumo == null)
            {
                throw new KeyNotFoundException($"No se encontró el insumo con ID {request.InsumoId}.");
            }

            // Validación DB-Driven de la Sub-Área Clínica destino
            string subAreaNombreResolved = "Sub-Área";
            if (request.AreaClinicaId != Guid.Empty)
            {
                var areaClinica = await _context.AreasClinicas
                    .FirstOrDefaultAsync(a => a.Id == request.AreaClinicaId, cancellationToken);

                if (areaClinica != null)
                {
                    subAreaNombreResolved = string.IsNullOrWhiteSpace(areaClinica.Codigo)
                        ? areaClinica.Nombre
                        : $"[{areaClinica.Codigo}] {areaClinica.Nombre}";
                }
                else if (!string.IsNullOrWhiteSpace(request.NombreSubArea))
                {
                    subAreaNombreResolved = request.NombreSubArea;
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.NombreSubArea))
            {
                subAreaNombreResolved = request.NombreSubArea;
            }

            var sedePrincipalId = SeedConstants.SedeId_Principal;
            var stockPrincipal = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == request.InsumoId && s.SedeId == sedePrincipalId, cancellationToken);

            var stockDisponible = stockPrincipal?.StockActual ?? 0m;
            if (stockDisponible < request.Cantidad)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente en Almacén Principal para '{insumo.Nombre}'. Disponible: {stockDisponible} {insumo.UnidadMedidaBase}, Solicitado: {request.Cantidad}.");
            }

            // Descuento exclusivo del Almacén Principal (Salida definitiva / Consumo Interno)
            if (stockPrincipal == null)
            {
                stockPrincipal = new StockSede(request.InsumoId, sedePrincipalId, 0);
                _context.StocksSedes.Add(stockPrincipal);
            }

            stockPrincipal.RegistrarMovimientoStock(-request.Cantidad, insumo.PermiteFraccionamiento);

            // Registro inmutable de la salida en MovimientosInsumo
            var motivoDetallado = string.IsNullOrWhiteSpace(request.Motivo)
                ? $"Envío directo a [{subAreaNombreResolved}]"
                : $"Envío directo a [{subAreaNombreResolved}]: {request.Motivo.Trim()}";

            var movimiento = new MovimientoInsumo(
                request.InsumoId,
                sedePrincipalId,
                "EnvioSubArea",
                request.Cantidad,
                insumo.UnidadMedidaBase,
                request.Cantidad,
                request.Usuario,
                motivoDetallado
            );

            _context.MovimientosInsumo.Add(movimiento);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "[ENVIO SUBAREA] Se despacharon {Cantidad} {Unidad} de '{Insumo}' a la sub-área [{SubArea}] por usuario {Usuario}.",
                request.Cantidad, insumo.UnidadMedidaBase, insumo.Nombre, subAreaNombreResolved, request.Usuario);

            return new EnviarASubAreaResponseDto
            {
                Success = true,
                MovimientoId = movimiento.Id,
                Message = $"Despacho directo de {request.Cantidad} unidades de '{insumo.Nombre}' a [{subAreaNombreResolved}] registrado exitosamente."
            };
        }
    }
}
