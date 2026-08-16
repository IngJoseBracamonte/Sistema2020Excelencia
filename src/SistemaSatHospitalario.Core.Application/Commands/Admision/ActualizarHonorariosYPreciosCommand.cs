using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CirugiaMedicoHonorarioInputDto
    {
        public Guid MedicoId { get; set; }
        public Guid? EspecialidadId { get; set; }
        public decimal MontoHonorarioUsd { get; set; }
        public bool EsCirujanoPrincipal { get; set; }
    }

    public class ActualizarHonorariosYPreciosCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public decimal PrecioDerechoSalaUsd { get; set; }
        public decimal PrecioBaseUsd { get; set; }
        public bool EsAlquilado { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public List<CirugiaMedicoHonorarioInputDto> Medicos { get; set; } = new();
    }

    public class ActualizarHonorariosYPreciosCommandHandler : IRequestHandler<ActualizarHonorariosYPreciosCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ActualizarHonorariosYPreciosCommandHandler> _logger;

        public ActualizarHonorariosYPreciosCommandHandler(
            IApplicationDbContext context,
            ILogger<ActualizarHonorariosYPreciosCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ActualizarHonorariosYPreciosCommand request, CancellationToken cancellationToken)
        {
            var orden = await _context.OrdenesCirugia
                .Include(o => o.MedicosHonorarios)
                .Include(o => o.Logs)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken);

            if (orden == null)
            {
                _logger.LogWarning("Orden de cirugía {OrdenId} no encontrada.", request.OrdenCirugiaId);
                return false;
            }

            orden.ActualizarPreciosAdministrativos(
                request.PrecioDerechoSalaUsd,
                request.PrecioBaseUsd,
                request.EsAlquilado,
                request.UsuarioId);

            var existentes = await _context.CirugiasMedicosHonorarios
                .Where(m => m.OrdenCirugiaId == orden.Id)
                .ToListAsync(cancellationToken);

            _context.CirugiasMedicosHonorarios.RemoveRange(existentes);

            var medicosIds = request.Medicos.Select(m => m.MedicoId).Distinct().ToList();
            var medicosDb = await _context.Medicos
                .Where(m => medicosIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, cancellationToken);

            foreach (var m in request.Medicos)
            {
                if (!medicosDb.TryGetValue(m.MedicoId, out var medicoEntity))
                {
                    _logger.LogWarning("Médico {MedicoId} no encontrado al asignar honorario.", m.MedicoId);
                    continue;
                }

                var especialidadId = m.EspecialidadId.HasValue && m.EspecialidadId.Value != Guid.Empty
                    ? m.EspecialidadId.Value
                    : medicoEntity.EspecialidadId;

                var nuevo = new CirugiaMedicoHonorario(orden.Id, m.MedicoId, especialidadId, m.MontoHonorarioUsd, m.EsCirujanoPrincipal);
                _context.CirugiasMedicosHonorarios.Add(nuevo);
            }

            var log = new CirugiaLog(orden.Id, request.UsuarioId, "AjustePrecios",
                $"Precios actualizados: DerechoSala=${request.PrecioDerechoSalaUsd:N2}, Base=${request.PrecioBaseUsd:N2}, PabellónAlquilado={request.EsAlquilado}");
            _context.CirugiaLogs.Add(log);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Precios y honorarios de la orden {OrdenId} actualizados por {UsuarioId}.", orden.Id, request.UsuarioId);
            return true;
        }
    }
}
