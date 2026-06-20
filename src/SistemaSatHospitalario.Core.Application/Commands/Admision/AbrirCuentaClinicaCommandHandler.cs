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
    public class AbrirCuentaClinicaCommandHandler : IRequestHandler<AbrirCuentaClinicaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public AbrirCuentaClinicaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Guid> Handle(AbrirCuentaClinicaCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar que el paciente existe
            var paciente = await _context.PacientesAdmision
                .FirstOrDefaultAsync(p => p.Id == request.PacienteId, cancellationToken);
            if (paciente == null)
            {
                throw new InvalidOperationException($"El paciente con ID {request.PacienteId} no existe.");
            }

            // 2. Buscar si ya tiene una cuenta abierta de ese tipo de ingreso
            var cuentaExistente = await _context.CuentasServicios
                .FirstOrDefaultAsync(c => c.PacienteId == request.PacienteId && 
                                           c.Estado == EstadoConstants.Abierta && 
                                           c.TipoIngreso == request.TipoIngreso, cancellationToken);

            if (cuentaExistente != null)
            {
                return cuentaExistente.Id; // Retornar la cuenta existente para evitar duplicaciones
            }

            // 3. Crear nueva cuenta clínica
            var nuevaCuenta = new CuentaServicios(
                request.PacienteId,
                request.UsuarioCarga,
                request.TipoIngreso,
                request.ConvenioId
            );

            await _context.CuentasServicios.AddAsync(nuevaCuenta, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return nuevaCuenta.Id;
        }
    }
}
