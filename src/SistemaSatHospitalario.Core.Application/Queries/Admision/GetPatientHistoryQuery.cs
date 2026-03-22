using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;

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

        public GetPatientHistoryQueryHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<List<PatientHistoryDto>> Handle(GetPatientHistoryQuery request, CancellationToken cancellationToken)
        {
            var cuentas = await _billingRepository.ObtenerCuentasPorPacienteAsync(request.PacienteId, cancellationToken);

            return cuentas.Select(c => new PatientHistoryDto
            {
                CuentaId = c.Id,
                FechaCreacion = c.FechaCreacion,
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
