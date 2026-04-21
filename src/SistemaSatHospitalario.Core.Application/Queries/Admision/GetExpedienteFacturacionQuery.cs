using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetExpedienteFacturacionQuery : IRequest<List<ExpedienteFacturacionDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SearchTerm { get; set; }
    }

    public class GetExpedienteFacturacionQueryHandler : IRequestHandler<GetExpedienteFacturacionQuery, List<ExpedienteFacturacionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public GetExpedienteFacturacionQueryHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        public async Task<List<ExpedienteFacturacionDto>> Handle(GetExpedienteFacturacionQuery request, CancellationToken cancellationToken)
        {
            var config = await _context.ConfiguracionGeneral.FirstOrDefaultAsync(cancellationToken);
            bool facturarLaboratorio = config?.FacturarLaboratorio ?? false;

            var query = from d in _context.DetallesServicioCuenta
                        join c in _context.CuentasServicios on d.CuentaServicioId equals c.Id
                        join p in _context.PacientesAdmision on c.PacienteId equals p.Id
                        join rf in _context.RecibosFactura on c.Id equals rf.CuentaServicioId into rfGroup
                        from rf in rfGroup.DefaultIfEmpty()
                        join sm in _context.SegurosConvenios on c.ConvenioId equals sm.Id into smGroup
                        from sm in smGroup.DefaultIfEmpty()
                        select new { d, c, p, rf, sm };

            // Filtros de fecha (V12.2): Límites precisos del día
            if (request.StartDate.HasValue)
                query = query.Where(x => x.d.FechaCarga >= request.StartDate.Value.Date);
            
            if (request.EndDate.HasValue)
                query = query.Where(x => x.d.FechaCarga <= request.EndDate.Value.Date.AddDays(1).AddTicks(-1));

            // Filtro Laboratorio
            if (!facturarLaboratorio)
            {
                query = query.Where(x => x.d.TipoServicio != "Laboratorio");
            }

            // Filtro de búsqueda
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => x.p.NombreCorto.Contains(request.SearchTerm) || x.p.CedulaPasaporte.Contains(request.SearchTerm));
            }

            var results = await query.OrderByDescending(x => x.d.FechaCarga).ToListAsync(cancellationToken);

            // Fetch users for Biller/Role logic (Joseph Bracamonte, Asistente Particular)
            var users = await _identityService.GetUsersAsync();
            var userMap = users.ToDictionary(u => u.Id.ToString(), u => u);

            return results.Select(x => {
                // El facturador es el que cargó el servicio si está pendiente, 
                // o el que facturó (de la caja) si está facturado.
                // Según el usuario: "Nombre Real, Rol"
                string facturadorId = x.rf != null ? (x.rf.CajaDiariaId.HasValue ? _context.CajasDiarias.FirstOrDefault(cd => cd.Id == x.rf.CajaDiariaId)?.UsuarioId : x.d.UsuarioCarga) : x.d.UsuarioCarga;
                
                string facturadorInfo = "SISTEMA";
                if (!string.IsNullOrEmpty(facturadorId) && userMap.TryGetValue(facturadorId, out var user))
                {
                    var role = user.Roles.FirstOrDefault()?.Replace("Asistente", "Asist.");
                    facturadorInfo = $"{user.FullName}, {role ?? "Personal"}";
                }

                // Obtener método de pago si existe factura
                string metodo = "N/A";
                if (x.rf != null)
                {
                    var payment = _context.DetallesPago.FirstOrDefault(dp => dp.ReciboFacturaId == x.rf.Id);
                    metodo = payment?.MetodoPago ?? "CRÉDITO";
                }

                return new ExpedienteFacturacionDto
                {
                    Id = x.d.Id,
                    Fecha = x.d.FechaCarga,
                    PacienteNombre = x.p.NombreCorto,
                    PacienteCedula = x.p.CedulaPasaporte,
                    PacienteTelefono = x.p.TelefonoContact,
                    Estudio = x.d.Descripcion,
                    TipoIngreso = x.c.TipoIngreso,
                    SeguroNombre = x.sm?.Nombre ?? "PARTICULAR",
                    MetodoPago = metodo,
                    MontoUSD = x.d.Precio * x.d.Cantidad,
                    FacturadoPor = facturadorInfo,
                    Estado = x.c.Estado == EstadoConstants.Facturada ? "Facturado" : "Pendiente",
                    TipoServicio = x.d.TipoServicio
                };
            }).ToList();
        }
    }
}
