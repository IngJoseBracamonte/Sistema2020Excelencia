using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateEspecialidadCommand : IRequest<Guid>
    {
        public string Nombre { get; set; }
    }
}
