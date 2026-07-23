using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegistrarCambioCamaCommandHandler : IRequestHandler<RegistrarCambioCamaCommand, RegistrarCambioCamaResult>
    {
        private readonly IApplicationDbContext _context;

        public RegistrarCambioCamaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RegistrarCambioCamaResult> Handle(RegistrarCambioCamaCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _context.CuentasServicios
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

            // 3. Actualiza la ubicación en la cuenta activa (Sin cargos financieros, $0.00 USD)
            cuenta.AsignarAreaClinica(camaDestino.Id, camaDestino.Nombre);

            await _context.SaveChangesAsync(cancellationToken);

            return new RegistrarCambioCamaResult
            {
                CuentaId = cuenta.Id,
                CamaDestinoId = camaDestino.Id,
                Exitoso = true
            };
        }
    }
}
