using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admin
{
    public class GetServiciosSinAsignarQuery : IRequest<List<ServicioSinAsignarDto>>
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? EstadoFiltro { get; set; } // PENDIENTE, ASIGNADO, TODOS
    }

    public class ServicioSinAsignarDto
    {
        public Guid DetalleId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public decimal Honorario { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public DateTime FechaCarga { get; set; }
        public Guid? MedicoAsignadoId { get; set; }
        public string? MedicoAsignadoNombre { get; set; }
        public string? CategoriaHonorario { get; set; }
        public bool EsAutoAsignado { get; set; }
    }

    public class GetServiciosSinAsignarQueryHandler : IRequestHandler<GetServiciosSinAsignarQuery, List<ServicioSinAsignarDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetServiciosSinAsignarQueryHandler(IApplicationDbContext context) { _context = context; }

        public async Task<List<ServicioSinAsignarDto>> Handle(GetServiciosSinAsignarQuery request, CancellationToken ct)
        {
            var excluirLista = new List<string> { "Laboratorio", "LAB", "INSUMO", "Insumo" };
            var query = _context.DetallesServicioCuenta
                .Include(d => d.CuentaServicio).ThenInclude(c => c.Paciente)
                .Where(d => !excluirLista.Contains(d.TipoServicio) && d.Honorario > 0)
                .AsQueryable();

            if (request.FechaDesde.HasValue)
            {
                var startDate = request.FechaDesde.Value.Date;
                query = query.Where(d => d.FechaCarga >= startDate);
            }

            if (request.FechaHasta.HasValue)
            {
                var endDate = request.FechaHasta.Value.Date.AddDays(1);
                query = query.Where(d => d.FechaCarga < endDate);
            }

            if (request.EstadoFiltro == "PENDIENTE")
                query = query.Where(d => d.MedicoResponsableId == null);
            else if (request.EstadoFiltro == "ASIGNADO")
                query = query.Where(d => d.MedicoResponsableId != null);

            var data = await query.OrderByDescending(d => d.FechaCarga)
                .Select(d => new ServicioSinAsignarDto
                {
                    DetalleId = d.Id,
                    Descripcion = d.Descripcion,
                    TipoServicio = d.TipoServicio,
                    Honorario = d.Honorario,
                    PacienteNombre = d.CuentaServicio.Paciente.NombreCorto,
                    FechaCarga = d.FechaCarga,
                    MedicoAsignadoId = d.MedicoResponsableId,
                    CategoriaHonorario = d.CategoriaHonorario
                }).ToListAsync(ct);

            // Enrich with medico names
            var medicoIds = data.Where(d => d.MedicoAsignadoId.HasValue).Select(d => d.MedicoAsignadoId.Value).Distinct().ToList();
            var medicos = await _context.Medicos.Where(m => medicoIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id, m => m.Nombre, ct);
            foreach (var item in data)
            {
                if (item.MedicoAsignadoId.HasValue && medicos.ContainsKey(item.MedicoAsignadoId.Value))
                    item.MedicoAsignadoNombre = medicos[item.MedicoAsignadoId.Value];
            }

            return data;
        }
    }
}
