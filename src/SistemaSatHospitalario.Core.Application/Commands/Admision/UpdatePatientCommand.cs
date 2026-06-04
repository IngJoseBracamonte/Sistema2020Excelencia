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
        public string Celular { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
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
            patient.ActualizarDatos(fullName, request.Celular, request.FechaNacimiento ?? patient.FechaNacimiento, request.Cedula);
            
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

                    // Email split
                    string email = request.Correo ?? "";
                    string emailDomain = "";
                    int atIndex = email.IndexOf('@');
                    if (atIndex >= 0)
                    {
                        emailDomain = email.Substring(atIndex);
                        email = email.Substring(0, atIndex);
                    }
                    legacyPatient.Correo = email;
                    legacyPatient.TipoCorreo = emailDomain;

                    // Phone split
                    string phone = request.Celular ?? "";
                    string phonePrefix = "";
                    if (phone.Length >= 4)
                    {
                        phonePrefix = phone.Substring(0, 4);
                        phone = phone.Substring(4);
                    }
                    legacyPatient.Celular = phone;
                    legacyPatient.CodigoCelular = phonePrefix;

                    await _legacyRepository.UpdatePatientLegacyAsync(legacyPatient, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
