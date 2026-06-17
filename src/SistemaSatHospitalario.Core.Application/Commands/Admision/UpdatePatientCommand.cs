using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdatePatientCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string TipoCorreo { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string CodigoCelular { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CodigoTelefono { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string Direccion { get; set; } = string.Empty;
    }

    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;

        public UpdatePatientCommandHandler(IApplicationDbContext context, ILegacyLabRepository legacyRepository)
        {
            _context = context;
            _legacyRepository = legacyRepository;
        }

        public async Task<bool> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            // 1. Obtener el paciente nativo
            var patient = await _context.PacientesAdmision
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (patient == null)
            {
                throw new InvalidOperationException("El paciente no existe en el sistema.");
            }

            // 2. Validar duplicidad de cédula nativa
            if (patient.CedulaPasaporte != request.Cedula)
            {
                var dup = await _context.PacientesAdmision
                    .AnyAsync(p => p.CedulaPasaporte == request.Cedula && p.Id != request.Id, cancellationToken);
                if (dup)
                {
                    throw new InvalidOperationException("La cédula ingresada ya está registrada con otro paciente.");
                }
            }

            // 3. Actualizar localmente
            var fullName = $"{request.Nombre} {request.Apellidos}".Trim();
            var mainPhone = !string.IsNullOrEmpty(request.Celular) 
                ? (request.CodigoCelular + request.Celular) 
                : (!string.IsNullOrEmpty(request.Telefono) ? (request.CodigoTelefono + request.Telefono) : "");

            patient.ActualizarDatos(fullName, mainPhone, request.FechaNacimiento ?? patient.FechaNacimiento, request.Cedula, request.Direccion);
            
            // 4. Si tiene vinculación legacy, actualizar en MySQL
            if (patient.IdPacienteLegacy.HasValue)
            {
                var legacyIdStr = patient.IdPacienteLegacy.Value.ToString();
                var legacyPatient = await _legacyRepository.GetPatientByIdAsync(legacyIdStr, cancellationToken);
                if (legacyPatient != null)
                {
                    legacyPatient.Cedula = request.Cedula;
                    legacyPatient.Nombre = request.Nombre;
                    legacyPatient.Apellidos = request.Apellidos;
                    legacyPatient.Sexo = request.Sexo ?? "ND";
                    
                    if (request.FechaNacimiento.HasValue)
                    {
                        legacyPatient.Fecha = request.FechaNacimiento.Value.ToString("yyyy-MM-dd");
                    }

                    legacyPatient.Correo = request.Correo ?? "";
                    legacyPatient.TipoCorreo = request.TipoCorreo ?? "";
                    legacyPatient.Celular = request.Celular ?? "";
                    legacyPatient.CodigoCelular = request.CodigoCelular ?? "";
                    legacyPatient.Telefono = request.Telefono ?? "";
                    legacyPatient.CodigoTelefono = request.CodigoTelefono ?? "";
                    legacyPatient.Direccion = request.Direccion ?? "";

                    await _legacyRepository.UpdatePatientLegacyAsync(legacyPatient, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
