using System;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegistrarAltaPacienteCommand : IRequest<RegistrarAltaPacienteResult>
    {
        public Guid PacienteId { get; set; }
        public Guid AdmisionId { get; set; } // Representa el ID de CuentaServicios / Admisión activa
        public TipoAltaEnum TipoAlta { get; set; } = TipoAltaEnum.Normal;
        public string? Observaciones { get; set; }
        public bool ConfirmadoPorEnfermeriaSinSolvencia { get; set; }
        public string? UsuarioAlta { get; set; }
        public string? IpAddress { get; set; }
    }

    public class RegistrarAltaPacienteResult
    {
        public bool Exitoso { get; set; }
        public Guid AdmisionId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public decimal SaldoPendienteUsd { get; set; }
        public string TipoAltaDesc { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
    }
}
