using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetPatientHistoryQuery : IRequest<List<PatientHistoryDto>>
    {
        // Se cambió de Guid a int para sincronización con Legacy
        public int PacienteId { get; set; }
    }

    public class GetPatientHistoryQueryHandler : IRequestHandler<GetPatientHistoryQuery, List<PatientHistoryDto>>
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IApplicationDbContext _context;

        public GetPatientHistoryQueryHandler(IBillingRepository billingRepository, IApplicationDbContext context)
        {
            _billingRepository = billingRepository;
            _context = context;
        }

        public async Task<List<PatientHistoryDto>> Handle(GetPatientHistoryQuery request, CancellationToken cancellationToken)
        {
            // V11.0: Traducimos la identidad legacy a la interna para la búsqueda de historial
            var internalId = await _context.PacientesAdmision
                .Where(p => p.IdPacienteLegacy == request.PacienteId)
                .Select(p => (Guid?)p.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!internalId.HasValue) return new List<PatientHistoryDto>();

            var cuentas = await _billingRepository.ObtenerCuentasPorPacienteAsync(internalId.Value, cancellationToken);

            return cuentas.Select(c => new PatientHistoryDto
            {
                CuentaId = c.Id,
                FechaCreacion = c.FechaCarga, // Alineado con Domain
                FechaCierre = c.FechaCierre,
                Estado = c.Estado,
                TipoIngreso = c.TipoIngreso,
                Total = c.CalcularTotal(),
                Servicios = c.Detalles.Select(d => new HistoryServiceDetailDto
                {
                    Descripcion = d.Descripcion,
                    Precio = d.Precio,
                    Cantidad = d.Cantidad,
                    TipoServicio = d.TipoServicio
                }).ToList()
            }).ToList();
        }
    }
}
