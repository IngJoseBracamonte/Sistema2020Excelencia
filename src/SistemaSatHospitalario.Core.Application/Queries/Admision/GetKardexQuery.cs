using System;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetKardexQuery : IRequest<KardexResultDto>
    {
        public Guid? SedeId { get; set; }
        public Guid? InsumoId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}
