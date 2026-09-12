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
                    .ThenInclude(d => d.TipoServicioNav)
                .Include(c => c.Paciente)
                .Include(c => c.Convenio)
                .Include(c => c.AreaClinica)
                .Include(c => c.Medico)
                .Include(c => c.EstadoNav)
                .Include(c => c.TipoIngresoNav)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(c => (c.Paciente != null && c.Paciente.NombreCorto.ToLower().Contains(term)) || 
                                         (c.Paciente != null && c.Paciente.CedulaPasaporte.ToLower().Contains(term)));
            }

            if (!string.IsNullOrEmpty(request.TipoIngreso))
            {
                if (request.TipoIngreso == EstadoConstants.Hospitalizacion)
                {
                    query = query.Where(c => (c.TipoIngresoNav != null && (c.TipoIngresoNav.Nombre == EstadoConstants.Hospitalizacion || c.TipoIngresoNav.Nombre == SubAreas.UCI)) ||
                                             c.TipoIngresoId == TipoIngresoConstants.HospitalizacionId || c.TipoIngresoId == TipoIngresoConstants.UciId);
                }
                else
                {
                    var targetTipoId = TipoIngresoConstants.FromLegacyString(request.TipoIngreso);
                    query = query.Where(c => (c.TipoIngresoNav != null && c.TipoIngresoNav.Nombre == request.TipoIngreso) || c.TipoIngresoId == targetTipoId);
                }
            }

            if (!string.IsNullOrEmpty(request.Estado))
            {
                var targetEstadoId = EstadoCuentaConstants.FromLegacyString(request.Estado);
                query = query.Where(c => (c.EstadoNav != null && c.EstadoNav.Nombre.Contains(request.Estado)) || c.EstadoId == targetEstadoId);
            }

            var cuentas = await query
                .OrderByDescending(c => c.FechaCarga)
                .ToListAsync(cancellationToken);

            var result = new List<CuentaAdministrativaDto>();

            foreach (var c in cuentas)
            {
                var recibo = await _context.RecibosFactura
                    .AsNoTracking()
                    .Where(r => r.CuentaServicioId == c.Id && (r.EstadoFiscalNav == null || r.EstadoFiscalNav.Nombre != EstadoConstants.Anulada))
                    .OrderByDescending(r => r.FechaEmision)
                    .FirstOrDefaultAsync(cancellationToken);

                var totalPagado = await _context.RecibosFactura
                    .AsNoTracking()
                    .Where(r => r.CuentaServicioId == c.Id && (r.EstadoFiscalNav == null || r.EstadoFiscalNav.Nombre != EstadoConstants.Anulada))
                    .SumAsync(r => (decimal?)r.TotalFacturadoUSD, cancellationToken) ?? 0m;

                var totalCuenta = c.CalcularTotal();
                var saldoPendiente = Math.Max(0m, totalCuenta - totalPagado);

                var estadoNombre = c.EstadoNav?.Nombre ?? EstadoCuentaConstants.ToLegacyString(c.EstadoId);

                var dto = new CuentaAdministrativaDto
                {
                    CuentaId = c.Id,
                    PacienteId = c.PacienteId,
                    PacienteNombre = c.Paciente?.NombreCorto ?? "Paciente Desconocido",
                    PacienteCedula = c.Paciente?.CedulaPasaporte ?? string.Empty,
                    FechaCarga = c.FechaCarga,
                    FechaCierre = c.FechaCierre,
                    Estado = estadoNombre,
                    TipoIngreso = c.TipoIngresoNav?.Nombre ?? TipoIngresoConstants.ToLegacyString(c.TipoIngresoId),
                    ConvenioId = c.ConvenioId,
                    SeguroNombre = c.Convenio?.Nombre ?? "PARTICULAR",
                    Total = totalCuenta,
                    TotalPagado = totalPagado,
                    SaldoPendiente = saldoPendiente,
                    ReciboId = recibo?.Id,
                    NumeroRecibo = recibo?.NumeroRecibo,
                    AreaClinicaId = c.AreaClinicaId,
                    AreaClinicaNombre = c.AreaClinica?.Nombre,
                    SubAreaClinica = c.SubAreaClinica,
                    MedicoId = c.MedicoId ?? c.Detalles.FirstOrDefault(d => d.MedicoResponsableId.HasValue)?.MedicoResponsableId,
                    MedicoNombre = c.Medico?.Nombre,
                    Detalles = c.Detalles.Select(d => new CuentaAdministrativaDetailDto
                    {
                        Id = d.Id,
                        ServicioId = d.ServicioId,
                        Descripcion = d.Descripcion,
                        Precio = d.Precio,
                        Honorario = d.Honorario,
                        Cantidad = d.Cantidad,
                        TipoServicio = d.TipoServicioNav?.Nombre ?? (d.TipoServicioId switch
                        {
                            TipoServicioConstants.Medico => "MEDICO",
                            TipoServicioConstants.Laboratorio => "LABORATORIO",
                            TipoServicioConstants.RX => "RX",
                            TipoServicioConstants.Tomo => "TOMO",
                            TipoServicioConstants.Informe => "INFORME",
                            _ => "Insumo"
                        }),
                        FechaCarga = d.FechaCarga,
                        LegacyMappingId = d.LegacyMappingId,
                        IncluidoEnTarifaBase = d.IncluidoEnTarifaBase,
                        MedicoResponsableId = d.MedicoResponsableId
                    }).ToList()
                };

                result.Add(dto);
            }

            return result;
        }
    }
}
