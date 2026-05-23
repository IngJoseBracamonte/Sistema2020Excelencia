using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeletePaymentMethodCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeletePaymentMethodCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("El ID del método de pago es inválido.");

            var metodo = await _context.CatalogoMetodosPago
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (metodo == null)
            {
                return false; // Already deleted or not found
            }

            // Verificar si tiene transacciones asociadas (DetallesPago)
            var hasPayments = await _context.DetallesPago
                .AnyAsync(dp => dp.MetodoPago == metodo.Valor || dp.MetodoPago == metodo.Nombre, cancellationToken);

            if (hasPayments)
            {
                // Desactivar para preservar la integridad de datos históricos
                metodo.SetActivo(false);
            }
            else
            {
                // Eliminación física si no tiene transacciones asociadas
                _context.CatalogoMetodosPago.Remove(metodo);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
