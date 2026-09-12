using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeleteAreaClinicaCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
