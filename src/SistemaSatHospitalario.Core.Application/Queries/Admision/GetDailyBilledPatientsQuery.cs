using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class DailyBilledPatientDto
    {
        public Guid PacienteId { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; } // En el sistema nuevo es NombreCorto, dejaremos esto para compatibilidad DTO
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
            // Normalización Profesional de Fecha: Tratamos la fecha como el día completo "Local del Usuario"
            var startOfDay = request.Fecha.Date;
            var nextDay = startOfDay.AddDays(1);

            // Búsqueda de cuentas cerradas con PROYECCIÓN SQL DIRECTA (.Select PRO)
            // Esto evita problemas de Include() y Grupos en Memoria (V11.11 Senior Design)
            var result = await _context.CuentasServicios
                .Where(c => c.Estado == "Facturada" && 
                             c.FechaCierre >= startOfDay && 
                             c.FechaCierre < nextDay)
                .Select(c => new 
                {
                    PacienteId = c.PacienteId,
                    Cedula = _context.PacientesAdmision.Where(p => p.Id == c.PacienteId).Select(p => p.CedulaPasaporte).FirstOrDefault(),
                    NombreCorto = _context.PacientesAdmision.Where(p => p.Id == c.PacienteId).Select(p => p.NombreCorto).FirstOrDefault(),
                    TotalCuenta = c.Detalles.Sum(d => d.Precio * d.Cantidad)
                })
                .ToListAsync(cancellationToken);

            // Post-procesamiento en memoria para formato de nombres (Diferente a la DB)
            return result
                .GroupBy(r => r.PacienteId)
                .Select(g => 
                {
                    var first = g.First();
                    var fullName = first.NombreCorto ?? "Paciente Desconocido";
                    
                    // Lógica Profesional de Desglose de Nombre:
                    var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var nombre = nameParts.Length > 0 ? nameParts[0] : fullName;
                    var apellidos = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

                    var totalFacturado = g.Sum(x => x.TotalCuenta);

                    return new DailyBilledPatientDto
                    {
                        PacienteId = g.Key,
                        Cedula = first.Cedula ?? "S/N",
                        Nombre = nombre,
                        Apellidos = apellidos,
                        TotalFacturado = Math.Round(totalFacturado, 2), // Regla 15: Precisión financiera
                        CuentasCerradas = g.Count()
                    };
                })
                .OrderByDescending(p => p.TotalFacturado)
                .ToList();
        }
    }
}
