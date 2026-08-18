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
            // 1. Validar que el paciente existe por GUID o por ID Legacy
            var pacSearchStr = !string.IsNullOrWhiteSpace(request.RawPacienteId) ? request.RawPacienteId.Trim() : request.PacienteId.ToString();

            var paciente = await _context.PacientesAdmision
                .FirstOrDefaultAsync(p => p.Id == request.PacienteId, cancellationToken);

            if (paciente == null && int.TryParse(pacSearchStr, out var legacyId))
            {
                paciente = await _context.PacientesAdmision
                    .FirstOrDefaultAsync(p => p.IdPacienteLegacy == legacyId, cancellationToken);
            }

            if (paciente == null)
            {
                throw new InvalidOperationException($"El paciente con ID {pacSearchStr} no existe.");
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
                .FirstOrDefaultAsync(c => c.PacienteId == paciente.Id && 
                                           c.Estado == EstadoConstants.Abierta && 
                                           c.TipoIngreso == request.TipoIngreso, cancellationToken);

            if (cuentaExistente != null)
            {
                return cuentaExistente.Id; // Retornar la cuenta existente para evitar duplicaciones
            }

            // 4. Crear nueva cuenta clínica
            var nuevaCuenta = new CuentaServicios(
                paciente.Id,
                request.UsuarioCarga,
                request.TipoIngreso,
                request.ConvenioId,
                request.AreaClinicaId,
                null,
                request.MedicoId
            );

            // 5. Marcar la cama física como ocupada si fue especificada
            AreaClinica? cama = null;
            if (request.AreaClinicaId.HasValue)
            {
                cama = await _context.AreasClinicas
                    .Include(a => a.ServicioTarifaBase)
                    .FirstOrDefaultAsync(a => a.Id == request.AreaClinicaId.Value, cancellationToken);
                if (cama != null)
                {
                    cama.MarcarComoOcupada();
                }
            }

            // 6. Generar ítem de cargo inicial por Ingreso (Emergencia $0.00, Hospitalización / UCI según tarifa base)
            bool esIngresoClinico = !string.IsNullOrEmpty(request.TipoIngreso) &&
                (request.TipoIngreso.Equals("Emergencia", StringComparison.OrdinalIgnoreCase) ||
                 request.TipoIngreso.Equals("Hospitalizacion", StringComparison.OrdinalIgnoreCase) ||
                 request.TipoIngreso.Equals("Hospitalización", StringComparison.OrdinalIgnoreCase) ||
                 request.TipoIngreso.Equals("UCI", StringComparison.OrdinalIgnoreCase));

            if (esIngresoClinico)
            {
                bool esEmergencia = request.TipoIngreso.Equals("Emergencia", StringComparison.OrdinalIgnoreCase);
                decimal precioIngreso = 0.00m;
                Guid servicioId = Guid.Empty;
                string? legacyMappingId = null;

                if (!esEmergencia && cama?.ServicioTarifaBase != null)
                {
                    precioIngreso = cama.ServicioTarifaBase.PrecioBase;
                    servicioId = cama.ServicioTarifaBase.Id;
                    legacyMappingId = cama.ServicioTarifaBase.LegacyMappingId;

                    if (request.ConvenioId.HasValue)
                    {
                        var priceConv = await _context.PreciosServicioConvenio
                            .FirstOrDefaultAsync(p => p.SeguroConvenioId == request.ConvenioId.Value 
                                                      && p.ServicioClinicoId == cama.ServicioTarifaBaseId.Value, cancellationToken);
                        if (priceConv != null)
                        {
                            precioIngreso = priceConv.PrecioDiferencial;
                        }
                    }
                }
                else if (cama?.ServicioTarifaBase != null)
                {
                    servicioId = cama.ServicioTarifaBase.Id;
                    legacyMappingId = cama.ServicioTarifaBase.LegacyMappingId;
                }

                string descripcionCargo = $"Ingreso - {request.TipoIngreso}" + (cama != null ? $" ({cama.Nombre})" : "");

                nuevaCuenta.AgregarServicio(
                    servicioId,
                    descripcionCargo,
                    precioIngreso,
                    0m, // Honorario
                    1m, // Cantidad
                    "Servicio",
                    request.UsuarioCarga,
                    legacyMappingId,
                    cama?.Id
                );
            }

            await _context.CuentasServicios.AddAsync(nuevaCuenta, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return nuevaCuenta.Id;
        }
    }
}
