using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdatePaymentMethodCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public int GrupoMoneda { get; set; }
        public bool EsVuelto { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }

    public class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdatePaymentMethodCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("El ID del método de pago es inválido.");

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("El nombre del método de pago es requerido.");

            if (string.IsNullOrWhiteSpace(request.Valor))
                throw new ArgumentException("El valor interno del método de pago es requerido.");

            if (request.GrupoMoneda != 1 && request.GrupoMoneda != 2)
                throw new ArgumentException("El grupo de moneda debe ser 1 (USD) o 2 (VES).");

            var metodo = await _context.CatalogoMetodosPago
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (metodo == null)
            {
                throw new InvalidOperationException("No se encontró el método de pago especificado.");
            }

            // Validar unicidad del valor si fue modificado
            if (metodo.Valor.ToLower() != request.Valor.ToLower())
            {
                var existe = await _context.CatalogoMetodosPago
                    .AnyAsync(x => x.Valor.ToLower() == request.Valor.ToLower() && x.Id != request.Id, cancellationToken);

                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe otro método de pago con el valor interno '{request.Valor}'.");
                }
            }

            metodo.Update(
                request.Nombre, 
                request.Valor, 
                request.GrupoMoneda, 
                request.EsVuelto, 
                request.Orden, 
                request.Activo
            );

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
