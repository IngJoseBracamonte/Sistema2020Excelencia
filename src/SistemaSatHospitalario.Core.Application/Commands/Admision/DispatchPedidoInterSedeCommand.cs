using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DispatchPedidoInterSedeCommand : IRequest
    {
        public Guid PedidoId { get; set; }
        public string Usuario { get; set; }
        public Dictionary<Guid, decimal>? CantidadesAprobadas { get; set; }
    }
}
