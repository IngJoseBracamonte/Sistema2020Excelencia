using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RevertirCheckOutCommand : IRequest<bool>
    {
        public Guid CuentaId { get; set; }
        public string UsuarioReversion { get; set; } = string.Empty;

        public RevertirCheckOutCommand(Guid cuentaId, string usuarioReversion)
        {
            CuentaId = cuentaId;
            UsuarioReversion = usuarioReversion ?? throw new ArgumentNullException(nameof(usuarioReversion));
        }
    }

    public class RevertirCheckOutCommandHandler : IRequestHandler<RevertirCheckOutCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<RevertirCheckOutCommandHandler> _logger;

        public RevertirCheckOutCommandHandler(IApplicationDbContext context, ILogger<RevertirCheckOutCommandHandler> _logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this._logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<bool> Handle(RevertirCheckOutCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando reversión de Check-Out (Reabrir Cuenta) para Cuenta {CuentaId} por {Usuario}", request.CuentaId, request.UsuarioReversion);

            // 1. Obtener la cuenta con su AreaClinica
            var cuenta = await _context.CuentasServicios
                .Include(c => c.AreaClinica)
                .FirstOrDefaultAsync(c => c.Id == request.CuentaId, cancellationToken);

            if (cuenta == null)
            {
                throw new InvalidOperationException($"No se encontró la cuenta con ID {request.CuentaId}");
            }

            // 2. Reabrir la cuenta
            cuenta.Reabrir();

            // 3. Rollback de la cama
            if (cuenta.AreaClinicaId.HasValue)
            {
                var cama = await _context.AreasClinicas.FirstOrDefaultAsync(a => a.Id == cuenta.AreaClinicaId.Value, cancellationToken);
                if (cama != null)
                {
                    // Volver a marcar la cama como ocupada
                    cama.MarcarComoOcupada();
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Reversión de Check-Out completada exitosamente para la cuenta {CuentaId}", request.CuentaId);
            return true;
        }
    }
}
