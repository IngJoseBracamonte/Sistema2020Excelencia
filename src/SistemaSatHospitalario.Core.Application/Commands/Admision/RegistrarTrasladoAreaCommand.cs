using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegistrarTrasladoAreaCommand : IRequest<RegistrarTrasladoAreaResult>
    {
        public Guid CuentaId { get; set; }
        public string AreaDestino { get; set; } = string.Empty; // EMERGENCIA, HOSPITALIZACION, UCI
        public Guid CamaDestinoId { get; set; }
        public int CantidadHoras { get; set; } = 1;
        public bool CambiaMedicoTratante { get; set; }
        public Guid? NuevoMedicoId { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public decimal MontoACobrarUsd { get; set; }
        public string UsuarioTraslado { get; set; } = string.Empty;
    }

    public class RegistrarTrasladoAreaResult
    {
        public Guid CuentaId { get; set; }
        public Guid CamaDestinoId { get; set; }
        public Guid? DetalleCargoId { get; set; }
        public decimal MontoCargadoUsd { get; set; }
        public bool Exitoso { get; set; }
    }
}
