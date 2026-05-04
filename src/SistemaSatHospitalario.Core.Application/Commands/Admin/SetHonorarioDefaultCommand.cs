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
    public class SetHonorarioDefaultCommand : IRequest<Unit>
    {
        public string CategoriaServicio { get; set; } = string.Empty;
        public Guid? MedicoId { get; set; }
        public string? Observaciones { get; set; }
    }

    public class SetHonorarioDefaultCommandHandler : IRequestHandler<SetHonorarioDefaultCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public SetHonorarioDefaultCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(SetHonorarioDefaultCommand request, CancellationToken ct)
        {
            var config = await _context.HonorariosConfig
                .FirstOrDefaultAsync(h => h.CategoriaServicio == request.CategoriaServicio, ct);

            var usuario = _currentUser.UserName ?? "Sistema";

            if (config == null)
            {
                config = new HonorarioConfig(request.CategoriaServicio, usuario);
                _context.HonorariosConfig.Add(config);
            }

            if (request.MedicoId.HasValue)
                config.AsignarMedicoDefault(request.MedicoId.Value, usuario, request.Observaciones);
            else
                config.LimpiarMedicoDefault(usuario);

            // Log de auditoría
            string? medicoNombre = null;
            if (request.MedicoId.HasValue)
            {
                var medico = await _context.Medicos.FindAsync(new object[] { request.MedicoId.Value }, ct);
                medicoNombre = medico?.Nombre;
            }

            var log = new LogAsignacionHonorario(
                Guid.Empty, request.CategoriaServicio, HonorarioConstants.AccionConfiguracionCambio,
                null, null,
                request.MedicoId, medicoNombre,
                usuario, request.Observaciones);
            _context.LogsAsignacionHonorario.Add(log);

            await _context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
