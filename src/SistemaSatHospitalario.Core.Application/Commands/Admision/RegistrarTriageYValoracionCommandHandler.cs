using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegistrarTriageYValoracionCommandHandler : IRequestHandler<RegistrarTriageYValoracionCommand, TriageYValoracionDto>
    {
        private readonly IApplicationDbContext _context;

        public RegistrarTriageYValoracionCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TriageYValoracionDto> Handle(RegistrarTriageYValoracionCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _context.CuentasServicios
                .FirstOrDefaultAsync(c => c.Id == request.CuentaServicioId, cancellationToken);

            if (cuenta == null)
            {
                throw new InvalidOperationException($"La cuenta con ID {request.CuentaServicioId} no existe.");
            }

            // Registrar Triage
            var triage = new TriageEnfermeria(
                request.CuentaServicioId,
                request.MotivoConsulta,
                request.TensionArterial,
                request.FrecuenciaCardiaca,
                request.FrecuenciaRespiratoria,
                request.Temperatura,
                request.SaturacionO2,
                request.GlicemiaCapilar,
                request.UsuarioRegistro
            );

            // Registrar Valoración Física
            var valoracion = new ValoracionFisica(
                request.CuentaServicioId,
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

            await _context.TriagesEnfermeria.AddAsync(triage, cancellationToken);
            await _context.ValoracionesFisicas.AddAsync(valoracion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new TriageYValoracionDto
            {
                TriageId = triage.Id,
                ValoracionId = valoracion.Id,
                CuentaServicioId = request.CuentaServicioId,
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
                UsuarioRegistro = triage.UsuarioRegistro
            };
        }
    }
}
