using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateSedeCommand : IRequest<Guid>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool EsPrincipal { get; set; }
    }
}
