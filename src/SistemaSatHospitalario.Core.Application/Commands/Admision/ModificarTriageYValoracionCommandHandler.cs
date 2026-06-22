using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ModificarTriageYValoracionCommandHandler : IRequestHandler<ModificarTriageYValoracionCommand, TriageYValoracionDto>
    {
        private readonly IApplicationDbContext _context;

        public ModificarTriageYValoracionCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TriageYValoracionDto> Handle(ModificarTriageYValoracionCommand request, CancellationToken cancellationToken)
        {
            var triage = await _context.TriagesEnfermeria
                .FirstOrDefaultAsync(t => t.Id == request.TriageId, cancellationToken);

            if (triage == null)
            {
                throw new InvalidOperationException($"El registro de Triage con ID {request.TriageId} no existe.");
            }

            var valoracion = await _context.ValoracionesFisicas
                .FirstOrDefaultAsync(v => v.Id == request.ValoracionId, cancellationToken);

            if (valoracion == null)
            {
                throw new InvalidOperationException($"El registro de Valoración Física con ID {request.ValoracionId} no existe.");
            }

            // Actualizar datos del Triage
            triage.ActualizarDatos(
                request.MotivoConsulta,
                request.TensionArterial,
                request.FrecuenciaCardiaca,
                request.FrecuenciaRespiratoria,
                request.Temperatura,
                request.SaturacionO2,
                request.GlicemiaCapilar,
                request.UsuarioRegistro,
                request.DescripcionRapida,
                request.DescripcionDetallada
            );

            // Actualizar datos de Valoración Física
            valoracion.ActualizarDatos(
                request.EstadoConciencia,
                request.GlasgowOcular,
                request.GlasgowVerbal,
                request.GlasgowMotor,
                request.GlasgowTotal,
                request.ViaAerea,
                request.Ventilacion,
                request.Pulso,
                request.PielMucosas,
                request.LlenadoCapilar,
                request.Pupilas,
                request.Alergias,
                request.AccesosVenosos,
                request.Pertenencias,
                request.AntecedentesMedicos,
                request.UsuarioRegistro
            );

            _context.TriagesEnfermeria.Update(triage);
            _context.ValoracionesFisicas.Update(valoracion);
            await _context.SaveChangesAsync(cancellationToken);

            return new TriageYValoracionDto
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
            };
        }
    }
}
