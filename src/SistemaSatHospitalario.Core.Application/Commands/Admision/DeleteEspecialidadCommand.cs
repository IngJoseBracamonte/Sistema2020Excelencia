using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeleteEspecialidadCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
