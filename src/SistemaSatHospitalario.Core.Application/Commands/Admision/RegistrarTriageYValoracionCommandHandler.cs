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

            // Obtener últimos registros para fusionar/llevar datos si se desactivaron secciones
            var lastTriage = await _context.TriagesEnfermeria
                .Where(t => t.CuentaServicioId == request.CuentaServicioId)
                .OrderByDescending(t => t.FechaRegistro)
                .FirstOrDefaultAsync(cancellationToken);

            var lastValoracion = await _context.ValoracionesFisicas
                .Where(v => v.CuentaServicioId == request.CuentaServicioId)
                .OrderByDescending(v => v.FechaRegistro)
                .FirstOrDefaultAsync(cancellationToken);

            // Determinar valores de Constantes Vitales
            string motivoConsulta = request.RegistrarConstantesVitales ? request.MotivoConsulta : (lastTriage?.MotivoConsulta ?? request.MotivoConsulta ?? "INGRESO");
            string tensionArterial = request.RegistrarConstantesVitales ? request.TensionArterial : (lastTriage?.TensionArterial ?? request.TensionArterial ?? "N/A");
            int frecuenciaCardiaca = request.RegistrarConstantesVitales ? request.FrecuenciaCardiaca : (lastTriage?.FrecuenciaCardiaca ?? request.FrecuenciaCardiaca);
            int frecuenciaRespiratoria = request.RegistrarConstantesVitales ? request.FrecuenciaRespiratoria : (lastTriage?.FrecuenciaRespiratoria ?? request.FrecuenciaRespiratoria);
            decimal temperatura = request.RegistrarConstantesVitales ? request.Temperatura : (lastTriage?.Temperatura ?? request.Temperatura);
            int saturacionO2 = request.RegistrarConstantesVitales ? request.SaturacionO2 : (lastTriage?.SaturacionO2 ?? request.SaturacionO2);
            int? glicemiaCapilar = request.RegistrarConstantesVitales ? request.GlicemiaCapilar : (lastTriage?.GlicemiaCapilar ?? request.GlicemiaCapilar);

            // Determinar descripción de estado del paciente
            string? descripcionRapida = request.RegistrarEstadoActual ? request.DescripcionRapida : (lastTriage?.DescripcionRapida ?? request.DescripcionRapida);
            string? descripcionDetallada = request.RegistrarEstadoActual ? request.DescripcionDetallada : (lastTriage?.DescripcionDetallada ?? request.DescripcionDetallada);

            // Registrar Triage
            var triage = new TriageEnfermeria(
                request.CuentaServicioId,
                motivoConsulta,
                tensionArterial,
                frecuenciaCardiaca,
                frecuenciaRespiratoria,
                temperatura,
                saturacionO2,
                glicemiaCapilar,
                request.UsuarioRegistro,
                descripcionRapida,
                descripcionDetallada
            );

            // Determinar valores de Valoración Física
            string estadoConciencia = request.RegistrarValoracionFisica ? request.EstadoConciencia : (lastValoracion?.EstadoConciencia ?? request.EstadoConciencia ?? "Alerta");
            int glasgowOcular = request.RegistrarValoracionFisica ? request.GlasgowOcular : (lastValoracion?.GlasgowOcular ?? request.GlasgowOcular);
            int glasgowVerbal = request.RegistrarValoracionFisica ? request.GlasgowVerbal : (lastValoracion?.GlasgowVerbal ?? request.GlasgowVerbal);
            int glasgowMotor = request.RegistrarValoracionFisica ? request.GlasgowMotor : (lastValoracion?.GlasgowMotor ?? request.GlasgowMotor);
            int glasgowTotal = request.RegistrarValoracionFisica ? request.GlasgowTotal : (lastValoracion?.GlasgowTotal ?? request.GlasgowTotal);
            string viaAerea = request.RegistrarValoracionFisica ? request.ViaAerea : (lastValoracion?.ViaAerea ?? request.ViaAerea ?? "Permeable");
            string ventilacion = request.RegistrarValoracionFisica ? request.Ventilacion : (lastValoracion?.Ventilacion ?? request.Ventilacion ?? "Normal");
            string pulso = request.RegistrarValoracionFisica ? request.Pulso : (lastValoracion?.Pulso ?? request.Pulso ?? "Rítmico");
            string pielMucosas = request.RegistrarValoracionFisica ? request.PielMucosas : (lastValoracion?.PielMucosas ?? request.PielMucosas ?? "Normocoloreada");
            string llenadoCapilar = request.RegistrarValoracionFisica ? request.LlenadoCapilar : (lastValoracion?.LlenadoCapilar ?? request.LlenadoCapilar ?? "< 2 segundos");
            string pupilas = request.RegistrarValoracionFisica ? request.Pupilas : (lastValoracion?.Pupilas ?? request.Pupilas ?? "Isocóricas");
            string accesosVenosos = request.RegistrarValoracionFisica ? request.AccesosVenosos : (lastValoracion?.AccesosVenosos ?? request.AccesosVenosos ?? "No");
            string pertenencias = request.RegistrarValoracionFisica ? request.Pertenencias : (lastValoracion?.Pertenencias ?? request.Pertenencias ?? "Entregadas a familiar");

            // Determinar valores de Antecedentes Médicos
            string alergias = request.RegistrarAntecedentes ? request.Alergias : (lastValoracion?.Alergias ?? request.Alergias ?? "Ninguna");
            string antecedentesMedicos = request.RegistrarAntecedentes ? request.AntecedentesMedicos : (lastValoracion?.AntecedentesMedicos ?? request.AntecedentesMedicos ?? "Ninguno");

            // Registrar Valoración Física
            var valoracion = new ValoracionFisica(
                request.CuentaServicioId,
                estadoConciencia,
                glasgowOcular,
                glasgowVerbal,
                glasgowMotor,
                glasgowTotal,
                viaAerea,
                ventilacion,
                pulso,
                pielMucosas,
                llenadoCapilar,
                pupilas,
                alergias,
                accesosVenosos,
                pertenencias,
                antecedentesMedicos,
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
                UsuarioRegistro = triage.UsuarioRegistro,
                DescripcionRapida = triage.DescripcionRapida,
                DescripcionDetallada = triage.DescripcionDetallada
            };
        }
    }
}
