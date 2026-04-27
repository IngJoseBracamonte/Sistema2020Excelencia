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
    public class MonitoringOrderDto
    {
        public Guid CuentaId { get; set; }
        public int LegacyOrderId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteCedula { get; set; }
        public DateTime FechaCarga { get; set; }
        public string Estado { get; set; } // PENDIENTE, PROCESADA
    }

    public class GetMonitoringOrdersQuery : IRequest<List<MonitoringOrderDto>> { }

    public class GetMonitoringOrdersQueryHandler : IRequestHandler<GetMonitoringOrdersQuery, List<MonitoringOrderDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetMonitoringOrdersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MonitoringOrderDto>> Handle(GetMonitoringOrdersQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            return await _context.CuentasServicios
                .AsNoTracking()
                .Include(c => c.Paciente)
                .Where(c => c.LegacyOrderId.HasValue && c.FechaCarga >= today)
                .OrderByDescending(c => c.FechaCarga)
                .Select(c => new MonitoringOrderDto
                {
                    CuentaId = c.Id,
                    LegacyOrderId = c.LegacyOrderId!.Value,
                    PacienteNombre = c.Paciente.NombreCorto,
                    PacienteCedula = c.Paciente.CedulaPasaporte,
                    FechaCarga = c.FechaCarga,
                    Estado = c.ProcesamientoEstado ?? "PENDIENTE"
                })
                .Take(50) // Limitar a las últimas 50 órdenes
                .ToListAsync(cancellationToken);
        }
    }
}
