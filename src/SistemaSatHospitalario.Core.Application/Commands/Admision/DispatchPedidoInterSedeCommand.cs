using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DispatchPedidoInterSedeCommand : IRequest
    {
        public Guid PedidoId { get; set; }
        public string Usuario { get; set; }
    }
}
