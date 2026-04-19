using System;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CerrarCajaCommand : IRequest<CerrarCajaResult>
    {
        public string UsuarioId { get; set; }
    }

    public class CerrarCajaCommandHandler : IRequestHandler<CerrarCajaCommand, CerrarCajaResult>
    {
        private readonly ICajaAdministrativaRepository _repository;
        private readonly IApplicationDbContext _context;

        public CerrarCajaCommandHandler(ICajaAdministrativaRepository repository, IApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CerrarCajaResult> Handle(CerrarCajaCommand request, CancellationToken cancellationToken)
        {
            // Cerramos específicamente la caja abierta por este usuario
            var cajaAbierta = await _repository.ObtenerCajaAbiertaPorUsuarioAsync(request.UsuarioId, cancellationToken);
            if (cajaAbierta == null)
            {
                throw new InvalidOperationException("No se encontró ninguna caja abierta para este usuario.");
            }

            // [PHASE-9] Automated Z-Report Calculation
            // Obtenemos todos los recibos vinculados a esta sesión de caja
            var recibos = await _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .Where(r => r.CajaDiariaId == cajaAbierta.Id)
                .ToListAsync(cancellationToken);

            var totalUSD = recibos.Sum(r => r.TotalFacturadoUSD);
            var totalVuelto = recibos.Sum(r => r.MontoVueltoUSD);
            
            // Cálculo de Bolívares (Suma de pagos en métodos de moneda local)
            // Métodos BS: PagoMovil, PuntoVenta, EfectivoBS, TransferenciaBS
            var totalBS = recibos.SelectMany(r => r.DetallesPago)
                .Where(p => p.MetodoPago.ToUpper().Contains("BS") || p.MetodoPago.ToUpper().Contains("MOVIL") || p.MetodoPago.ToUpper().Contains("PUNTO"))
                .Sum(p => p.MontoAbonadoMoneda);

            cajaAbierta.CerrarCaja();
            await _repository.GuardarCambiosAsync(cancellationToken);

            return new CerrarCajaResult
            {
                CajaId = cajaAbierta.Id,
                TotalIngresosUSD = totalUSD, 
                TotalVueltoUSD = totalVuelto,
                TotalIngresosBS = totalBS,
                ConteoVentas = recibos.Count,
                Usuario = cajaAbierta.NombreUsuario,
                FechaCierre = DateTime.UtcNow
            };
        }
    }
}
