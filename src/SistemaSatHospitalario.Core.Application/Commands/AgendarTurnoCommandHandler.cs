using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class AgendarTurnoCommandHandler : IRequestHandler<AgendarTurnoCommand, Guid>
    {
        private readonly ITurnoMedicoRepository _turnoRepository;
        private readonly IAuditoriaIncidenciaRepository _auditoriaRepository;

        // Simulamos inyección del operador actual desde un ClaimsPrincipal
        private readonly Guid _currentOperadorId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public AgendarTurnoCommandHandler(
            ITurnoMedicoRepository turnoRepository,
            IAuditoriaIncidenciaRepository auditoriaRepository)
        {
            _turnoRepository = turnoRepository ?? throw new ArgumentNullException(nameof(turnoRepository));
            _auditoriaRepository = auditoriaRepository ?? throw new ArgumentNullException(nameof(auditoriaRepository));
        }

        public async Task<Guid> Handle(AgendarTurnoCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar si existe incidencia en ese horario
            var incidencia = await _turnoRepository.ObtenerIncidenciaSolaParaHoraAsync(request.MedicoId, request.FechaHoraToma, cancellationToken);

            if (incidencia != null)
            {
                if (!request.IgnorarIncidencia)
                {
                    throw new InvalidOperationException($"El horario solicitado presenta una incidencia de tipo {incidencia.Tipo}: {incidencia.Descripcion}. Debe confirmar la omisión explicita para agendar.");
                }

                if (!request.IncidenciaIgnoradaId.HasValue || request.IncidenciaIgnoradaId.Value != incidencia.Id)
                {
                     throw new ArgumentException("Debe proveer el ID correcto de la incidencia que está intentando sobreescribir.");
                }
            }

            // 2. Crear turno
            var turno = new TurnoMedico(
                request.MedicoId,
                request.PacienteId,
                request.FechaHoraToma,
                request.IgnorarIncidencia,
                request.IncidenciaIgnoradaId
            );

            await _turnoRepository.AgregarAsync(turno, cancellationToken);

            // 3. Registrar auditoría si hubo salto
            if (incidencia != null && request.IgnorarIncidencia)
            {
                var auditoria = new RegistroAuditoriaIncidencia(
                    turno.Id,
                    incidencia.Id,
                    _currentOperadorId
                );
                
                await _auditoriaRepository.RegistrarAsync(auditoria, cancellationToken);
            }

            return turno.Id;
        }
    }
}
