using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateAreaClinicaCommand : IRequest<Guid>
    {
        public Guid SedeId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
}
