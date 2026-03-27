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
        // Se estandarizó de int a Guid para identidad nativa (V11.1)
        public Guid PacienteId { get; set; }
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
            // V11.1: Búsqueda directa por identidad nativa
            var cuentas = await _billingRepository.ObtenerCuentasPorPacienteAsync(request.PacienteId, cancellationToken);

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
