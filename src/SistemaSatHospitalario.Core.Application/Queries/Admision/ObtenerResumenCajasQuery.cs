using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

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
        private readonly IUserResolverService _userResolver;

        public ObtenerResumenCajasQueryHandler(IApplicationDbContext context, IUserResolverService userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<ResumenCajaGlobalDto> Handle(ObtenerResumenCajasQuery request, CancellationToken cancellationToken)
        {
            var cajasAbiertas = await _context.CajasDiarias
                .Where(c => c.EstadoId == EstadoCajaConstants.AbiertaId)
                .ToListAsync(cancellationToken);

            var turnos = new List<ResumenTurnoDto>();

            var cajaUserIds = cajasAbiertas
                .Where(c => c.UsuarioIdentityId.HasValue)
                .Select(c => c.UsuarioIdentityId!.Value)
                .Distinct()
                .ToList();

            var userMap = await _userResolver.GetDisplayNameMapAsync(cajaUserIds, cancellationToken);

            foreach (var caja in cajasAbiertas)
            {
                var recaudado = await _context.RecibosFactura
                    .Where(r => r.CajaDiariaId == caja.Id && r.EstadoFiscalNav.Nombre != EstadoConstants.Anulada)
                    .SelectMany(r => r.DetallesPago)
                    .SumAsync(p => p.EquivalenteAbonadoBase, cancellationToken);

                turnos.Add(new ResumenTurnoDto
                {
                    TurnoId = caja.Id,
                    CajeroUserId = caja.UsuarioIdentityId.HasValue && userMap.TryGetValue(caja.UsuarioIdentityId.Value, out var nombre) ? nombre : "Sistema",
                    Estado = EstadoCajaConstants.ToLegacyString(caja.EstadoId),
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