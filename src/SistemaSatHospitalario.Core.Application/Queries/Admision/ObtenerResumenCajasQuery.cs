using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class ResumenCajaGlobalDto
    {
        public List<ResumenTurnoDto> Turnos { get; set; } = new();
    }

    public class ResumenTurnoDto
    {
        public Guid TurnoId { get; set; }
        public string CajeroUserId { get; set; }
        public string Estado { get; set; }
        public decimal RecaudadoBase { get; set; }
    }

    public class ObtenerResumenCajasQuery : IRequest<ResumenCajaGlobalDto>
    {
    }

    public class ObtenerResumenCajasQueryHandler : IRequestHandler<ObtenerResumenCajasQuery, ResumenCajaGlobalDto>
    {
        private readonly IApplicationDbContext _context;

        public ObtenerResumenCajasQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResumenCajaGlobalDto> Handle(ObtenerResumenCajasQuery request, CancellationToken cancellationToken)
        {
            // Buscamos todas las cajas abiertas actualmente (una por usuario/turno)
            var cajasAbiertas = await _context.CajasDiarias
                .Where(c => c.Estado == "Abierta")
                .ToListAsync(cancellationToken);

            // Obtenemos los montos recaudados por cada caja abierta sumando sus recibos
            var turnos = new List<ResumenTurnoDto>();

            foreach (var caja in cajasAbiertas)
            {
                var recaudado = await _context.RecibosFactura
                    .Where(r => r.CajaDiariaId == caja.Id && r.EstadoFiscal != "Anulada")
                    .SelectMany(r => r.DetallesPago)
                    .SumAsync(p => p.EquivalenteAbonadoBase, cancellationToken);

                turnos.Add(new ResumenTurnoDto
                {
                    TurnoId = caja.Id,
                    CajeroUserId = caja.NombreUsuario,
                    Estado = caja.Estado,
                    RecaudadoBase = recaudado
                });
            }

            return new ResumenCajaGlobalDto
            {
                Turnos = turnos
            };
        }
    }
}
