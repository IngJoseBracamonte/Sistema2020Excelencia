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

            // 2. Cerrar administrativamente la cuenta actual
            cuentaActual.Facturar(); // Cambia el estado a 'Facturada' y guarda la fecha de cierre

            Guid? parentCuentaId = cuentaActual.CuentaPrincipalId ?? cuentaActual.Id;
            Guid? nuevaCuentaId = null;

            // 3. Si no es un egreso definitivo (Alta), crear la nueva cuenta (hija) para la nueva ubicación
            if (!request.EsEgreso)
            {
                var nuevaCuenta = new CuentaServicios(
                    request.PacienteId,
                    request.UsuarioTraslado,
                    request.NuevoTipoIngreso, // ej: "Hospitalizacion", "Emergencia", etc.
                    request.NuevoConvenioId
                );

                // Enlazar a la cuenta principal
                nuevaCuenta.VincularCuentaPrincipal(parentCuentaId.Value);

                await _context.CuentasServicios.AddAsync(nuevaCuenta, cancellationToken);
                nuevaCuentaId = nuevaCuenta.Id;
            }

            // 4. Guardar cambios en base de datos
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
