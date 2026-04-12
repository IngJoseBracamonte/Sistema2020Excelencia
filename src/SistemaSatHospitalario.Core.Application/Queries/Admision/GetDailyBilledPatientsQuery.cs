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
    public class DailyBilledPatientDto
    {
        public Guid PacienteId { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public decimal TotalFacturado { get; set; }
        public int CuentasCerradas { get; set; }
    }

    public class GetDailyBilledPatientsQuery : IRequest<List<DailyBilledPatientDto>>
    {
        public DateTime Fecha { get; set; }
    }

    public class GetDailyBilledPatientsQueryHandler : IRequestHandler<GetDailyBilledPatientsQuery, List<DailyBilledPatientDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetDailyBilledPatientsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DailyBilledPatientDto>> Handle(GetDailyBilledPatientsQuery request, CancellationToken cancellationToken)
        {
            // Normalización Ultra-Agresiva para depuración (V12.1)
            // Tomamos lo que sea que esté facturado en el rango amplio (±12h para TZ resilience)
            var targetDay = request.Fecha.Date;
            var startRange = targetDay.AddHours(-12);
            var endRange = targetDay.AddHours(36);

            // Cargamos cuentas primero (Sin el JOIN para evitar exclusiones si hay huérfanos)
            var billedAccounts = await _context.CuentasServicios
                .AsNoTracking()
                .Where(c => c.Estado == EstadoConstants.Facturada && 
                            c.FechaCierre != null &&
                            c.FechaCierre >= startRange && 
                            c.FechaCierre <= endRange)
                .Select(c => new 
                {
                    c.Id,
                    c.PacienteId,
                    c.FechaCierre,
                    Monto = c.Detalles.Sum(d => d.Precio * d.Cantidad)
                })
                .ToListAsync(cancellationToken);

            if (!billedAccounts.Any()) return new List<DailyBilledPatientDto>();

            // Resolvemos pacientes en memoria (Evita problemas de JOIN/Collations)
            var patientIds = billedAccounts.Select(a => a.PacienteId).Distinct().ToList();
            var patients = await _context.PacientesAdmision
                .AsNoTracking()
                .Where(p => patientIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            return billedAccounts
                .GroupBy(a => a.PacienteId)
                .Select(g => 
                {
                    var p = patients.ContainsKey(g.Key) ? patients[g.Key] : null;
                    var fullName = p?.NombreCorto ?? "Paciente Huérfano";
                    var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    return new DailyBilledPatientDto
                    {
                        PacienteId = g.Key,
                        Cedula = p?.CedulaPasaporte ?? "S/N",
                        Nombre = nameParts.Length > 0 ? nameParts[0] : fullName,
                        Apellidos = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "",
                        TotalFacturado = Math.Round(g.Sum(x => x.Monto), 2),
                        CuentasCerradas = g.Count()
                    };
                })
                .OrderByDescending(p => p.TotalFacturado)
                .ToList();
        }
    }
}
