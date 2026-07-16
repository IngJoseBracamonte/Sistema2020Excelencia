using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

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
                .FirstOrDefaultAsync(c => c.PacienteId == request.PacienteId && c.Estado == EstadoConstants.Abierta, cancellationToken);

            if (cuentaActual == null)
            {
                throw new InvalidOperationException($"El paciente con ID {request.PacienteId} no tiene una cuenta activa para realizar el traslado o egreso.");
            }

            // 2. Liberar la cama física anterior si el paciente estaba asignado a una
            if (cuentaActual.AreaClinicaId.HasValue)
            {
                var camaAnterior = await _context.AreasClinicas
                    .FirstOrDefaultAsync(a => a.Id == cuentaActual.AreaClinicaId.Value, cancellationToken);
                if (camaAnterior != null)
                {
                    camaAnterior.Liberar();
                }
            }

            // 3. Cerrar administrativamente la cuenta actual
            cuentaActual.Facturar(); // Cambia el estado a 'Facturada' y guarda la fecha de cierre

            Guid? parentCuentaId = cuentaActual.CuentaPrincipalId ?? cuentaActual.Id;
            Guid? nuevaCuentaId = null;

            // 4. Si no es un egreso definitivo (Alta), crear la nueva cuenta (hija) para la nueva ubicación
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

                // Ocupar la nueva cama si fue especificada
                if (request.NuevaAreaClinicaId.HasValue)
                {
                    var nuevaCama = await _context.AreasClinicas
                        .FirstOrDefaultAsync(a => a.Id == request.NuevaAreaClinicaId.Value, cancellationToken);
                    if (nuevaCama != null)
                    {
                        nuevaCama.MarcarComoOcupada();
                    }
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
