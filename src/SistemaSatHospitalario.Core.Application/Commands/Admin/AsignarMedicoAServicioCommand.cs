using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admin
{
    public class AsignarMedicoAServicioCommand : IRequest<Unit>
    {
        public Guid DetalleServicioId { get; set; }
        public Guid MedicoId { get; set; }
        public string CategoriaHonorario { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class AsignarMedicoAServicioCommandHandler : IRequestHandler<AsignarMedicoAServicioCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public AsignarMedicoAServicioCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(AsignarMedicoAServicioCommand request, CancellationToken ct)
        {
            var detalle = await _context.DetallesServicioCuenta.FindAsync(new object[] { request.DetalleServicioId }, ct);
            if (detalle == null) throw new InvalidOperationException("Detalle no encontrado.");

            var medico = await _context.Medicos.FindAsync(new object[] { request.MedicoId }, ct);
            if (medico == null) throw new InvalidOperationException("Médico no encontrado.");

            // Guardar estado anterior para auditoría
            var medicoAnteriorId = detalle.MedicoResponsableId;
            string? medicoAnteriorNombre = null;
            if (medicoAnteriorId.HasValue)
            {
                var anterior = await _context.Medicos.FindAsync(new object[] { medicoAnteriorId.Value }, ct);
                medicoAnteriorNombre = anterior?.Nombre;
            }

            var tipoAccion = medicoAnteriorId.HasValue ? HonorarioConstants.AccionReasignacion : HonorarioConstants.AccionAsignacionManual;

            detalle.AsignarMedicoResponsable(request.MedicoId, request.CategoriaHonorario);

            var log = new LogAsignacionHonorario(
                request.DetalleServicioId, detalle.Descripcion, tipoAccion,
                medicoAnteriorId, medicoAnteriorNombre,
                request.MedicoId, medico.Nombre,
                _currentUser.UserName ?? "Sistema", request.Observaciones);
            _context.LogsAsignacionHonorario.Add(log);

            await _context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
