using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetTriageYValoracionHistoryQueryHandler : IRequestHandler<GetTriageYValoracionHistoryQuery, List<TriageYValoracionDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTriageYValoracionHistoryQueryHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<TriageYValoracionDto>> Handle(GetTriageYValoracionHistoryQuery request, CancellationToken cancellationToken)
        {
            var triages = await _context.TriagesEnfermeria
                .AsNoTracking()
                .Where(t => t.CuentaServicioId == request.CuentaServicioId)
                .OrderByDescending(t => t.FechaRegistro)
                .ToListAsync(cancellationToken);

            var valoraciones = await _context.ValoracionesFisicas
                .AsNoTracking()
                .Where(v => v.CuentaServicioId == request.CuentaServicioId)
                .OrderByDescending(v => v.FechaRegistro)
                .ToListAsync(cancellationToken);

            var result = new List<TriageYValoracionDto>();

            for (int i = 0; i < triages.Count; i++)
            {
                var triage = triages[i];
                
                // Buscar la valoración que tenga la fecha de registro más cercana (dentro de un rango de 10 segundos) 
                // o usar la de igual índice si coincide la cantidad de elementos.
                var valoracion = valoraciones.FirstOrDefault(v => Math.Abs((v.FechaRegistro - triage.FechaRegistro).TotalSeconds) < 10)
                                 ?? (i < valoraciones.Count ? valoraciones[i] : null);

                if (valoracion != null)
                {
                    result.Add(new TriageYValoracionDto
                    {
                        TriageId = triage.Id,
                        ValoracionId = valoracion.Id,
                        CuentaServicioId = triage.CuentaServicioId,
                        MotivoConsulta = triage.MotivoConsulta,
                        TensionArterial = triage.TensionArterial,
                        FrecuenciaCardiaca = triage.FrecuenciaCardiaca,
                        FrecuenciaRespiratoria = triage.FrecuenciaRespiratoria,
                        Temperatura = triage.Temperatura,
                        SaturacionO2 = triage.SaturacionO2,
                        GlicemiaCapilar = triage.GlicemiaCapilar,
                        EstadoConciencia = valoracion.EstadoConciencia,
                        GlasgowOcular = valoracion.GlasgowOcular,
                        GlasgowVerbal = valoracion.GlasgowVerbal,
                        GlasgowMotor = valoracion.GlasgowMotor,
                        GlasgowTotal = valoracion.GlasgowTotal,
                        ViaAerea = valoracion.ViaAerea,
                        Ventilacion = valoracion.Ventilacion,
                        Pulso = valoracion.Pulso,
                        PielMucosas = valoracion.PielMucosas,
                        LlenadoCapilar = valoracion.LlenadoCapilar,
                        Pupilas = valoracion.Pupilas,
                        Alergias = valoracion.Alergias,
                        AccesosVenosos = valoracion.AccesosVenosos,
                        Pertenencias = valoracion.Pertenencias,
                        AntecedentesMedicos = valoracion.AntecedentesMedicos,
                        FechaRegistro = triage.FechaRegistro,
                        UsuarioRegistro = triage.UsuarioRegistro,
                        DescripcionRapida = triage.DescripcionRapida,
                        DescripcionDetallada = triage.DescripcionDetallada
                    });
                }
            }

            return result;
        }
    }
}
