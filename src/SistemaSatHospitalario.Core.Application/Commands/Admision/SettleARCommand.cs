using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class SettleARCommand : IRequest<bool>
    {
        public Guid ARId { get; set; }
        public string ReferenciaPago { get; set; }
        public string Observaciones { get; set; }
    }
}
