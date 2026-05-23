using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreatePaymentMethodCommand : IRequest<Guid>
    {
        public string Nombre { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public int GrupoMoneda { get; set; }
        public bool EsVuelto { get; set; }
        public int Orden { get; set; }
    }

    public class CreatePaymentMethodCommandHandler : IRequestHandler<CreatePaymentMethodCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreatePaymentMethodCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("El nombre del método de pago es requerido.");

            if (string.IsNullOrWhiteSpace(request.Valor))
                throw new ArgumentException("El valor interno del método de pago es requerido.");

            if (request.GrupoMoneda != 1 && request.GrupoMoneda != 2)
                throw new ArgumentException("El grupo de moneda debe ser 1 (USD) o 2 (VES).");

            var existe = await _context.CatalogoMetodosPago
                .AnyAsync(x => x.Valor.ToLower() == request.Valor.ToLower(), cancellationToken);

            if (existe)
            {
                throw new InvalidOperationException($"Ya existe un método de pago con el valor interno '{request.Valor}'.");
            }

            var metodo = new CatalogoMetodoPago(
                request.Nombre, 
                request.Valor, 
                request.GrupoMoneda, 
                request.EsVuelto, 
                request.Orden
            );

            await _context.CatalogoMetodosPago.AddAsync(metodo, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return metodo.Id;
        }
    }
}
