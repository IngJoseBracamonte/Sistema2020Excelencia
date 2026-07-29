using System;
using System.Collections.Generic;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DispatchPedidoInterSedeCommand : IRequest
    {
        public Guid PedidoId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public Dictionary<Guid, decimal>? CantidadesAprobadas { get; set; }
        public Dictionary<Guid, string>? ObservacionesPorDetalle { get; set; }
    }
}
