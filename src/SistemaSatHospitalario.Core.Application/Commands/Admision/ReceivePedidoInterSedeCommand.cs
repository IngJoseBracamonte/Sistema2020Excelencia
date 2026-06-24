using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ReceivePedidoInterSedeCommand : IRequest
    {
        public Guid PedidoId { get; set; }
        public string Usuario { get; set; }
        public Dictionary<Guid, decimal> Discrepancias { get; set; } = new Dictionary<Guid, decimal>();
    }
}
