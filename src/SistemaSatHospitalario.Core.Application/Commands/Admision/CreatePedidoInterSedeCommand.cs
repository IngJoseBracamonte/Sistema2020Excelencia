using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreatePedidoInterSedeCommand : IRequest<Guid>
    {
        public CreatePedidoInterSedeDto Dto { get; set; }
        public string Usuario { get; set; }
    }
}
