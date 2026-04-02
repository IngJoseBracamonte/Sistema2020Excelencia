using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetActiveAppointmentsQuery : IRequest<List<ActiveAppointmentDto>>
    {
        public DateTime? Fecha { get; set; }
        public Guid? MedicoId { get; set; }
    }

    public class ActiveAppointmentDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = EstadoConstants.TypeCita; // "Cita", "Reserva"
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; } = string.Empty;
        public string MedicoEspecialidad { get; set; } = string.Empty;
        public Guid PacienteId { get; set; }
        public int? PacienteLegacyId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public DateTime HoraPautada { get; set; }
        public Guid? CuentaId { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class GetActiveAppointmentsQueryHandler : IRequestHandler<GetActiveAppointmentsQuery, List<ActiveAppointmentDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetActiveAppointmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActiveAppointmentDto>> Handle(GetActiveAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var targetDate = request.Fecha?.Date;
            var nextDay = targetDate?.AddDays(1);

            // 1. Obtener Citas (Excluyendo canceladas)
            var citasQuery = _context.CitasMedicas.AsQueryable();
            if (targetDate.HasValue) citasQuery = citasQuery.Where(c => c.HoraPautada >= targetDate.Value && c.HoraPautada < nextDay!.Value);
            if (request.MedicoId.HasValue) citasQuery = citasQuery.Where(c => c.MedicoId == request.MedicoId.Value);

            var citas = await (from c in citasQuery
                               join m in _context.Medicos on c.MedicoId equals m.Id into mGroup
                               from m in mGroup.DefaultIfEmpty()
                               join p in _context.PacientesAdmision on c.PacienteId equals p.Id into pGroup
                               from p in pGroup.DefaultIfEmpty()
                               select new ActiveAppointmentDto
                               {
                                   Id = c.Id,
                                   Type = EstadoConstants.TypeCita,
                                   MedicoId = c.MedicoId,
                                   MedicoNombre = m != null ? m.Nombre : "ID: " + c.MedicoId.ToString().Substring(0, 8),
                                   MedicoEspecialidad = m != null ? m.Especialidad.Nombre : EstadoConstants.NoVinculado,
                                   PacienteId = c.PacienteId,
                                   PacienteLegacyId = p != null ? p.IdPacienteLegacy : null,
                                   PacienteNombre = p != null ? p.NombreCorto : "Paciente #" + c.PacienteId,
                                   HoraPautada = c.HoraPautada,
                                   CuentaId = c.CuentaServicioId,
                                   Estado = c.Estado
                               })
                               .ToListAsync(cancellationToken);

            // 2. Obtener Reservas Temporales (Vigentes)
            var reservasQuery = _context.ReservasTemporales.Where(r => r.ExpiracionUtc > DateTime.UtcNow);
            if (targetDate.HasValue) reservasQuery = reservasQuery.Where(r => r.HoraPautada >= targetDate.Value && r.HoraPautada < nextDay!.Value);
            if (request.MedicoId.HasValue) reservasQuery = reservasQuery.Where(r => r.MedicoId == request.MedicoId.Value);

            var reservas = await (from r in reservasQuery
                                  join m in _context.Medicos on r.MedicoId equals m.Id into mGroup
                                  from m in mGroup.DefaultIfEmpty()
                                  select new ActiveAppointmentDto
                                  {
                                      Id = r.Id,
                                   Type = EstadoConstants.TypeReserva,
                                   MedicoId = r.MedicoId,
                                   MedicoNombre = m != null ? m.Nombre : "ID: " + r.MedicoId.ToString().Substring(0, 8),
                                   MedicoEspecialidad = m != null ? m.Especialidad.Nombre : EstadoConstants.NoVinculado,
                                   PacienteId = Guid.Empty,
                                   PacienteNombre = EstadoConstants.ReservaTemporal,
                                   HoraPautada = r.HoraPautada,
                                   Estado = EstadoConstants.Reservado
                                  })
                                  .ToListAsync(cancellationToken);

            return citas.Concat(reservas).OrderBy(x => x.HoraPautada).ToList();
        }
    }
}
