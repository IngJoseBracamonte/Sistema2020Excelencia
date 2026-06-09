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

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetCuentasAdministrativasQuery : IRequest<List<CuentaAdministrativaDto>>
    {
        public string? SearchTerm { get; set; }
        public string? TipoIngreso { get; set; }
        public string? Estado { get; set; }
    }

    public class GetCuentasAdministrativasQueryHandler : IRequestHandler<GetCuentasAdministrativasQuery, List<CuentaAdministrativaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCuentasAdministrativasQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CuentaAdministrativaDto>> Handle(GetCuentasAdministrativasQuery request, CancellationToken cancellationToken)
        {
            var query = _context.CuentasServicios
                .Include(c => c.Detalles)
                .Include(c => c.Paciente)
                .Include(c => c.Convenio)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(c => c.Paciente.NombreCorto.ToLower().Contains(term) || c.Paciente.CedulaPasaporte.ToLower().Contains(term));
            }

            if (!string.IsNullOrEmpty(request.TipoIngreso))
            {
                query = query.Where(c => c.TipoIngreso == request.TipoIngreso);
            }

            if (!string.IsNullOrEmpty(request.Estado))
            {
                query = query.Where(c => c.Estado == request.Estado);
            }

            var cuentas = await query
                .OrderByDescending(c => c.FechaCarga)
                .Take(50)
                .ToListAsync(cancellationToken);

            var result = new List<CuentaAdministrativaDto>();

            foreach (var c in cuentas)
            {
                var recibo = await _context.RecibosFactura
                    .AsNoTracking()
                    .Where(r => r.CuentaServicioId == c.Id && r.EstadoFiscal != EstadoConstants.Anulada)
                    .OrderByDescending(r => r.FechaEmision)
                    .FirstOrDefaultAsync(cancellationToken);

                var dto = new CuentaAdministrativaDto
                {
                    CuentaId = c.Id,
                    PacienteId = c.PacienteId,
                    PacienteNombre = c.Paciente?.NombreCorto ?? "Paciente Desconocido",
                    PacienteCedula = c.Paciente?.CedulaPasaporte ?? string.Empty,
                    FechaCarga = c.FechaCarga,
                    FechaCierre = c.FechaCierre,
                    Estado = c.Estado,
                    TipoIngreso = c.TipoIngreso,
                    ConvenioId = c.ConvenioId,
                    SeguroNombre = c.Convenio?.Nombre ?? "PARTICULAR",
                    Total = c.CalcularTotal(),
                    ReciboId = recibo?.Id,
                    NumeroRecibo = recibo?.NumeroRecibo,
                    Detalles = c.Detalles.Select(d => new CuentaAdministrativaDetailDto
                    {
                        Id = d.Id,
                        ServicioId = d.ServicioId,
                        Descripcion = d.Descripcion,
                        Precio = d.Precio,
                        Honorario = d.Honorario,
                        Cantidad = d.Cantidad,
                        TipoServicio = d.TipoServicio,
                        FechaCarga = d.FechaCarga,
                        LegacyMappingId = d.LegacyMappingId
                    }).ToList()
                };

                result.Add(dto);
            }

            return result;
        }
    }
}
