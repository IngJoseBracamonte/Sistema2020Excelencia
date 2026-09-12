using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ActivarAreaClinicaCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
