using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeleteSedeCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
