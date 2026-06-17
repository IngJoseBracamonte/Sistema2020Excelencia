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
            var nativePatient = await _context.PacientesAdmision
                .FirstOrDefaultAsync(p => p.CedulaPasaporte == request.Cedula, cancellationToken);
            
            if (nativePatient != null) 
                throw new InvalidOperationException("El paciente ya se encuentra registrado localmente.");

            // 2. Validar duplicados en Legacy y Onboard si es necesario
            var existingLegacy = await _legacyRepository.GetPatientByCedulaAsync(request.Cedula, cancellationToken);
            int unifiedId;

            if (existingLegacy != null) 
            {
                // El paciente ya existe en Legacy, procedemos a Nativizarlo directamente (V11.8)
                unifiedId = existingLegacy.IdPersona;
                var fullName = $"{existingLegacy.Nombre} {existingLegacy.Apellidos}".Trim();
                var mainPhone = !string.IsNullOrEmpty(existingLegacy.Celular) ? existingLegacy.Celular : existingLegacy.Telefono;
                
                DateTime? dob = null;
                if (DateTime.TryParse(existingLegacy.Fecha, out var parsedDob)) dob = parsedDob;

                nativePatient = new PacienteAdmision(existingLegacy.Cedula, fullName, mainPhone ?? "", unifiedId, dob, existingLegacy.Direccion);
                await _context.PacientesAdmision.AddAsync(nativePatient, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else 
            {
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
                    Direccion = request.Direccion ?? ""
                };

                unifiedId = await _legacyRepository.CreatePatientLegacyAsync(legacyPatient, cancellationToken);

                // 4. Crear en Native usando el ID del Legacy para mantener paridad de Relaciones (OS, Facturas)
                var fullName = $"{request.Nombre} {request.Apellidos}".Trim();
                var mainPhone = !string.IsNullOrEmpty(request.Celular) ? request.Celular : request.Telefono;

                DateTime? dob = null;
                if (DateTime.TryParse(request.FechaNacimiento, out var parsedDob)) dob = parsedDob;

                nativePatient = new PacienteAdmision(request.Cedula, fullName, mainPhone ?? "", unifiedId, dob, request.Direccion);
                await _context.PacientesAdmision.AddAsync(nativePatient, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new PatientDto
            {
                Id = nativePatient.Id,
                IdPacienteLegacy = unifiedId,
                Cedula = nativePatient.CedulaPasaporte,
                Nombre = existingLegacy != null ? existingLegacy.Nombre : request.Nombre,
                Apellidos = existingLegacy != null ? (existingLegacy.Apellidos ?? "") : (request.Apellidos ?? ""),
                Sexo = existingLegacy != null ? (existingLegacy.Sexo ?? "ND") : (request.Sexo ?? "ND"),
                Correo = existingLegacy != null ? (existingLegacy.Correo ?? "") : (request.Correo ?? ""),
                TipoCorreo = existingLegacy != null ? (existingLegacy.TipoCorreo ?? "") : (request.TipoCorreo ?? ""),
                Celular = existingLegacy != null ? (existingLegacy.Celular ?? "") : (request.Celular ?? ""),
                CodigoCelular = existingLegacy != null ? (existingLegacy.CodigoCelular ?? "") : (request.CodigoCelular ?? ""),
                Telefono = existingLegacy != null ? (existingLegacy.Telefono ?? "") : (request.Telefono ?? ""),
                CodigoTelefono = existingLegacy != null ? (existingLegacy.CodigoTelefono ?? "") : (request.CodigoTelefono ?? ""),
                FechaNacimiento = existingLegacy != null ? (existingLegacy.Fecha ?? "") : (request.FechaNacimiento ?? ""),
                Direccion = nativePatient.Direccion ?? "",
                Source = "Legacy",
                EsLegacy = true
            };
        }
    }
}
