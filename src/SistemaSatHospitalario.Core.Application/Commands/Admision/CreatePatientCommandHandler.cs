using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;

        public CreatePatientCommandHandler(IApplicationDbContext context, ILegacyLabRepository legacyRepository)
        {
            _context = context;
            _legacyRepository = legacyRepository;
        }

        public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar duplicados en local
            var existingNative = await _context.PacientesAdmision
                .AnyAsync(p => p.CedulaPasaporte == request.Cedula, cancellationToken);
            if (existingNative) throw new InvalidOperationException("El paciente ya se encuentra registrado localmente.");

            // 2. Validar duplicados en Legacy
            var existingLegacy = await _legacyRepository.GetPatientByCedulaAsync(request.Cedula, cancellationToken);
            if (existingLegacy != null) throw new InvalidOperationException("El paciente ya existe en el sistema Legacy. Use la búsqueda para seleccionarlo.");

            // 3. Crear primero en Legacy para obtener el IdPersona (ID Numérico unificado)
            var legacyPatient = new DatosPersonalesLegacy
            {
                Identificacion = request.Cedula,
                Nombre1 = request.Nombre,
                Apellido1 = "",
                Telefono = request.Telefono,
                FechaNacimiento = DateTime.Now.AddYears(-30) // Default o solicitado por UI
            };

            int unifiedId = await _legacyRepository.CreatePatientLegacyAsync(legacyPatient, cancellationToken);

            // 4. Crear en Native usando el ID del Legacy para mantener paridad
            var newPatient = new PacienteAdmision(unifiedId, request.Cedula, request.Nombre, request.Telefono);
            await _context.PacientesAdmision.AddAsync(newPatient, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new PatientDto
            {
                Id = newPatient.Id,
                Cedula = newPatient.CedulaPasaporte,
                Nombre = newPatient.NombreCorto,
                Apellidos = "",
                Celular = newPatient.TelefonoContact,
                Source = "Nativo",
                EsLegacy = false
            };
        }
    }
}
