using System;
using System.Collections.Generic;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CloseAccountCommand : IRequest<CloseAccountResult>
    {
        public Guid CuentaId { get; set; }
        public decimal TasaCambio { get; set; }
        public List<DetallePagoDto> Pagos { get; set; } = new();
        public string UsuarioCajero { get; set; }
        public string UsuarioId { get; set; }
    }
}
