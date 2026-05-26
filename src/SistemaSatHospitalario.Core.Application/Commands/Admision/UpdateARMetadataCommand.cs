using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateARMetadataCommand : IRequest<bool>
    {
        public Guid CuentaPorCobrarId { get; set; }
        public string? QuienAutorizo { get; set; }
        public string? DoctorProcedimiento { get; set; }
        public string? InformacionAdicional { get; set; }
    }
}
