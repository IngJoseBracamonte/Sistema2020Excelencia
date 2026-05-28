using System;
using System.Collections.Generic;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateARMetadataCommand : IRequest<bool>
    {
        public Guid CuentaPorCobrarId { get; set; }
        public string? QuienAutorizo { get; set; }
        public string? DoctorProcedimiento { get; set; }
        public string? InformacionAdicional { get; set; }
        public List<GarantiaItemDto> GarantiasItems { get; set; } = new();
    }
}
