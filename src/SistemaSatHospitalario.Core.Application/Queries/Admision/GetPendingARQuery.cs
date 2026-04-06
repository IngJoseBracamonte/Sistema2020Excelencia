using System.Collections.Generic;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetPendingARQuery : IRequest<List<PendingARDto>>
    {
        public string? SearchTerm { get; set; }
        public string? Estado { get; set; } // Pendiente, Cobrada
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
