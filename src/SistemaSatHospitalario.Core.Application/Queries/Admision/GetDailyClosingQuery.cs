using System;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetDailyClosingQuery : IRequest<DailyClosingDto>
    {
        public string UserId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Today;
    }
}
