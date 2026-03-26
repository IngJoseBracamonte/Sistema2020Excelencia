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
                Cedula = request.Cedula,
                Nombre = request.Nombre,
                Apellidos = request.Apellidos ?? "",
                Sexo = request.Sexo ?? "ND", 
                Fecha = !string.IsNullOrEmpty(request.FechaNacimiento) ? request.FechaNacimiento : DateTime.Now.AddYears(-30).ToString("yyyy-MM-dd"), 
                Correo = request.Correo ?? "",
                TipoCorreo = request.TipoCorreo ?? "",
                Celular = request.Celular ?? "",
                CodigoCelular = request.CodigoCelular ?? "",
                Telefono = request.Telefono ?? "",
                CodigoTelefono = request.CodigoTelefono ?? "",
                Visible = 1
            };

            int unifiedId = await _legacyRepository.CreatePatientLegacyAsync(legacyPatient, cancellationToken);

            // 4. Crear en Native usando el ID del Legacy para mantener paridad de Relaciones (OS, Facturas)
            var fullName = $"{request.Nombre} {request.Apellidos}".Trim();
            var mainPhone = !string.IsNullOrEmpty(request.Celular) ? request.Celular : request.Telefono;

            var newPatient = new PacienteAdmision(unifiedId, request.Cedula, fullName, mainPhone ?? "");
            await _context.PacientesAdmision.AddAsync(newPatient, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new PatientDto
            {
                Id = unifiedId,
                Cedula = legacyPatient.Cedula,
                Nombre = legacyPatient.Nombre,
                Apellidos = legacyPatient.Apellidos,
                Sexo = legacyPatient.Sexo,
                Correo = legacyPatient.Correo,
                Celular = legacyPatient.Celular,
                Source = "Legacy",
                EsLegacy = true
            };
        }
    }
}
