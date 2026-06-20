using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetOpenAccountQueryHandler : IRequestHandler<GetOpenAccountQuery, OpenAccountDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetOpenAccountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OpenAccountDto?> Handle(GetOpenAccountQuery request, CancellationToken cancellationToken)
        {
            var query = _context.CuentasServicios
                .Include(c => c.Detalles)
                .AsNoTracking()
                .Where(c => c.PacienteId == request.PacienteId && c.Estado == EstadoConstants.Abierta);

            if (!string.IsNullOrEmpty(request.TipoIngreso))
            {
                query = query.Where(c => c.TipoIngreso == request.TipoIngreso);
            }

            var activeCuenta = await query.FirstOrDefaultAsync(cancellationToken);
            if (activeCuenta == null) return null;

            var detallesDto = new List<OpenAccountDetailDto>();
            List<CuentaServicios> accountsToProcess;

            if (request.Consolidar)
            {
                var rootId = activeCuenta.CuentaPrincipalId ?? activeCuenta.Id;
                var allChainAccounts = await _context.CuentasServicios
                    .Include(c => c.Detalles)
                    .AsNoTracking()
                    .Where(c => c.Id == rootId || c.CuentaPrincipalId == rootId)
                    .ToListAsync(cancellationToken);

                var billedAccountIds = await _context.RecibosFactura
                    .Select(rf => rf.CuentaServicioId)
                    .ToListAsync(cancellationToken);

                var arAccountIds = await _context.CuentasPorCobrar
                    .Select(ar => ar.CuentaServicioId)
                    .ToListAsync(cancellationToken);

                accountsToProcess = allChainAccounts
                    .Where(c => !billedAccountIds.Contains(c.Id) && !arAccountIds.Contains(c.Id))
                    .ToList();
            }
            else
            {
                accountsToProcess = new List<CuentaServicios> { activeCuenta };
            }

            foreach (var cuenta in accountsToProcess)
            {
                foreach (var d in cuenta.Detalles)
                {
                    string? medicoNombre = null;
                    if (d.MedicoResponsableId.HasValue)
                    {
                        var medico = await _context.Medicos.FindAsync(new object[] { d.MedicoResponsableId.Value }, cancellationToken);
                        medicoNombre = medico?.Nombre;
                    }

                    var desc = (request.Consolidar && accountsToProcess.Count > 1) 
                        ? $"[{cuenta.TipoIngreso}] {d.Descripcion}" 
                        : d.Descripcion;

                    detallesDto.Add(new OpenAccountDetailDto
                    {
                        Id = d.Id,
                        ServicioId = d.ServicioId,
                        Descripcion = desc,
                        Precio = d.Precio,
                        Honorario = d.Honorario,
                        Cantidad = d.Cantidad,
                        TipoServicio = d.TipoServicio,
                        FechaCarga = d.FechaCarga,
                        LegacyMappingId = d.LegacyMappingId,
                        MedicoResponsableId = d.MedicoResponsableId,
                        MedicoNombre = medicoNombre
                    });
                }
            }

            return new OpenAccountDto
            {
                Id = activeCuenta.Id,
                PacienteId = activeCuenta.PacienteId,
                TipoIngreso = activeCuenta.TipoIngreso,
                ConvenioId = activeCuenta.ConvenioId,
                Detalles = detallesDto
            };
        }
    }
}
