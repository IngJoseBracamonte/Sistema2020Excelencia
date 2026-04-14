using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetControlCitasQuery : IRequest<List<ControlCitasDto>>
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public string? SectorName { get; set; } // Opcional: Filtrar por Especialidad o Medico
    }

    public class GetControlCitasQueryHandler : IRequestHandler<GetControlCitasQuery, List<ControlCitasDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetControlCitasQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ControlCitasDto>> Handle(GetControlCitasQuery request, CancellationToken cancellationToken)
        {
            var date = request.Date.Date;
            
            // 1. Obtener todas las citas del día con sus relaciones base
            var citas = await _context.CitasMedicas
                .Include(c => c.Medico)
                    .ThenInclude(m => m.Especialidad)
                .Include(c => c.CuentaServicio)
                .Where(c => c.HoraPautada.Date == date)
                .OrderBy(c => c.HoraPautada)
                .ToListAsync(cancellationToken);

            // 2. Obtener IDs de pacientes y cuentas para joins en memoria (más eficiente que joins complejos en DB para este volumen)
            var pacienteIds = citas.Select(c => c.PacienteId).Distinct().ToList();
            var cuentaIds = citas.Select(c => c.CuentaServicioId).Distinct().ToList();

            var pacientes = await _context.PacientesAdmision
                .Where(p => pacienteIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var montosCuentas = await _context.DetallesServicioCuenta
                .Where(d => cuentaIds.Contains(d.CuentaServicioId))
                .GroupBy(d => d.CuentaServicioId)
                .Select(g => new { CuentaId = g.Key, Total = g.Sum(x => x.Precio * x.Cantidad) })
                .ToDictionaryAsync(x => x.CuentaId, x => x.Total, cancellationToken);

            var conveniosMap = await _context.SegurosConvenios
                .ToDictionaryAsync(s => s.Id, s => s.Nombre, cancellationToken);

            // 3. Mapeo con lógica de Turnos (Agrupado por Medico)
            var result = new List<ControlCitasDto>();
            var turnosContador = new Dictionary<Guid, int>(); // MedicoId -> UltimoTurno

            foreach (var cita in citas)
            {
                if (!pacientes.TryGetValue(cita.PacienteId, out var paciente)) continue;

                // Incrementar turno para este médico
                if (!turnosContador.ContainsKey(cita.MedicoId)) turnosContador[cita.MedicoId] = 0;
                turnosContador[cita.MedicoId]++;

                int? edad = null;
                if (paciente.FechaNacimiento.HasValue)
                {
                    edad = DateTime.Today.Year - paciente.FechaNacimiento.Value.Year;
                    if (paciente.FechaNacimiento.Value.Date > DateTime.Today.AddYears(-edad.Value)) edad--;
                }

                montosCuentas.TryGetValue(cita.CuentaServicioId, out var monto);
                
                string formaPago = "PARTICULAR";
                if (cita.CuentaServicio?.ConvenioId.HasValue == true && conveniosMap.TryGetValue(cita.CuentaServicio.ConvenioId.Value, out var convenioNombre))
                {
                    formaPago = convenioNombre;
                }

                result.Add(new ControlCitasDto
                {
                    Id = cita.Id,
                    Hora = cita.HoraPautada,
                    PacienteNombre = paciente.NombreCorto,
                    PacienteCedula = paciente.CedulaPasaporte,
                    PacienteTelefono = paciente.TelefonoContact,
                    PacienteEdad = edad,
                    Especialidad = cita.Medico.Especialidad.Nombre,
                    Medico = cita.Medico.Nombre,
                    FormaPago = formaPago,
                    MontoUSD = monto,
                    Estado = cita.Estado,
                    Observaciones = cita.Comentario ?? "",
                    Turno = turnosContador[cita.MedicoId],
                    CuentaServicioId = cita.CuentaServicioId
                });
            }

            return result;
        }
    }
}
