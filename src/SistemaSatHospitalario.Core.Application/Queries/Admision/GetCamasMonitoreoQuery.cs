using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetCamasMonitoreoQuery : IRequest<List<CamaMonitoreoDto>>
    {
    }

    public class GetCamasMonitoreoQueryHandler : IRequestHandler<GetCamasMonitoreoQuery, List<CamaMonitoreoDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCamasMonitoreoQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CamaMonitoreoDto>> Handle(GetCamasMonitoreoQuery request, CancellationToken cancellationToken)
        {
            // 1. Obtener todas las camas activas en el sistema
            var camas = await _context.AreasClinicas
                .Include(a => a.Sede)
                .Where(a => a.Activo)
                .ToListAsync(cancellationToken);

            // 2. Obtener todas las cuentas abiertas vinculadas a alguna cama
            var cuentasAbiertas = await _context.CuentasServicios
                .Include(c => c.Paciente)
                .Include(c => c.Detalles)
                .Where(c => c.Estado == "Abierta" && c.AreaClinicaId != null)
                .ToListAsync(cancellationToken);

            var result = new List<CamaMonitoreoDto>();

            foreach (var cama in camas)
            {
                // Buscar si la cama está ocupada por una cuenta abierta
                var cuentaAsociada = cuentasAbiertas.FirstOrDefault(c => c.AreaClinicaId == cama.Id);
                var estaOcupada = cuentaAsociada != null;

                var dto = new CamaMonitoreoDto
                {
                    CamaId = cama.Id,
                    Codigo = cama.Codigo,
                    Nombre = cama.Nombre,
                    SedeNombre = cama.Sede?.Nombre ?? "Sede General",
                    Estado = estaOcupada ? "Ocupada" : "Disponible",
                    EsAreaAdmision = cama.EsAreaAdmision
                };

                if (estaOcupada && cuentaAsociada != null)
                {
                    dto.PacienteId = cuentaAsociada.PacienteId;
                    dto.PacienteNombre = cuentaAsociada.Paciente != null 
                        ? cuentaAsociada.Paciente.NombreCorto
                        : "Paciente Desconocido";
                    dto.PacienteCedula = cuentaAsociada.Paciente?.CedulaPasaporte;
                    dto.FechaIngreso = cuentaAsociada.FechaCarga;
                    dto.CuentaId = cuentaAsociada.Id;

                    // Calcular total respetando la regla All-Inclusive
                    dto.TotalFacturado = cuentaAsociada.Detalles.Sum(d => 
                        d.IncluidoEnTarifaBase ? 0.00m : ((d.Precio + d.Honorario) * d.Cantidad)
                    );

                    // Mapear cargos detallados
                    dto.DetallesCargos = cuentaAsociada.Detalles.Select(d => new CamaMonitoreoDetalleCargoDto
                    {
                        Descripcion = d.Descripcion,
                        Precio = d.Precio,
                        Honorario = d.Honorario,
                        Cantidad = d.Cantidad,
                        IncluidoEnTarifaBase = d.IncluidoEnTarifaBase,
                        Total = d.IncluidoEnTarifaBase ? 0.00m : ((d.Precio + d.Honorario) * d.Cantidad)
                    }).ToList();

                    // Cargar historial de triages asociados al paciente de forma global
                    var pacienteId = cuentaAsociada.PacienteId;
                    var triages = await _context.TriagesEnfermeria
                        .Where(t => t.CuentaServicio.PacienteId == pacienteId)
                        .OrderByDescending(t => t.FechaRegistro)
                        .ToListAsync(cancellationToken);

                    var valoraciones = await _context.ValoracionesFisicas
                        .Where(v => v.CuentaServicio.PacienteId == pacienteId)
                        .ToListAsync(cancellationToken);

                    dto.HistorialTriage = triages.Select(t => {
                        var valoracion = valoraciones.FirstOrDefault(v => v.CuentaServicioId == t.CuentaServicioId);
                        return new CamaMonitoreoTriageDto
                        {
                            FechaRegistro = t.FechaRegistro,
                            UsuarioRegistro = t.UsuarioRegistro,
                            MotivoConsulta = t.MotivoConsulta,
                            TensionArterial = t.TensionArterial,
                            FrecuenciaCardiaca = t.FrecuenciaCardiaca,
                            FrecuenciaRespiratoria = t.FrecuenciaRespiratoria,
                            Temperatura = t.Temperatura,
                            SaturacionO2 = t.SaturacionO2,
                            GlasgowTotal = valoracion?.GlasgowTotal ?? 15
                        };
                    }).ToList();
                }

                result.Add(dto);
            }

            return result;
        }
    }
}
