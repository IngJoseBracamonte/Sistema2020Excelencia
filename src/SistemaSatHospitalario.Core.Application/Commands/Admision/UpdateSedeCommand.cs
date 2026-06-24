using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateSedeCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool EsPrincipal { get; set; }
    }
}
