using System;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetOpenAccountQuery : IRequest<OpenAccountDto?>
    {
        public Guid PacienteId { get; set; }
        public string? TipoIngreso { get; set; }
        public bool Consolidar { get; set; }
    }
}
