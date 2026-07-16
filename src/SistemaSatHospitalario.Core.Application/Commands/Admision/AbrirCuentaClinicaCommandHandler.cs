using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AbrirCuentaClinicaCommandHandler : IRequestHandler<AbrirCuentaClinicaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AbrirCuentaClinicaCommandHandler> _logger;

        public AbrirCuentaClinicaCommandHandler(IApplicationDbContext context, ILogger<AbrirCuentaClinicaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            // 2. Validar especialidad del médico si aplica
            if (request.MedicoId.HasValue && !string.IsNullOrEmpty(request.TipoIngreso))
            {
                bool isHospitalizacionOrUci = request.TipoIngreso.Equals("Hospitalizacion", StringComparison.OrdinalIgnoreCase) ||
                                              request.TipoIngreso.Equals("UCI", StringComparison.OrdinalIgnoreCase);
                if (isHospitalizacionOrUci)
                {
                    var doctor = await _context.Medicos
                        .Include(m => m.Especialidad)
                        .FirstOrDefaultAsync(m => m.Id == request.MedicoId.Value, cancellationToken);

                    if (doctor == null)
                    {
                        throw new InvalidOperationException($"El médico con ID {request.MedicoId} no existe.");
                    }

                    bool esUCI = request.TipoIngreso.Equals("UCI", StringComparison.OrdinalIgnoreCase);
                    bool specialtyIsValid = true;

                    if (esUCI)
                    {
                        var specName = doctor.Especialidad?.Nombre?.ToUpper() ?? "";
                        if (!specName.Contains("INTENSIVISTA") && !specName.Contains("CRITICO") && !specName.Contains("CRÍTICO"))
                        {
                            specialtyIsValid = false;
                        }
                    }
                    else
                    {
                        // Hospitalizacion validation
                        var specName = doctor.Especialidad?.Nombre?.ToUpper() ?? "";
                        if (specName.Contains("ODONTOLOG") || specName.Contains("DENTISTA") || specName.Contains("ESTETICA"))
                        {
                            specialtyIsValid = false;
                        }
                    }

                    if (!specialtyIsValid)
                    {
                        if (!request.PermitirBypassExcepcionMedica)
                        {
                            throw new InvalidOperationException($"El médico {doctor.Nombre} tiene la especialidad {doctor.Especialidad?.Nombre ?? "desconocida"}, la cual no es válida para el área de {request.TipoIngreso}.");
                        }
                        else
                        {
                            _logger.LogWarning("ADVERTENCIA DE SEGURIDAD CLÍNICA: Admisión a {TipoIngreso} con Médico Especialista no apto ({MedicoNombre} - {Especialidad}) bypassada por el usuario {Usuario}",
                                request.TipoIngreso, doctor.Nombre, doctor.Especialidad?.Nombre, request.UsuarioCarga);
                        }
                    }
                }
            }

            // 3. Buscar si ya tiene una cuenta abierta de ese tipo de ingreso
            var cuentaExistente = await _context.CuentasServicios
                .FirstOrDefaultAsync(c => c.PacienteId == request.PacienteId && 
                                           c.Estado == EstadoConstants.Abierta && 
                                           c.TipoIngreso == request.TipoIngreso, cancellationToken);

            if (cuentaExistente != null)
            {
                return cuentaExistente.Id; // Retornar la cuenta existente para evitar duplicaciones
            }

            // 4. Crear nueva cuenta clínica
            var nuevaCuenta = new CuentaServicios(
                request.PacienteId,
                request.UsuarioCarga,
                request.TipoIngreso,
                request.ConvenioId,
                request.AreaClinicaId,
                null,
                request.MedicoId
            );

            // 5. Marcar la cama física como ocupada si fue especificada
            if (request.AreaClinicaId.HasValue)
            {
                var cama = await _context.AreasClinicas
                    .FirstOrDefaultAsync(a => a.Id == request.AreaClinicaId.Value, cancellationToken);
                if (cama != null)
                {
                    cama.MarcarComoOcupada();
                }
            }

            await _context.CuentasServicios.AddAsync(nuevaCuenta, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return nuevaCuenta.Id;
        }
    }
}
