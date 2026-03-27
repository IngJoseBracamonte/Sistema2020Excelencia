using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateTasaCambioCommand : IRequest<bool>
    {
        public decimal Monto { get; set; }
    }

    public class UpdateTasaCambioCommandHandler : IRequestHandler<UpdateTasaCambioCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITasaNotificationService _notificationService;

        public UpdateTasaCambioCommandHandler(IApplicationDbContext context, ITasaNotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(UpdateTasaCambioCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Sanear base de datos: desactivar todas las previas (independientemente de estado actual)
                var todasLasTasas = await _context.TasaCambio
                    .ToListAsync(cancellationToken);

                foreach (var t in todasLasTasas)
                {
                    t.Deactivate();
                }

                // 2. Agregar nueva tasa
                var nuevaTasa = new TasaCambio(request.Monto);
                _context.TasaCambio.Add(nuevaTasa);

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                
                if (success)
                {
                    await transaction.CommitAsync(cancellationToken);
                    await _notificationService.NotifyTasaUpdatedAsync(request.Monto, cancellationToken);
                    return true;
                }

                return false;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
