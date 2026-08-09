using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Inventario;

namespace SistemaSatHospitalario.Core.Application.Commands.Inventario
{
    public class RegistrarPagoProveedorCommand : IRequest<PagoProveedorDto>
    {
        public Guid OrdenCompraId { get; set; }
        public decimal MontoAbonadoUSD { get; set; }
        public decimal TasaCambio { get; set; }
        public string MetodoPago { get; set; } = "Transferencia";
        public string Referencia { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class RegistrarPagoProveedorCommandHandler : IRequestHandler<RegistrarPagoProveedorCommand, PagoProveedorDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public RegistrarPagoProveedorCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<PagoProveedorDto> Handle(RegistrarPagoProveedorCommand request, CancellationToken cancellationToken)
        {
            var orden = await _context.OrdenesCompraInventario
                .Include(o => o.Pagos)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCompraId, cancellationToken);

            if (orden == null)
            {
                throw new InvalidOperationException($"La orden de compra de inventario con ID '{request.OrdenCompraId}' no fue encontrada.");
            }

            var userId = _currentUserService.UserName ?? _currentUserService.UserId?.ToString() ?? "admin";
            var pago = orden.RegistrarAbono(
                request.MontoAbonadoUSD,
                request.TasaCambio,
                request.MetodoPago,
                request.Referencia,
                userId,
                request.Observaciones
            );

            _context.PagosProveedores.Add(pago);
            await _context.SaveChangesAsync(cancellationToken);

            return new PagoProveedorDto
            {
                Id = pago.Id,
                OrdenCompraId = orden.Id,
                NumeroFactura = orden.NumeroFactura,
                ProveedorNombre = orden.ProveedorNombre,
                FechaPago = pago.FechaPago,
                MontoAbonadoUSD = pago.MontoAbonadoUSD,
                TasaCambio = pago.TasaCambio,
                MontoAbonadoBs = pago.MontoAbonadoBs,
                MetodoPago = pago.MetodoPago,
                Referencia = pago.Referencia,
                UsuarioId = pago.UsuarioId,
                Observaciones = pago.Observaciones
            };
        }
    }
}
