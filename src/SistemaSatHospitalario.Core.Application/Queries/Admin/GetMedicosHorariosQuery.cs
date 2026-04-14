using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admin
{
    public class GetMedicosHorariosQuery : IRequest<List<MedicoHorarioDto>>
    {
    }

    public class MedicoHorarioDto
    {
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; }
        public string Especialidad { get; set; }
        public List<HorarioBloqueDto> Horarios { get; set; } = new();
    }

    public class HorarioBloqueDto
    {
        public Guid Id { get; set; }
        public int DiaSemana { get; set; }
        public string Inicio { get; set; } // HH:mm
        public string Fin { get; set; } // HH:mm
    }

    public class GetMedicosHorariosQueryHandler : IRequestHandler<GetMedicosHorariosQuery, List<MedicoHorarioDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetMedicosHorariosQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicoHorarioDto>> Handle(GetMedicosHorariosQuery request, CancellationToken cancellationToken)
        {
            var medicos = await _context.Medicos
                .Include(m => m.Especialidad)
                .ToListAsync(cancellationToken);

            var horarios = await _context.HorariosAtencionMedicos.ToListAsync(cancellationToken);

            return medicos.Select(m => new MedicoHorarioDto
            {
                MedicoId = m.Id,
                MedicoNombre = m.Nombre,
                Especialidad = m.Especialidad.Nombre,
                Horarios = horarios
                    .Where(h => h.MedicoId == m.Id)
                    .OrderBy(h => h.DiaSemana)
                    .ThenBy(h => h.HoraInicio)
                    .Select(h => new HorarioBloqueDto
                    {
                        Id = h.Id,
                        DiaSemana = h.DiaSemana,
                        Inicio = h.HoraInicio.ToString(@"hh\:mm"),
                        Fin = h.HoraFin.ToString(@"hh\:mm")
                    }).ToList()
            }).ToList();
        }
    }
}
