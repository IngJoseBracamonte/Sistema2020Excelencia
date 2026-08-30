using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegisterSurgicalPatientCommandHandler(IApplicationDbContext context)
        : IRequestHandler<RegisterSurgicalPatientCommand, PatientDto>
    {
        public async Task<PatientDto> Handle(RegisterSurgicalPatientCommand request, CancellationToken cancellationToken)
        {
            var cedula = request.Cedula.Trim().ToUpperInvariant();
            var nombre = request.Nombre.Trim();
            var apellidos = request.Apellidos.Trim();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                throw new ArgumentException("La cédula o pasaporte es obligatorio.", nameof(request.Cedula));
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del paciente es obligatorio.", nameof(request.Nombre));
            }

            if (request.FechaNacimiento.HasValue && request.FechaNacimiento.Value.Date > DateTime.UtcNow.Date)
            {
                throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(request.FechaNacimiento));
            }

            var existingPatient = await context.PacientesAdmision
                .AsNoTracking()
                .FirstOrDefaultAsync(patient => patient.CedulaPasaporte == cedula, cancellationToken);

            if (existingPatient is not null)
            {
                return Map(existingPatient);
            }

            var fullName = string.Join(' ', new[] { nombre, apellidos }.Where(value => !string.IsNullOrWhiteSpace(value)));
            var patient = new PacienteAdmision(
                cedula,
                fullName,
                string.IsNullOrWhiteSpace(request.Celular) ? null : request.Celular.Trim(),
                idLegacy: null,
                fechaNacimiento: request.FechaNacimiento,
                direccion: string.IsNullOrWhiteSpace(request.Direccion) ? null : request.Direccion.Trim());

            await context.PacientesAdmision.AddAsync(patient, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Map(patient);
        }

        private static PatientDto Map(PacienteAdmision patient) => new()
        {
            Id = patient.Id,
            IdPacienteLegacy = patient.IdPacienteLegacy,
            Cedula = patient.CedulaPasaporte,
            Nombre = patient.NombreCorto,
            Apellidos = string.Empty,
            Celular = patient.TelefonoContact ?? string.Empty,
            Telefono = patient.TelefonoContact ?? string.Empty,
            FechaNacimiento = patient.FechaNacimiento?.ToString("yyyy-MM-dd") ?? string.Empty,
            Direccion = patient.Direccion ?? string.Empty,
            EsLegacy = patient.IdPacienteLegacy.HasValue,
            Source = "Nativo"
        };
    }
}